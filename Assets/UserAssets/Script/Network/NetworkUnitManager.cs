
using Photon.Pun;
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
    /// 1: Àû
    /// 0: ³ª
    /// </summary>
    [SerializeField] private Nexus[] damageable;
    public static Damageable[] usingUnit = new Damageable[4];
    public static List<Damageable> enemyUnitList { get; } = new();
    public static List<Damageable> myUnitList { get; } = new();
    public static List<BuildingCard> mybuildingList { get; } = new();
    public static List<BuildingCard> enemyBuildingList { get; } = new();

    void Awake()
    {
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();

        if (PhotonNetwork.IsMasterClient)
        {
            enemyUnitList.Add(damageable[1]);
            myUnitList.Add(damageable[0]);
        }
        else
        {
            enemyUnitList.Add(damageable[0]);
            myUnitList.Add(damageable[1]);
        }
    }
}