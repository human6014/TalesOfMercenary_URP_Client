using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSender : MonoBehaviour
{
    private UnitJsonData[] usingUnitDatas;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetUsingCardData(UnitJsonData[] _usingUnitData)
    {
        //unitData = new UnitJsonData[unitJsonData.Length];
        usingUnitDatas = _usingUnitData;
    }

    public UnitJsonData[] GetUnitData()
    {
        return usingUnitDatas;
    }
}
