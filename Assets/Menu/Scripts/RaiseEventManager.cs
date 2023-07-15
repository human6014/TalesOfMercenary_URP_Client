using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class RaiseEventManager : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("¸Þ½ÃÁö Àü¼Û");
        //SendMoveUnitsToTargetPositionEvent();
        //GoodBye();
    }
    public enum EventCode : byte
    {
        EsPlayerDrawedCardMessage = 0,
        EsPlayerBuildingUpgradedMessage = 1,
        EsPlayerSpawnedUnitMessage = 2,
        EsPlayerNexusUpgradedMessage = 3,
        EsUnitAttackMessage = 4,
        EsUnitMovementMessage = 5,
        EsUnitDiedMessage = 6,
        EsPlayerUseMagicCardMessage = 7,
        EsUnitTrackMovemntMessage = 8,
        EsGameStartMessage = 9,
        EcPlayerDrawedCardMessage = 10,
        EcNexusUpgradedMessage = 11,
        EcUnitDestinationInputMessage = 12,
        EcUseMagicCardMessage = 13,
        EcBuildingUpgradeMessage = 14,
        EcSpawnUnitMessage = 15
    }

    //private void SendMoveUnitsToTargetPositionEvent()
    //{
    //    object[] content = new object[] { new Vector3(1.0f, 1.0f, 1.0f), 3, 4 }; // Array contains the target position and the IDs of the selected units
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
    //    Debug.Log("Å¸ÀÔ: " + (byte)EventCode.EsPlayerBuildingUpgradedMessage + "ÁÂÇ¥: " + (int)content[0] + "id" + (int)content[1] + "id" + (int)content[2] + "º¸³¿, ÃÑ ±æÀÌ;" + content.Length);
    //    PhotonNetwork.RaiseEvent((byte)1, content, raiseEventOptions, SendOptions.SendReliable);
       
    //}
    //private void GoodBye()
    //{
    //    object[] content = new object[] { 3 }; // Array contains the target position and the IDs of the selected units
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers tSo All in order to receive this event on the local client as well
    //    PhotonNetwork.RaiseEvent((byte)EventCode.EsPlayerSpawnedUnitMessage, content, raiseEventOptions, SendOptions.SendReliable);
        
    //}

    #region Message host -> Client
    public static void SendPlayerDrawedCard(ushort cardID, ushort userID)
    {

        Debug.Log("PlayerDrawedCardMessage¸¦ º¸³¿");
    }//

    public static void SendPlayerBuildingUpgraded(ushort userID, ushort BuildingInfo, ushort BuildingExp, byte BuildingLevel)
    {

        Debug.Log("PlayerBuildingUpgradedMessage¸¦ º¸³¿");
    }

    public static void SendPlayerSpawnedUnit(ushort userID, ushort unitUniqueID, ushort unitInstanceID, Vector3 spawnPos, Vector3 finalDestination)
    {

        Debug.Log("PlayerSpawnedUnitMessage¸¦ º¸³¿");
    }//

    public static void SendPlayerNexusUpgraded(ushort userID)
    {

        Debug.Log("PlayerNexusUpgradedMessage¸¦ º¸³¿");
    }//

    public static void UnitAttack(ushort attackingUnitInstanceID, ushort attackedUnitInstanceID, int damage)
    {

        Debug.Log("UnitAttackMessage¸¦ º¸³¿");
    }

    public static void UnitMovement(ushort unitInstanceID, Vector3 destination)
    {

        Debug.Log("UnitMovementMessage¸¦ º¸³¿");
    }

    public static void UnitDied(ushort unitInstanceID)
    {

        Debug.Log("UnitDiedMessage¸¦ º¸³¿");
    }

    public static void PlayerUseMagicCard(ushort usedMagicID, Vector3 destination)
    {

        Debug.Log("PlayerUseMagicCardMessage¸¦ º¸³¿");
    }

    public static void UnitTrackMovemnt(ushort trackingUnitInstanceID, ushort trackedUnitInstanceID)
    {

        Debug.Log("UnitTrackMovemntMessage¸¦ º¸³¿");
    }

    public static void GameStart(ushort userID)
    {

        Debug.Log("GameStartMessage¸¦ º¸³¿");
    }
    #endregion

    #region  Message Client -> host
    public static void SendPlayerDrawedCard()
    {

    }

    public static void SendcNexusUpgraded()
    {

    }

    public static void SendUnitDestinationInput(ushort unitID, Vector3 destination)
    {
        Debug.Log("PlayerSpawnedUnitMessage¸¦ º¸³¿");
    }

    public static void SendUseMagicCard(ushort cardID, Vector3 destination)
    {
    }

    public static void SendBuildingUpgrade(ushort BuildingInfo)
    {

    }

    public static void SendSpawnUnit(ushort unitID, Vector3 destination)
    {
    }
    #endregion
}