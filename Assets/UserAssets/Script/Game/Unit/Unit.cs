using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider HPbar;

    private Animator animator;
    private Transform cachedTransfrom;
    private NavMeshAgent navMeshAgent;
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
    [SerializeField] private float attackRange = 0.5f; // 공격 사거리
    [SerializeField] private float detectRange = 2.5f; // 상대 유닛 감지 거리
    [SerializeField] private float attackSpeed = 1.5f; // 공격 속도
    [SerializeField] private float attackDelay; // 공격 속도 계산용

    //속성
    [SerializeField] private EElement element;  // 속성
    #endregion

    #region logic Info
    private float defaultStoppingDistance;

    private bool isPlayer = false; // 임시용
    private bool isDetectEnemy;
    private bool isPointMove;
    private bool isCliked;
    private bool isAlive;
    private bool isBatch;
    
    private Vector3 finalDestination;
    private Vector3 currentDestination;
    #endregion

    #region Property
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool IsClicked { get => isCliked; set => isCliked = value; }
    public bool IsEnemy { get; private set; }
    public ushort OwnerID { get; set; }
    public ushort InstanceID { get; private set; }
    
    #endregion

    private void Awake()
    {
        cachedTransfrom = transform;
    }

    public void Start()
    {
        isAlive = true;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;
    }

    private void FixedUpdate()
    {
        
        if (!isBatch) return;
        attackDelay += Time.deltaTime;
        /*
        if (isPointMove) PointMove();
        else
        {
            if (isDetectEnemy = DetectEnemy()) DetectAttack();
            DefaultMove();
        }
        */
    }

    public void InitBatch(ushort ownerID, ushort instanceID, Vector3 finalDestination)
    {
        //Debug.Log("유닛 배치 완료");
        navMeshAgent = GetComponent<NavMeshAgent>();

        HPbar.maxValue = HPbar.value = currentHp = maxHP;
        //실험용 적 때문에 Start함수와 중복되는 부분 있음

        OwnerID = ownerID;
        InstanceID = instanceID;
        this.finalDestination = finalDestination;

        isBatch = true;
        isAlive = true;
        navMeshAgent.enabled = true;
        
        defaultStoppingDistance = navMeshAgent.stoppingDistance;

        SetAffiliationUnit();
        currentDestination = finalDestination;

        SetDestination(finalDestination);
        //Find 함수 별로 안좋음
    }

    private void SetAffiliationUnit()
    {
        if (OwnerID == NetworkManager.NetworkManagerSingleton.Client.Id)
        {
            HPbar.targetGraphic.color = Color.green;
            IsEnemy = false;
            gameObject.layer = 16;
        }
        else
        {
            HPbar.targetGraphic.color = Color.red;
            IsEnemy = true;
            gameObject.layer = 17;
        }

        if (NetworkManager.NetworkManagerSingleton.IsReversed) finalDestination = GameManager.player1Nexus;
        else finalDestination = GameManager.player2Nexus;
    }

    #region 공격 + 탐지 // 코드 개선 필요함
    private void Attack(Unit enemy)
    {
        //Debug.Log(transform.name + "Attack");
        attackDelay = 0;
        //animator.SetBool("attack",true);
        enemy.GetDamage(str);
    }

    private void Attack(Nexus nexus)
    {
        attackDelay = 0;
        nexus.Hit(str);
    }

    private void Attack(NeutralUnit neutralUnit)
    {
        attackDelay = 0;
        neutralUnit.Hit(str);
    }
    #endregion

    public void GetDamage(int damage)
    {
        //animator.SetBool("hit",true);
        HPbar.value = (currentHp -= damage);
    }

    public void Die()
    {
        HPbar.value = 0;
        isAlive = false;
        isBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    private void DefaultMove()
    {
        navMeshAgent.SetDestination(currentDestination);
        if (!isDetectEnemy) currentDestination = finalDestination;
    }

    public void PointMove()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isPointMove = false;
            navMeshAgent.stoppingDistance = defaultStoppingDistance;
        }
    }

    public void PointMove(Vector3 pos)
    {
        //isPointMove = true;
        //navMeshAgent.stoppingDistance = 0;
        //navMeshAgent.SetDestination(pos);

        NetworkUnitManager.SendDestinationInput(InstanceID, pos);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(transform.position, attackRange);
    }
#endif
}
