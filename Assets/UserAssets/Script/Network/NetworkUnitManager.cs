using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkUnitManager : MonoBehaviour
{
    public static Unit[] usingUnit = new Unit[4];

    public static Dictionary<ushort, Unit> unitList = new();

    #region Massage

    [MessageHandler((ushort)ServerToClientId.playerSpawnedUnit)]
    private static void PlayerSpawnedUnit(Message message)
    {
        ushort ownerID = message.GetUShort();
        byte unitDataNum = message.GetByte();
        ushort instanceID = message.GetUShort();
        Vector3 spawnPosition = message.GetVector3();
        Vector3 finalDestination = message.GetVector3();

        Debug.LogFormat("RecivePlayerSpawnedUnit(), {0}, {1}, {2}, {3}, {4}", ownerID, unitDataNum, instanceID, spawnPosition, finalDestination);
        Unit unit = Instantiate(usingUnit[unitDataNum], spawnPosition, Quaternion.identity);
        unit.InitBatch(ownerID, instanceID, finalDestination);

        unitList.Add(instanceID, unit);
    }

    [MessageHandler((ushort)ServerToClientId.playerBuildingUpgraded)]
    private static void PlayerBuildingUpgrade(Message message)
    {
        ushort ownerID = message.GetUShort();
        int buildingINFO = message.GetInt();
        byte buildingLevel = message.GetByte();

        Player.playerBuildings[ownerID][buildingINFO].CardLevel = buildingLevel; //신규

        //DeckCard[] tem = Player.playerBuildings[ownerID]; 기존
        //tem[buildingINFO].CardLevel = buildingLevel;
        Debug.LogFormat("RecivePlayerBuildingUpgraded(), {0}, {1}, {2}", ownerID, buildingINFO, buildingLevel);
    }

    [MessageHandler((ushort)ServerToClientId.playerNexusUpgraded)]
    private static void PlayerNexusUpgrade(Message message)
    {
        ushort ownerID = message.GetUShort();

        Player.playerNexus[ownerID].CardLevel++;
        Debug.LogFormat("RecivePlayerNexusUpgrade(), {0}", ownerID);
    }

    [MessageHandler((ushort)ServerToClientId.unitAttack)]
    private static void UnitAttack(Message message)
    {
        ushort attackingUnitID = message.GetUShort();
        ushort attackedUnitID = message.GetUShort();
        int damage = message.GetInt();

        unitList[attackedUnitID].GetDamage(damage);
        Debug.LogFormat("ReciveUnitAttack(), {0}, {1}, {2}", attackingUnitID, attackedUnitID, damage);
    }

    [MessageHandler((ushort)ServerToClientId.unitDied)]
    private static void UnitDied(Message message)
    {
        ushort unitID = message.GetUShort();

        unitList[unitID].Die();
        Debug.LogFormat("ReciveUnitDied(), {0}", unitID);
    }

    [MessageHandler((ushort)ServerToClientId.unitMovement)]
    private static void UnitMovement(Message message)
    {
        ushort unitID = message.GetUShort();
        Vector3 destination = message.GetVector3();

        unitList[unitID].SetDestination(destination);
        Debug.LogFormat("ReciveUnitMovement(), {0}, {1}", unitID, destination);
    }

    [MessageHandler((ushort)ServerToClientId.playerDrawedCard)]
    private static void PlayerDrawedCard(Message message)
    {
        int cardID = message.GetInt();

        //....
    }

    [MessageHandler((ushort)ServerToClientId.unitTrackMovemnt)]
    private static void UnitTrackMovement(Message message)
    {
        ushort trackingInstanceID = message.GetUShort();
        ushort trackedInstanceID = message.GetUShort();

        unitList[trackingInstanceID].SetDestination(unitList[trackedInstanceID].transform.position);
    }

    public static void SendUnitSpawn(byte unitDataNum, Vector3 spawnPosition)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.spawnUnit);
        message.AddByte(unitDataNum);
        message.AddVector3(spawnPosition);
        NetworkManager.NetworkManagerSingleton.Client.Send(message);
        Debug.LogFormat("SendUnitSpawn(), {0}, {1}", unitDataNum, spawnPosition);
    }

    public static void SendPlayerDrawCard()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.playerDrawedCard);
        NetworkManager.NetworkManagerSingleton.Client.Send(message);
        Debug.LogFormat("SendPlayerDrawCard()");
    }

    public static void SendNeuxsUpgrade()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.nexusUpgraded);
        NetworkManager.NetworkManagerSingleton.Client.Send(message);
        Debug.LogFormat("SendNeuxsUpgrade()");
    }

    public static void SendBuildingUpgrade(int buildingID)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.buildingUpgrad);
        message.AddInt(buildingID);
        NetworkManager.NetworkManagerSingleton.Client.Send(message);
        Debug.LogFormat("SendBuildingUpgrade(), {0}", buildingID);
    }

    public static void SendDestinationInput(ushort unitID, Vector3 destination)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.unitDestinationInput);
        message.AddUShort(unitID);
        message.AddVector3(destination);
        NetworkManager.NetworkManagerSingleton.Client.Send(message);
        Debug.LogFormat("SendDestinationInput(), {0}, {1}", unitID, destination);
    }

    #endregion

    void Start()
    {
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
    }
}
