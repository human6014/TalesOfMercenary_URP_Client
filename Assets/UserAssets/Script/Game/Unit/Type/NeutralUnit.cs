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
    [SerializeField] private UnitUIController mUnitUIController;

    private UnitAnimationController mUnitAnimationController;
    private NavMeshAgent mNavMeshAgent;
    private Damageable mTarget;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private List<string> mRemoveList = new List<string>();
    private EElement mtargetElement;
    private Vector3 mDestPos;

    private string mTargetUUID;
    private string mUUID;

    private int mPriority;

    private float mInitTime = 3.0f;
    private float mAttackDelay;

    private bool mIsBatch;
    private bool mDoAttackHost;

    private const int mFightPriority = 5;
    #region Animation sting
    private const string IdleState = "IsIdle";
    private const string MoveState = "IsMove";
    private const string NormalAttackState = "NormalAttack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "Die";
    #endregion
    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPhotonView = GetComponent<PhotonView>();
    }

    #region 초기화
    public void Init(Vector3 spawnPos)
    {
        mIsBatch = true;
        mCurrentHp = mUnitScriptable.maxHP;
        gameObject.layer = GameManager.mMyUnitLayer;
        mAttack = GetComponent<Attackable>();
        mDestPos = spawnPos;
        mUnitUIController.Init(mCurrentHp, false);
    }

    private void Start()
    {
        if (mPhotonView.IsMine) StartCoroutine(LandingCoroutine(3));
    }

    [PunRPC]
    public void SyncInit(string UUID) //적이 소환한 유닛 초기화
    {
        mCurrentHp = mUnitScriptable.maxHP;
        mUnitUIController.Init(mCurrentHp, mPhotonView.IsMine);
        NetworkUnitManager.AddEnemyUnit(UUID, this);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUUID = UUID;
        IsAlive = true;
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
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, IdleState, true);

        yield return new WaitForSeconds(0.3f);

        IsAlive = true; // 이 시점이 땅에 도착한 시점
        mUUID = MyUUIDGeneration.GenrateUUID();
        mNavMeshAgent.enabled = true;
        NetworkUnitManager.AddmyUnit(mUUID, this);
        mUnitUIController.Init(mUnitScriptable.maxHP, false);
        mPhotonView.RPC(nameof(SyncInit), RpcTarget.Others, mUUID);
        mPriority = mNavMeshAgent.avoidancePriority;
        Findenemy();
    }
    #endregion

    public void Update()
    {
        if (!mPhotonView.IsMine) return;
        if (!IsAlive) return;

        mAttackDelay += Time.deltaTime;

        if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID))
        {
            mTarget = null;
            mTargetUUID = "";
        }

        //attack
        if (mTarget != null) // 타깃이 있을 때
        {

            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // 타깃 사망 확인 
            {
                Findenemy();
                return;
            }
            TargetMove();
        }
        Findenemy();
    }
    #region DIE
    private void Die(string unit)
    {
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, DieState);
        IsAlive = false;
        mIsBatch = false;
        Debug.Log("드래곤 사망");
        NetworkUnitManager.myUnitList.Remove(unit);
        Destroy(gameObject, 3);
    }

    [PunRPC]
    public void DieRPC(string unit)
    {
        Debug.Log("드래곤 사망");
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(unit);
        Destroy(gameObject, 3);
    }
    #endregion

    #region DAMAGE
    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit, string attackedUnitUUID)
    {
        Debug.Log("드래곤 공격당함 공격 유닛 id: " + attackUnit);
        if (damage <= 0) return;

        if (mCurrentHp <= damage)
        {
            Die(mUUID);
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others, mUUID);
            return;
        }
        else mCurrentHp -= damage;

        mUnitUIController.GetDamage(mCurrentHp);

        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                transform.LookAt(NetworkUnitManager.enemyUnitList[attackUnit].transform.position);
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                mTargetUUID = mTarget.getUUID();
            }
        }
    }

    public override void GetDamage(int damage, string attackUnitUUID, string attackedUnitUUID)
    {
        if (mCurrentHp <= damage) mCurrentHp = 0;
        else mCurrentHp -= damage;

        mUnitUIController.GetDamage(mCurrentHp);
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }
    #endregion

    private void TargetMove()
    {
        transform.LookAt(mTarget.transform.position);
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
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
            //IdleAnimation();
        }
        else//타깃이 공격 범위보다 멀때
        {
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
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

    private void Findenemy() // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
    {
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
        mTarget = target;
        mTargetUUID = temUUID;
    }

    public override string getUUID()
    {
        return mUUID;
    }
}
