using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider HPbar;
    [SerializeField] private UnitAnimationController m_UnitAnimationController;

    private Animator mAnimator;
    private Transform mCachedTransfrom;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region Stat Info
    [SerializeField] private int unitId;        //식별번호
    [Header("Stats")]
    [SerializeField] private int level;         //현재 
    private float attackDelay; // 공격 속도 계산용
    #endregion

    #region logic Info
    private float defaultStoppingDistance;
    private bool mIsPointMove;
    private bool mIsCliked;
    private bool mIsAlive;
    private bool mIsBatch;
    private bool mIsMoving;

    private float mPointMoveDist = 0.3f;
    private bool mIsmoving;
    private int mPriority;
    private Vector3 mVectorDestination;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    #endregion

    #region Property
    public bool IsAlive { get => mIsAlive; set => mIsAlive = value; }
    public bool IsClicked { get => mIsCliked; set => mIsCliked = value; }
    public bool IsEnemy { get; private set; }

    #endregion

    protected virtual void Awake()
    {
        mCachedTransfrom = transform;
        m_UnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
        //InitBatch(photonView.IsMine);
    }

    public void Start()
    {
        mName = "유닛";
        mIsAlive = true;
        HPbar.maxValue = HPbar.value = mHp = mMaxHP;
        Debug.Log("사거리는 : " + mAttackRange);
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        attackDelay += Time.deltaTime;

        if (mTarget != null) // 타깃이 있을 때
        {
            if(!mTarget.isAlive()) // 타깃 사망 확인 
            {
                FindenemyOrNull(transform.position);
                return;
            }
            Debug.Log("타깃 타입 : " + mTarget.mName + "남은 거리: " + Vector3.Distance(mTarget.transform.position, transform.position));
            if (Vector3.Distance(mTarget.transform.position, transform.position) <= mAttackRange) // 타깃이 공격 사정 범위로 들어왔을때 -> 정지하고 공격
            {
                mNavMeshAgent.avoidancePriority = mPriority;
                mNavMeshAgent.SetDestination(transform.position);
                if (attackDelay >= mAttackSpeed) 
                {
                    Debug.Log("공격");
                    mAttack.Attack(this, mTarget);
                    attackDelay = 0;
                }
            }
            else//타깃이 공격 범위보다 멀때
            {
                Debug.Log("적 타깃으로 이동 중");
                mIsmoving = true;
                mNavMeshAgent.SetDestination(mTarget.transform.position);
            }
        }
        else // 타깃이 없을 때 -> 백터로 이동 중
        {
            Debug.Log("남은 거리: " + Vector3.Distance(mVectorDestination, transform.position));
            if (Vector3.Distance(mVectorDestination, transform.position) <= mAttackRange) // 목적지가 공격 사거리 안 일때
            {
                FindenemyOrNull(transform.position);// 새로운 타깃 탐색 -> 실패시 계속 백터 포지션으로 이동
                mNavMeshAgent.SetDestination(mVectorDestination);
            }
            else
            {
                Debug.Log("백터로 이동 중");
                mNavMeshAgent.SetDestination(mVectorDestination);
            }
            m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
        }
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = mHp = mMaxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        defaultStoppingDistance = mNavMeshAgent.stoppingDistance;
        mIsBatch = true;
        FindenemyOrNull(transform.position);
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void FindenemyOrNull(Vector3 destination) // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
    {
        Debug.Log("새로운 공격대상 발견(조건부)");
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (destination - key.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key;
            }
        }
        if (minDis <= mAttackRange) // 공격대상 존재
        {
            Debug.Log("새로운 공격대상 발견");
            mTarget = target;
        }
    }

    public override void getDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (mHp <= damage)//사망
        {
            Die();
        }
        else
        {
            HPbar.value = (mHp -= damage);
        }
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

    public void PointMove()
    {
        if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
        {
            mIsPointMove = false;
            mNavMeshAgent.stoppingDistance = defaultStoppingDistance;
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }

}
