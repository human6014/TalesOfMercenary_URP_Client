using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Scriptable;
using UnityEditor;
/// <summary>
/// target이 바뀔때마다 settarget()함수를 호출해서 모든 유저에게 타깃 변수를 동기화해줘야 한다.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int unitId;        //식별번호

    private UnitAnimationController mUnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region logic Info
    private bool mIsBatch;
    private bool mIsMoving = true;

    private float mAttackDelay; // 공격 속도 계산용
    private int mPriority;
    private const int mFightPriority = 5;

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
        mUnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = Hp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;

        mIsBatch = true;
        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();

        NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUnitScriptable.UUID);

    }

    [PunRPC]
    public void SyncInitBatch(string uuid) //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(uuid, this);
        HPbar.maxValue = HPbar.value = Hp = mUnitScriptable.maxHP;
        mUnitScriptable.UUID = uuid;
        IsAlive = true;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        mAttackDelay += Time.deltaTime;

        if (mTarget != null) // 타깃이 있을 때
        {
            if (!mTarget.IsAlive) // 타깃 사망 확인 
            {
                Debug.Log("타깃 사망및 타깃 재 탐색");
                Findenemy();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
        mUnitAnimationController.PlayMoveAnimation(mIsMoving);
    }

    /// <summary>
    /// 타깃이 있고 살아있을 때
    /// </summary>
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);

        if (dist <= mUnitScriptable.attackRange) // 타깃이 공격 사정 범위로 들어왔을때 -> 정지하고 공격
        {
            Debug.Log("타깃 타입 : " + mTarget.mUnitScriptable.unitName + " 정지 후 공격");
            mIsMoving = false;
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                Debug.Log("공격");
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
        }
        else//타깃이 공격 범위보다 멀때
        {
            Debug.Log("타깃 타입 : " + mTarget.mUnitScriptable.unitName + "남은 거리: " + dist);
            mIsMoving = true;
            mNavMeshAgent.avoidancePriority = mPriority;
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
        if (dist <= mUnitScriptable.movementRange) // 목적지가 공격 사거리 안 일때
        {
            Findenemy();// 새로운 타깃 탐색 -> 실패시 계속 백터 포지션으로 이동
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
        else
        {
            Debug.Log("백터로 이동 중");
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    private void Findenemy()
    {
        Debug.Log("새로운 공격대상 발견");
        Debug.Log(NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        string temUUID = null;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (transform.position - key.Value.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key.Value;
                temUUID = key.Key;
            }
        }
        mTarget = target;
        Debug.Log("새로운 타깃 타입" + mTarget.mUnitScriptable.unitType);
    }

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        if (damage <= 0) return;
        if (Hp <= damage) Die();
        else HPbar.value = (Hp -= damage);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList[attackUnit].IsAlive)
            {
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
            }
        }
    }

    public void Die()
    {
        HPbar.value = 0;
        IsAlive = false;
        mIsBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }


}
