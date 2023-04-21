using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Host = 1
/// Client = 0
/// </summary>
public class NetworkUnitManager : MonoBehaviour
{

    private void Awake()
    {
        //넥서스를 유닛 리스트에 넣기
    }
    public static Unit[] usingUnit = new Unit[4];
    /// <summary>
    /// instanceID는 현재 필드에있는 공격 가능한 모든 유닛의 번호로 설정하고 싶다.
    /// 넥서스가 0,1을 고정으로 할당하고 시작 
    /// HOST 넥서스 = 0
    /// CLIENT 넥서스 = 1
    /// </summary>
    static int instanceID = 2;
    //{ "유닛 인스턴스 아이디": "유닛" }
    private static Dictionary<int, Unit> unitList = new();

    void Start()
    {
        //usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
    }
    public void TakeDamage(int unitID, int amount)
    {

    }

    /// <summary>
    /// 클라이언트용 함수
    /// 타겟 인스턴스를 쫓도록 생성하면 된다. -> 공격 사거리가 되면 거기서 정지하고 공격 
    /// 만일 타깃이 다시 공격 사정거리 밖으로 나가면 다시 공격 사정거리까지 이동 ->쫓아가며 공격하는 로직 필요
    /// 위 로직은 서버에서 따로 패킷을 안 보낸다
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="unitID"></param>
    /// <param name="unitInstanceID"></param>
    /// <param name="position"></param>
    /// <param name="targetInstaceID"></param>
    public static void SpawnUnit(int userID, int unitID, int unitInstanceID, Vector3 position, int targetInstaceID)
    {
        /*todo
         * 1. 유닛을 배열에 추가
         */
        Debug.LogFormat("UnitSpwan : , {0} , {1}, {2}, {4}, {5}", userID, unitID, unitInstanceID, position, targetInstaceID);
    }
    /// <summary>
    /// 호스트용 함수
    /// 해당 위치에서 가장 가까운 적 탐색 후 해당 위치로 타겟 지정 -> 배열을 순회하며 가까운 적으로 타깃 지정
    /// 지정 후 메시지 전송 그리고 해당 유닛 생성 후 배열에 추가
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="unitID"></param>
    /// <param name="position"></param
    public static void SpawnUnit(int userID, int unitID, Vector3 position)
    {

        /*todo
         * 1. 받은 벡터에서 가장 가까운 적 탐지
         * 2. 해당 적을 타깃으로하는 유닛 생성
         * 3. 유닛을 배열에 추가
         */
        int targetInstaceID = 10;
        Debug.Log("생성할 유닛 인스턴스 번호" + instanceID);
        SendEvent.HplayerSpawnedUnit(userID, unitID, instanceID, position, targetInstaceID);
        Debug.LogFormat("클라에게 유닛 생성메시지 전달 완료");
        instanceID++;
        Debug.Log(instanceID);
    }

    public static void UnitMove_Vector(int unitInstanceID, Vector3 position)
    { 
        Debug.LogFormat("SendMovement() {0}, {1}", instanceID, position);
    }
    /*
        #region Massage
        [MessageHandler((ushort)ClientToServerId.spawnUnit)]
        private static void SpawnUnit(ushort ownerID, Message message)
        {
            byte unitDataNum = message.GetByte();
            Vector3 spawnPosition = message.GetVector3();
            Vector3 finalDestination;
            if (ownerID != 1) finalDestination = GameManager.player1Nexus;
            else finalDestination = GameManager.player2Nexus;

            Debug.LogFormat("getSpawnUnit(), {0}, {1}", unitDataNum, spawnPosition);
            //모든 클라이언트에게 생성하라고 메시지 
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawnedUnit);
            newMessage.AddUShort(ownerID);//유닛을 소유한 클라이언트 ID
            newMessage.AddByte(unitDataNum);//데이터베이스 ID
            newMessage.AddUShort(instanceID);//인스턴스 아이디
            newMessage.AddVector3(spawnPosition); //유닛 소환 위치
            newMessage.AddVector3(finalDestination); //최종 이동 위치

            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
            Debug.LogFormat("sendSpawnUnit(), {0}, {1}, {2}, {3}, {4}", ownerID, unitDataNum, instanceID, spawnPosition, finalDestination);
            //유닛풀에 유닛 생성 

            Unit unit = Instantiate(usingUnit[unitDataNum], spawnPosition, Quaternion.identity);
            unit.InitBatch(ownerID, instanceID, finalDestination);


            unitList.Add(instanceID, unit);

            foreach (KeyValuePair<ushort, Unit> units in unitList)
            {
                Debug.LogFormat("UnitList : , {0} , {1}", units.Key, units.Value);
            }
            instanceID++;
        }

        [MessageHandler((ushort)ClientToServerId.unitDestinationInput)]
        private static void ReceiveSendMovement(ushort ownerID, Message message)
        {
            ushort instanceID = message.GetUShort();
            Vector3 destination = message.GetVector3();

            Debug.LogFormat("get unitDestinationInput() {0}, {1}", instanceID, destination);

            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(instanceID);//인스턴스 아이디
            newMessage.AddVector3(destination);//목적지
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);

            unitList[instanceID].SetDestination(destination);

            Debug.LogFormat("SendMovement() {0}, {1}", instanceID, destination);
        }


        public static void SendUnitTrackMovement(ushort trackingInstanceID, ushort trackedInstanceID)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(trackingInstanceID);// 내 유닛 인스턴스 아이디
            newMessage.AddUShort(trackedInstanceID);// 상대방 유닛 인스턴스 아이디
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
        }

        public static void SendUnitMovement(ushort instanceID, Vector3 destination)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(instanceID);//인스턴스 아이디
            newMessage.AddVector3(destination);//목적지
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
        }

        public static void SendUnitAttack(ushort attackingInstanceID, ushort attackedInstanceID, int calcDamage)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitAttack);
            newMessage.AddUShort(attackingInstanceID);// 공격하는 유닛
            newMessage.AddUShort(attackedInstanceID);// 공격당하는 유닛
            newMessage.AddInt(calcDamage);
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
            Debug.LogFormat("SendUnitAttack() {0}, {1}", attackingInstanceID, attackedInstanceID, calcDamage);
        }

        public static void SendUnitDied(ushort unitInstanceID)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitDied);
            message.AddUShort(unitInstanceID); //죽은 유닛 인스턴스 아이디
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(message);

            Debug.LogFormat("SendUnitDied() {0}", unitInstanceID);
        }

        #endregion*/
}