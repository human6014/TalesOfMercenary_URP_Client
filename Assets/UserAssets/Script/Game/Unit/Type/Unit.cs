using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Scriptable;
using UnityEditor;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
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
    private string mTargetUUID;
    private List<string> mRemoveList = new List<string>();
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
        mCurrentHp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;
        gameObject.layer = GameManager.mMyUnitLayer;

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
        mCurrentHp = mUnitScriptable.maxHP;
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = uuid;
        IsAlive = true;
    }

    public override string getUUID()
    {
        return mUnitScriptable.UUID;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;
        if (IsAlive == false)
        {
            Die();
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            return;
        }
        mAttackDelay += Time.deltaTime;

        if (mTarget != null) // 타깃이 있을 때
        {
            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // 타깃 사망 확인 
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

    #region move
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
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
        }
        else//타깃이 공격 범위보다 멀때
        {
            //Debug.Log("타깃 타입 : " + mTarget.mUnitScriptable.unitName + "남은 거리: " + dist);
            mIsMoving = true;
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private void NonTargetMove()
    {
        if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID)) // 타깃이 죽었을때
        {
            mTarget = null;
            Findenemy();
            return;
        }
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        //Debug.Log("남은 거리: " + dist);
        mIsMoving = true;
        if (dist <= mUnitScriptable.movementRange) // 목적지가 공격 사거리 안 일때
        {
            Findenemy();
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

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
    #endregion

    private void Findenemy()
    {
        Debug.Log("적 탐색...");
        Debug.Log("적 유닛 갯수 : " + NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        string temUUID = null;
        mRemoveList.Clear();

        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                tem = (transform.position - key.Value.transform.position).sqrMagnitude;
                if (minDis > tem)
                {
                    minDis = tem;
                    target = key.Value;
                    temUUID = key.Key;
                }
            }
            else
            {
                mRemoveList.Add(key.Key);
                //NetworkUnitManager.enemyUnitList.Remove(key.Key);
            }
        }
        for (int i = 0; i < mRemoveList.Count; i++)
        {
            NetworkUnitManager.enemyUnitList.Remove(mRemoveList[i]);
        }
        mTarget = target;
        mTargetUUID = temUUID;
        Debug.Log("새로운 타깃 타입" + mTarget.mUnitScriptable.unitType);
    }

    #region Damage
    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        if (damage <= 0) return;

        if (mCurrentHp <= damage)
        {
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            Die();
            return;
        }
        else mCurrentHp -= damage;
        //else HPbar.value = (mCurrentHp -= damage);

        Debug.Log("받은 피해 : " + damage + ",  현재 체력 : " + mCurrentHp);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (NetworkUnitManager.enemyUnitList[attackUnit].IsAlive)
                {
                    mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                }
            }

        }
    }
    #endregion

    #region DIE
    public void Die()
    {
        int i;
        i = NetworkUnitManager.myUnitList.Count;
        Debug.Log("삭제 전");
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                Debug.Log("삭제 전 살아있는 유닛: " + key.Value.mUnitScriptable.unitType);
            }
            else
            {
                Debug.Log("삭제 전 죽은 유닛: " + key.Value.mUnitScriptable.unitType);
            }
        }
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        Debug.Log("삭제 후");
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                Debug.Log("삭제 후 살아있는 유닛: " + key.Value.mUnitScriptable.unitType);
            }
            else
            {
                Debug.Log("삭제 후 죽은 유닛: " + key.Value.mUnitScriptable.unitType);
            }
        }
        Debug.Log("유닛 삭제 -> (삭제 전 myUnitList 갯수 : " + i + "삭제 후 :" + NetworkUnitManager.myUnitList.Count + ")");
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        IsAlive = false;
        mIsBatch = false;
        Destroy(gameObject);
    }

    [PunRPC]
    public void DieRPC()
    {
        int i;
        i = NetworkUnitManager.enemyUnitList.Count;
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        Debug.Log("유닛 삭제 -> (삭제 전 enemyUnitList 갯수 : " + i + "삭제 후 :" + NetworkUnitManager.enemyUnitList.Count + ")");
        Destroy(gameObject);
    }
    #endregion
}
