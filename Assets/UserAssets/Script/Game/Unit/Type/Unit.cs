using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int unitId;        //식별번호

    private UnitAnimationController m_UnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region logic Info
    private bool mIsAlive;
    private bool mIsBatch;
    private bool mIsMoving = true;

    private float attackDelay; // 공격 속도 계산용
    private int mPriority;

    private Vector3 mVectorDestination;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    #endregion

    #region Property
    public bool IsClicked { get; set; }
    public bool IsEnemy { get; private set; }

    #endregion

    protected virtual void Awake()
    {
        m_UnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = Hp = mUnitScriptable.maxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        mIsBatch = true;
        FindenemyOrNull();
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        attackDelay += Time.deltaTime;
        
        if (mTarget != null) // 타깃이 있을 때
        {
            if(!mTarget.IsAlive) // 타깃 사망 확인 
            {
                FindenemyOrNull();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
        m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
    }

    /// <summary>
    /// 타깃이 있고 살아있을 때
    /// </summary>
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        Debug.Log("타깃 타입 : " + mTarget.mUnitScriptable.unitName + "남은 거리: " + dist);
        if (dist <= mUnitScriptable.attackRange) // 타깃이 공격 사정 범위로 들어왔을때 -> 정지하고 공격
        {
            mIsMoving = false;
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (attackDelay >= mUnitScriptable.attackSpeed)
            {
                Debug.Log("공격");
                mAttack.Attack(this, mTarget);
                attackDelay = 0;
            }
        }
        else//타깃이 공격 범위보다 멀때
        {
            Debug.Log("적 타깃으로 이동 중");
            mIsMoving = true;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    /// <summary>
    /// 타깃이 없을 때 -> 백터로 이동 중
    /// </summary>
    private void NonTargetMove()
    {
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        Debug.Log("남은 거리: " + dist);
        mIsMoving = true;
        if (dist <= mUnitScriptable.attackRange) // 목적지가 공격 사거리 안 일때
        {
            FindenemyOrNull();// 새로운 타깃 탐색 -> 실패시 계속 백터 포지션으로 이동
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
        else
        {
            Debug.Log("백터로 이동 중");
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    private void FindenemyOrNull() // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
    {
        Debug.Log("새로운 공격대상 발견(조건부)");
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (transform.position - key.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key;
            }
        }
        mTarget = target;
        if (minDis <= mUnitScriptable.attackRange) // 공격대상 존재
        {
            Debug.Log("새로운 공격대상 발견");
            mTarget = target;
        }
    }

    public override void GetDamage(int damage)
    {
        if (damage <= 0) return;
        if (Hp <= damage) Die();
        else HPbar.value = (Hp -= damage);
    }

    public void Die()
    {
        HPbar.value = 0;
        mIsAlive = false;
        mIsBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
}
