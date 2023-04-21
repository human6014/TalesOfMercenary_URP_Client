
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Host = 0
/// Client = 1
/// </summary>
public class NetworkUnitManager : MonoBehaviour
{
    public static readonly int Host = 0;
    public static readonly int Client = 1;

    [SerializeField] private Nexus[] damageable;

    public static Damageable[] usingUnit = new Damageable[4];
    /// <summary>
    /// instanceID는 현재 필드에있는 공격 가능한 모든 유닛의 번호로 설정하고 싶다.
    /// 넥서스가 0,1을 고정으로 할당하고 시작 
    /// HOST 넥서스 = 0
    /// CLIENT 넥서스 = 1
    /// </summary>
    public static int instanceID = 2;
    //{ "유닛 인스턴스 아이디": "유닛" }
    public static Dictionary<int, Damageable> unitList = new();
    public static Dictionary<int, BuildingCard> buildingList = new();



    void Awake()
    {
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
        for(int i = 0; i < damageable.Length; i++) unitList.Add(i, damageable[i]);
    }

    #region 클라이언트
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

        Debug.Log("SpawnUnit Host한테 수신받음");
        //Debug.LogFormat("UnitSpwan : , {0} , {1}, {2}, {4}, {5}", userID, unitID, unitInstanceID, position, targetInstaceID);
        unitList.Add( unitInstanceID, usingUnit[unitID]);
        GameObject obj = Instantiate(usingUnit[unitID], position, Quaternion.identity).gameObject;

        unitList.TryGetValue(targetInstaceID, out Damageable damageable);
        obj.GetComponent<Unit>().InitBatch(userID, unitInstanceID,damageable.transform.position);
    }
    
    public static void UnitMove_Vector(int unitInstanceID, Vector3 position)
    { 
        /*todo
         * 들어온 값을 배열의 유닛의 최종 목적지로 설정해준다.
         */
    }

    public static void UnitAttack(int attackUnitInstanceID, int attackedUnitInstanceID, int damage)
    {
        
    }

    public static void UnitMove_target(int unitInstanceID, int targetID)
    {

    }

    public static void UnitDied(int unitInstanceID)
    {

    }

    public static void PlayerNexusUpgraded()
    {
        //배열의 넥서스 레벨을 올린다
    }
    #endregion

    #region 호스트
    /// <summary>
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
        //unitList.Add(instanceID, usingUnit[unitID]);

        Debug.Log("생성할 유닛 인스턴스 번호" + instanceID);
        SendEvent.HplayerSpawnedUnit(userID, unitID, instanceID, position, 0);
        unitList.Add(instanceID, usingUnit[unitID]);
        GameObject obj = Instantiate(usingUnit[unitID], position, Quaternion.identity).gameObject;

        unitList.TryGetValue(0, out Damageable damageable);
        obj.GetComponent<Unit>().InitBatch(userID, 0, damageable.transform.position);
        //여기에 유닛 배열에 추가및 초기화
        Debug.LogFormat("클라에게 유닛 생성메시지 전달 완료");
        instanceID++;
        Debug.Log(instanceID);
    }

    public static void InputUnitMove_Vector(int unitInstanceID, Vector3 position)
    {
        /*todo
         * 1. 자신의 유닛을 움직였나 확인
         * 2. 올바른 입력이면 유닛 움직임을 클라이언트에게 보낸다
         * 3. 배열의 유닛의 설정값을 바꿔준다.
         */
        UnitMove_Vector(unitInstanceID, position);
    }
    #endregion
}