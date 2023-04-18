using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Receive : MonoBehaviour, IOnEventCallback
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
    private void OnEnable()
    {
        Debug.Log("콜백 등록");
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //Debug.Log(RoomManager.Instance.m_PhotonView.IsMine + "@@@@@@@@@@@@@@@@@@@@@@@@");
        Debug.Log("메시지 받음");
        Debug.Log("이벤트 코드" + eventCode);
        object[] data = (object[])photonEvent.CustomData;
        switch(eventCode)
        {
            case (byte)EventCode.EsPlayerSpawnedUnitMessage:
                EsPlayerSpawnedUnitMessage(data);
                break;

            case (byte)EventCode.EsPlayerNexusUpgradedMessage:
                EsPlayerNexusUpgradedMessage(data);
                break;
        }
    }
    public void EsPlayerSpawnedUnitMessage(object[] data)
    {
        Vector3 targetPosition = (Vector3)data[0];
        byte id1 = (byte)data[1];
        byte id2 = (byte)data[2];
        Debug.Log(id2 + "attack" + id1 + "move point -> " + targetPosition + "받았다!!!!!!!!!");
    }
    public void EsPlayerNexusUpgradedMessage(object[] data)
    {
        Debug.Log((byte)data[0] + " 유닛이 죽음 데이터 받았다!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

}