using Photon.Pun;
using Scriptable;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkUnitManager : MonoBehaviour
{
    /// <summary>
    /// 1: 적
    /// 0: 나
    /// </summary>
    private GameManager mGameManger; 
    public static Dictionary<string, Damageable> enemyUnitList { get; } = new();
    public static Dictionary<string, Damageable> myUnitList { get; } = new();
    public static List<BuildingCard> enemyBuildingList { get; } = new();



    private void Init()
    {
        enemyUnitList.Clear();
        myUnitList.Clear();
        enemyBuildingList.Clear();
    }

    private void Awake()
    {
        Init();

        mGameManger = GetComponent<GameManager>();

        if (PhotonNetwork.IsMasterClient)
        {
            enemyUnitList.Add("1", mGameManger.GetNexus(1));
            myUnitList.Add("1", mGameManger.GetNexus(0));
        }
        else
        {
            enemyUnitList.Add("1", mGameManger.GetNexus(0));
            myUnitList.Add("1", mGameManger.GetNexus(1));
        }
    }

    public static void AddmyUnit(string uuid, Damageable unit)
    {
        myUnitList.Add(uuid, unit);
        Debug.Log(uuid + " : 아군 유닛 추가");
    }

    public static void RemoveMyUnit(string uuid)
    {
        bool success;
        success = myUnitList.Remove(uuid);
        Debug.Log(uuid + " : 아군 유닛 삭제 ->" + success + "남은 유닛 갯수" + myUnitList.Count);
        foreach (KeyValuePair<string, Damageable> kv in myUnitList)
        {
            Debug.Log(kv.Key + "남은 아군");
        }
    }

    public static void AddEnemyUnit(string uuid, Damageable unit)
    {
        enemyUnitList.Add(uuid, unit);
        Debug.Log(uuid + " : 적 유닛 추가");
    }

    public static void RemoveEnemyUnit(string uuid)
    {
        bool success;
        success = enemyUnitList.Remove(uuid);
        Debug.Log(uuid + " : 적 유닛 삭제 -> " + success + "남은 유닛 갯수" + enemyUnitList.Count);
        foreach (KeyValuePair<string, Damageable> kv in enemyUnitList)
        {
            Debug.Log(kv.Key + "남은 적군");
        }
    }
}