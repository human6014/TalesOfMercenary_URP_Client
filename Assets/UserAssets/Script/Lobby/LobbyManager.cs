using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private DataSender dataSender;
    [SerializeField] private RectTransform allCard;

    //private BanPickManager banPickManager;
    public BuildingCard[] deckCards { get; private set; } = new BuildingCard[4];
    private UnitJsonData[] allUnitData;
    private UnitJsonData[] usingUnitData;


    //로비로 돌아올때마다 호출됨
    //-> static이나 bool변수로 데이터 로드 1번만 되도록 제어 필요
    private void Awake()
    {
        //banPickManager = GetComponent<BanPickManager>();
        //deckCards = allCard.GetComponentsInChildren<DeckCard>();
        allUnitData = new UnitJsonData[deckCards.Length];
        InitDataSetting();
    }

    /// <summary>
    /// 모든 카드 데이터 로드
    /// </summary>
    private void InitDataSetting()
    {
        allUnitData[0] = JsonManager.LoadJsonFile<UnitJsonData>("0_Warrior_Data");
        allUnitData[1] = JsonManager.LoadJsonFile<UnitJsonData>("1_Wizard_Data");
        allUnitData[2] = JsonManager.LoadJsonFile<UnitJsonData>("2_ShieldMan_Data");
        allUnitData[3] = JsonManager.LoadJsonFile<UnitJsonData>("3_Archer_Data");
        //allUnitData[0].Print();
    }

    /// <summary>
    /// 밴픽 끝난 후 게임 시작
    /// </summary>
    public void MoveGameScene()
    {
        //int[] dataNum = banPickManager.GetUsingCardNum();
        //usingUnitData = new UnitJsonData[dataNum.Length];
        //for (int i = 0; i < dataNum.Length; i++) usingUnitData[i] = allUnitData[dataNum[i]];
        
        dataSender.SetUsingCardData(usingUnitData);
        SceneManager.LoadScene("GameScene");
    }

    public void MoveLoginScene()
    {
        //로그아웃 작업

        SceneManager.LoadScene("LoginScene");
    }
}
