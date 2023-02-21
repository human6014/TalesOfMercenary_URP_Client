using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//통신
//게임 진행
public class GameManager : MonoBehaviour
{
    private const int maxHandSize = 10;
    private const int maxDeckSize = 5;
    private const int initUnitListSize = 15;
    private const int initEventUnitSize = 2;

    [SerializeField] private bool managerMode; //테스트용 모드 (돈 무한 등등...)

    public static Vector3 player1Nexus;
    public static Vector3 player2Nexus;

    private DataSender dataSender;
    private UnitJsonData[] unitData;// = new UnitJsonData[6];

    //0: 나 1: 상대
    private List<User> users;

    #region 손패
    private List<Unit> myHand_Unit;
    private List<Magic> myHand_Magic;
    //무조건 핸드는 2개가 같이 연산되어야 한다.
    #endregion

    #region 덱
    private List<Building> myDeck;
    private List<Building> enemyDeck;
    #endregion

    #region 필드 위 유닛
    private List<Unit> spawndedMyUnit;
    private List<Unit> spawndedEnemyUnit;
    private List<Unit> spawndedEventUnit;
    #endregion

    #region 시간
    //float 현재 시간
    public float CurrentTime { get; private set; }
    //int 현재 시간
    public int CurrentTimeSecond { get; private set; }
    //둘 중 뭐 쓸지 몰것다
    #endregion

    #region 돈관련
    //변수 위치 이상해

    //현재 최대 골드
    public int MaxGold { get; set; }

    //현재 골드
    public int CurrentGold { get; set; }

    //현재 골드 수급 시간
    public float IncreseGoldTime { get; set; } // IncreseGoldTime초당 1원 오름

    //골드 수급 시간 계산용
    private float CoolTime { get; set; }
    #endregion

    /// <summary>
    /// CurrentGold가 값보다 같거나 클 경우 CurrentGold에서 값을 빼고 true, 아닐경우 false
    /// </summary>
    /// <param name="gold">소모할 비용</param>
    /// <returns>return CurrentGold >= gold</returns>
    public bool DoValidGold(int gold)
    {
        if(CurrentGold >= gold)
        {
            CurrentGold -= gold;
            return true;
        }
        return false;
    }
    
    public void UpgradeNexus(float decreseTime = 0.05f)
    {
        IncreseGoldTime -= decreseTime;
    }

    public GameManager(User my, User enemy)
    {
        users = new List<User>() { my, enemy };

        myHand_Unit = new List<Unit>(maxHandSize);
        myHand_Magic = new List<Magic>(maxHandSize);

        myDeck = new List<Building>(maxDeckSize);
        enemyDeck = new List<Building>(maxDeckSize);

        spawndedMyUnit = new List<Unit>(initUnitListSize);
        spawndedEnemyUnit = new List<Unit>(initUnitListSize);
        spawndedEventUnit = new List<Unit>(initEventUnitSize);
    }

    private void Awake()
    {
        InitData();

        player1Nexus = GameObject.Find("MyNexus").transform.position;
        player2Nexus = GameObject.Find("EnemyNexus").transform.position;

        MaxGold = 100;
        IncreseGoldTime = 0.25f;
    }

    private void InitData()
    {
        dataSender = FindObjectOfType<DataSender>();
        if (dataSender == null) return;

        unitData = dataSender.GetUnitData();

        Destroy(dataSender.gameObject);
    }

    public UnitJsonData[] GetDeckInfo() => unitData;

    private void FixedUpdate()
    {
        CoolTime += Time.deltaTime;
        CurrentTime += Time.deltaTime;
        CurrentTimeSecond = (int)CurrentTime;

        if (managerMode) CurrentGold = MaxGold;
        if (CurrentGold < MaxGold)
        {
            if (CoolTime > IncreseGoldTime)
            {
                CurrentGold += 1;
                CoolTime = 0;
            }
        }
    }


}
