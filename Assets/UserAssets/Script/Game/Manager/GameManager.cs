using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool managerMode; //테스트용 모드 (돈 무한 등등...)
    [SerializeField] private Nexus[] damageable;
    [SerializeField] private GameObject[] mCamera;
    [SerializeField] private LayerMask mHostLayer;
    [SerializeField] private LayerMask mClientLayer;

    public static int MyUnitLayer;
    public static int EnemyUnitLayer;
    public static readonly int HOST_NUMBER = 0;
    public static readonly int CLIENT_NUMBER = 1;

    public Nexus GetNexus(int i) => damageable[i];

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
        if (CurrentGold >= gold)
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

    private void Awake()
    {
        InitCamera();
        InitLayer();

        MaxGold = 100;
        IncreseGoldTime = 0.25f;
    }

    private void InitCamera()
    {
        mCamera[0].SetActive(PhotonNetwork.IsMasterClient);
        mCamera[1].SetActive(!PhotonNetwork.IsMasterClient);
    }

    private void InitLayer()
    {
        //MyUnitLayer = PhotonNetwork.IsMasterClient ? mHostLayer : mClientLayer;
        //EnemyUnitLayer = PhotonNetwork.IsMasterClient ? mClientLayer : mHostLayer;

        MyUnitLayer = 16;
        EnemyUnitLayer = 17;

        Debug.Log(MyUnitLayer);
        Debug.Log(EnemyUnitLayer);
    }

    private void FixedUpdate()
    {
        CoolTime += Time.deltaTime;
        CurrentTime += Time.deltaTime;
        CurrentTimeSecond = (int)CurrentTime;

        #region Gold
        if (managerMode) CurrentGold = MaxGold;
        if (CurrentGold < MaxGold)
        {
            if (CoolTime > IncreseGoldTime)
            {
                CurrentGold += 1;
                CoolTime = 0;
            }
        }
        #endregion
        #region Event
        //if(CurrentTime == DragonEventTime)
        //{
        //    if(PhotonNetwork.IsMasterClient)
        //    {
        //        DragonSpawnEvent();
        //    }
        //}
        #endregion
    }

    private void DragonSpawnEvent()
    {
        GameObject obj = null;
        Vector3 spwanPostion = new Vector3(0f, 0f, 0f);
        obj = PhotonNetwork.Instantiate("OfficialUnit/NeutralUnit/" + "RedDragon", spwanPostion, Quaternion.identity);
        //obj.GetComponent<NeutralUnit>().Init();
    }

}
