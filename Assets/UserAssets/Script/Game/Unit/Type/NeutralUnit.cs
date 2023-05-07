using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
using Scriptable;
/// <summary>
/// 공격 로직
/// 
/// </summary>
public class NeutralUnit : Damageable
{
    private NavMeshAgent mNavMeshAgent;
    private Damageable mTarget;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private string mTargetUUID;

    #region logic Info
    private Vector3 mDestPos;

    private const int mFightPriority = 5;
    private int mPriority;

    private float mInitTime = 3.0f;
    private float mAttackDelay;

    private bool mIsBatch;
    private bool mDoAttackHost;
    private List<string> mRemoveList = new List<string>();

    #endregion

    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPhotonView = GetComponent<PhotonView>();
    }

    #region 초기화
    public void Init(Vector3 spawnPos)
    {
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();
        mIsBatch = true;
        mCurrentHp = mUnitScriptable.maxHP;
        gameObject.layer = GameManager.mMyUnitLayer;
        mAttack = GetComponent<Attackable>();
        mDestPos = spawnPos;
    }

    private void Start()
    {
        StartCoroutine(LandingCoroutine(3));
    }

    [PunRPC]
    public void SyncInit(string UUID) //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(UUID, this);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = UUID;
        IsAlive = true;
    }
    #endregion

    public void Update()
    {
        if (!mPhotonView.IsMine) return;
        if (!IsAlive) return;
        //if (!IsAlive)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, mDestPos, Time.deltaTime * mInitTime);
        //    if (transform.position == mDestPos)
        //    {
        //        IsAlive = true; // 이 시점이 땅에 도착한 시점
        //        mNavMeshAgent.enabled = true;
        //        NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        //        mPhotonView.RPC(nameof(SyncInit), RpcTarget.Others, mUnitScriptable.UUID);
        //        Findenemy();
        //        mPriority = mNavMeshAgent.avoidancePriority;
        //    }
        //    return;
        //}

        mAttackDelay += Time.deltaTime;

        //attack
        {
            if (mTarget == null || !mTarget.IsAlive || !NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID)) // 타깃 사망 확인 
            {
                Debug.Log("타깃 사망및 타깃 재 탐색");
                Findenemy();
                return;
            }
            TargetMove();
        }
    }

    private IEnumerator LandingCoroutine(float duration)
    {
        Vector3 startPos = transform.position;
        float currentTime = 0;
        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, mDestPos, currentTime / duration);
            yield return null;
        }

        IsAlive = true; // 이 시점이 땅에 도착한 시점
        mNavMeshAgent.enabled = true;
        NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        mPhotonView.RPC(nameof(SyncInit), RpcTarget.Others, mUnitScriptable.UUID);
        mPriority = mNavMeshAgent.avoidancePriority;
        Findenemy();
    }

    #region DIE
    private void Die()
    {
        //DieAnimation();

        IsAlive = false;
        mIsBatch = false;
        Debug.Log("드래곤 사망");
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject);
    }

    [PunRPC]
    public void DieRPC()
    {
        Debug.Log("드래곤 사망");
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject);
    }
    #endregion

    #region DAMAGE
    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        Debug.Log("드래곤 공격당함 공격 유닛 id: " + attackUnit);
        if (damage <= 0) return;

        if (mCurrentHp <= damage)
        {
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            Die();
            return;
        }
        else mCurrentHp -= damage;

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

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }
    #endregion

    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        //Debug.Log("타깃 타입 : " + mTarget.mUnitScriptable.unitName + "남은 거리: " + dist);
        if (dist <= mUnitScriptable.attackRange) // 타깃이 공격 사정 범위로 들어왔을때 -> 정지하고 공격
        {
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
            //IdleAnimation();
        }
        else//타깃이 공격 범위보다 멀때
        {
            Debug.Log("적 타깃으로 이동 중");
            //WalkAnimation();
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private void Findenemy() // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
    {
        Debug.Log("드래곤 적 탐색중..");
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

    public override string getUUID()
    {
        return mUnitScriptable.UUID;
    }
}
