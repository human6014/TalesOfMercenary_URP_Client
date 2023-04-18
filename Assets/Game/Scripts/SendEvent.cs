using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendEvent : MonoBehaviour
{
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

    private void SendMoveUnitsToTargetPositionEvent()
    {
        Debug.Log("∏ﬁΩ√¡ˆ 1π¯ ∫∏≥ø");
        object[] content = new object[] { new Vector3(1.0f, 1.0f, 1.0f), (byte)3, (byte)4 }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        Debug.Log("≈∏¿‘: " + (byte)EventCode.EsPlayerSpawnedUnitMessage + "¡¬«•: " + (Vector3)content[0] + "id" + (byte)content[1] + "id" + (byte)content[2]  + "∫∏≥ø, √— ±Ê¿Ã;" + content.Length);
        PhotonNetwork.RaiseEvent((byte)EventCode.EsPlayerSpawnedUnitMessage, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private void GoodBye()
    {
        Debug.Log("∏ﬁΩ√¡ˆ 2π¯ ∫∏≥ø");
        object[] content = new object[] { (byte)3 }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers tSo All in order to receive this event on the local client as well
        Debug.Log("≈∏¿‘: " + (byte)EventCode.EsPlayerNexusUpgradedMessage +  "id" + (byte)content[0] + "∫∏≥ø, √— ±Ê¿Ã;" + content.Length);
        PhotonNetwork.RaiseEvent((byte)EventCode.EsPlayerNexusUpgradedMessage, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private void Start()
    {
        SendMoveUnitsToTargetPositionEvent();
        GoodBye();
    }
}
