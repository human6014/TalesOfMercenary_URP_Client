using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ReceiveEvent : MonoBehaviour, IOnEventCallback
{
    public enum EEventCode : byte
    {
        BuildingUpgrade = 0,
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Debug.Log("메시지 받음 ( " + "이벤트 코드" + eventCode + ") ");
        switch (eventCode)
        {
            case (byte)EEventCode.BuildingUpgrade:
                enemyBuildingUpgrade((object[])photonEvent.CustomData);
                break;
            default:
                //Debug.Assert(false);
                break;
        }
    }

    public void enemyBuildingUpgrade(object[] data)
    {
        int buildingID = (int)data[0];

        for (int i = 0; i < NetworkUnitManager.enemyBuildingList.Count; i++)
        {
            if (NetworkUnitManager.enemyBuildingList[i].CardId == buildingID)
            {
                NetworkUnitManager.enemyBuildingList[i].BuildingUpgarde();
            }
        }
    }
}
