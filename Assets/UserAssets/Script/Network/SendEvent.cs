using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendEvent : MonoBehaviour
{
    public enum EEventCode : byte
    {
        BuildingUpgrade = 0,
    }

    public static void HplayerDrawedCard(int buildingID)
    {
        object[] content = new object[] { (int)buildingID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.BuildingUpgrade, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
