using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// target이 바뀔때마다 settarget()함수를 호출해서 모든 유저에게 타깃 변수를 동기화해줘야 한다.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private UnitUIController mUnitUIController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private string mTargetUUID;
    [SerializeField] private float dis;
    [SerializeField] private bool IsLIVE;
    [SerializeField] private int hp;
    [SerializeField] private string mUUID;

    private EElement mtargetElement;
    private UnitAnimationController mUnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private List<string> mRemoveList = new List<string>();

    #endregion

    #region logic Info
    private bool mIsBatch;

    private float mAttackDelay; // 공격 속도 계산용
    private int mPriority;
    private const int mFightPriority = 5;

    private Vector3 mVectorDestination;
    #endregion

    #region Property
    public bool IsClicked { get; set; }
    public bool IsEnemy { get; private set; }

    public override string getUUID()
    {
        return mUUID;
    }
    #endregion

    #region Animation string
    private const string MoveState = "IsMove";
    private const string NormalAttackState = "NormalAttack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "IsDie";
    private const string HitState = "Hit";
    #endregion

    #region Init & Awake
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
        //mNavMeshAgent.speed = mUnitScriptable.speed;
        mCurrentHp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;
        gameObject.layer = GameManager.mMyUnitLayer;

        mIsBatch = true;
        mUnitUIController.Init(mUnitScriptable.maxHP, true);

        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);
        mUUID = MyUUIDGeneration.GenrateUUID();
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUUID);
        {
            hp = mUnitScriptable.maxHP;
            NetworkUnitManager.AddmyUnit(mUUID, this);
            IsLIVE = true;
        }
    }

    [PunRPC]
    public void SyncInitBatch(string uuid) //적이 소환한 유닛 초기화
    {
        {
            NetworkUnitManager.AddEnemyUnit(uuid, this);
            hp = mCurrentHp;
            IsLIVE = true;
        }
        mCurrentHp = mUnitScriptable.maxHP;
        mUnitUIController.Init(mUnitScriptable.maxHP, mPhotonView.IsMine);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUUID = uuid;
        IsAlive = true;
    }
    #endregion

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;
        mAttackDelay += Time.deltaTime;

        if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID))
        {
            mTarget = null;
            mTargetUUID = "";
        }

        if (mTarget != null) // 타깃이 있을 때
        {

            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // 타깃 사망 확인 
            {
                Debug.Log("타깃 사망및 타깃 재 탐색   :   " + mTarget != null);
                Findenemy();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
    }
    private void Findenemy()
    {
        Debug.Log("적 탐색...");
        //Debug.Log("적 유닛 갯수 : " + NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
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
                    temUUID = key.Key;
                    mtargetElement = key.Value.mUnitScriptable.element;
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
        SetTarget(temUUID);
        mTargetUUID = temUUID;
        Debug.Log("새로운 타깃 발견 : " + mTarget.mUnitScriptable.UUID + ",     타입 : " + mTarget.mUnitScriptable.unitType);
    }
    private string TypeToString(AttackType attackType)
    {
        string attackAnimName = "";
        switch (attackType)
        {
            case AttackType.Normal:
                attackAnimName = NormalAttackState;
                break;
            case AttackType.Critical:
                attackAnimName = NormalAttackState;
                break;
            case AttackType.Skill:
                attackAnimName = SkillAttackState;
                break;
        }
        return attackAnimName;
    }

    void SetTarget(string uuid)
    {
        mTarget = NetworkUnitManager.enemyUnitList[uuid];
    }

    #region move
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        dis = dist;
        transform.LookAt(mTarget.transform.position);
        if (dist <= mUnitScriptable.attackRange) // 타깃이 공격 사정 범위로 들어왔을때 -> 정지하고 공격
        {
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);

            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                AttackType attackType = mAttack.Attack(getUUID(), mUnitScriptable.element, mTargetUUID, mtargetElement);
                mAttackDelay = 0;
                mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, TypeToString(attackType));
            }
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, false);
        }
        else//타깃이 공격 범위보다 멀때
        {
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private void NonTargetMove()
    {
        if (mTargetUUID == "")
        {
            Findenemy();
            return;
        }
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        dis = dist;
        if (dist <= mUnitScriptable.movementRange) // 목적지가 공격 사거리 안 일때
        {
            Findenemy();
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
        else
        {
            //WalkAnimation();
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mTargetUUID = "Vector";
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
    #endregion


    #region Damage
    public override void GetDamage(int damage, string attackUnitUUID, string attackedUnitUUID) //적의 클라에서 호출
    {
        if (mCurrentHp <= damage) mCurrentHp = 0;
        else mCurrentHp -= damage;
        {
            hp = mCurrentHp;
        }
        mUnitUIController.GetDamage(mCurrentHp);
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID, attackedUnitUUID);
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit, string attackedUnitUUID) // 자신의 클라에서 호출
    {
        if (mCurrentHp <= damage)
        {
            Die(mUUID);
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others, mUUID);
            return;
        }
        else mCurrentHp -= damage;
        {
            hp = mCurrentHp;
        }
        mUnitUIController.GetDamage(mCurrentHp);

        Debug.Log(mTarget.mUnitScriptable.unitType);

        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                Debug.Log("넥서스 공격 중 피격");
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                transform.LookAt(NetworkUnitManager.enemyUnitList[attackUnit].transform.position);
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                mTargetUUID = attackUnit;
            }
        }
    }
    #endregion

    #region DIE
    public void Die(string unit)//자신의 클라에서 호출
    {

        Debug.Log("Die() " + this.mUUID);
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveMyUnit(unit);
            mNavMeshAgent.enabled = false;
            IsAlive = false;
            mIsBatch = false;
        }
        //DieAnimation();
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, DieState, true);

        Destroy(gameObject, 3f);
    }

    [PunRPC]
    public void DieRPC(string unit)//적의 클라에서 호출
    {
        Debug.Log("DieRPC() " + this.mUUID);
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveEnemyUnit(unit);
            //mNavMeshAgent.enabled = false;
            IsAlive = false;
            mIsBatch = false;
            Destroy(gameObject, 3f);
        }
    }
    #endregion
}
