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
    [SerializeField] private int level;         //현재 레벨

    //피
    [SerializeField] private int maxHP;         // 최대 체력
    private int currentHp;     // 현재 체력

    //스탯
    [SerializeField] private int def;           // 방어력
    [SerializeField] private int mp;            // 마나
    [SerializeField] private int str;           // 공격력
    [SerializeField] private float speed;         // 이동속도

    [Header("Additional stats")]
    [SerializeField] private float criticalRate;    // 크리티컬율
    [SerializeField] private float criticalDamage;  // 크리티컬 데미지
    [SerializeField] private float attackRange = 0.8f; // 공격 사거리
    [SerializeField] private float attackSpeed = 1.5f; // 공격 속도

    [SerializeField] private EElement element;  // 속성



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
        //InitBatch(photonView.IsMine);
    }

    public void Start()
    {
        mIsAlive = true;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;


        attackDelay += Time.deltaTime;
        if (mTarget != null) // 타깃이 있을 때
        {
            if (Vector3.Distance(mVectorDestination, transform.position) <= attackRange) // 타깃이 공격 사정 범위로 들어왔을때
            {
                mNavMeshAgent.avoidancePriority = mPriority;
                mIsmoving = false;
                mNavMeshAgent.isStopped = false;
            }
            else//타깃이 공격 범위보다 멀때
            {
                mIsmoving = true;
                mNavMeshAgent.SetDestination(mTarget.transform.position);
            }
            m_UnitAnimationController.PlayMoveAnimation(mNavMeshAgent.enabled);
            return;
        }
        else // 타깃이 없을 때 -> 백터로 이동 중
        {
            if (Vector3.Distance(mVectorDestination, transform.position) <= attackRange) // 목적지가 공격 사거리 안 일때
            {
                if (Vector3.Distance(mVectorDestination, transform.position) <= 0.15f)//도착했을 때
                {
                    mNavMeshAgent.stoppingDistance = attackRange;
                    Findenemy();
                    m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
                    return;
                }
                FindenemyOrNull(mVectorDestination, attackRange);// 새로운 타깃 탐색 -> 실패시 계속 백터 포지션으로 이동
                m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
            }
        }
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        defaultStoppingDistance = mNavMeshAgent.stoppingDistance;
        mIsBatch = true;
        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void Findenemy() // 반드시 적을 찾는다.thr
    {
        Debug.Log("새로운 공격대상 발견(무조건)");
        float minDis = float.MaxValue;
        Damageable target = NetworkUnitManager.enemyUnitList[0]; // 디폴트 적 넥서스
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
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.SetDestination(mTarget.transform.position);
    }

    private void FindenemyOrNull(Vector3 destination, float attackrange) // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
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
        if (minDis <= attackrange) // 공격대상 존재
        {
            Debug.Log("새로운 공격대상 발견");
            mTarget = target;
            mNavMeshAgent.enabled = true;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    public override void Hit(int damage)
    {
        //animator.SetBool("hit",true);
        HPbar.value = (currentHp -= damage);
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
        mVectorDestination = pos;
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(pos);
    }
}
