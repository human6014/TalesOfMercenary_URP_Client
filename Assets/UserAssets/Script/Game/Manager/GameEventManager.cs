using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    [SerializeField] private LayerMask mGroundLayer;
    [SerializeField] private Transform[] neutralUnitSpawnPos;
    [SerializeField] private NeutralUnit neutralUnit;
    [SerializeField] private float mDragonEventTime;

    private Vector3[] neutralUnitInitPos;
    private Vector3[] nexusPos;
    private float mCurrentTime;
    private bool mIsSpawnNeutralUnit;
    private int spawnNum;
    private int mIsHost;

    private void Awake()
    {
        mIsHost = PhotonNetwork.IsMasterClient ? 0 : 1;
    }
    private void Start()
    {
        spawnNum = neutralUnitSpawnPos.Length;
        neutralUnitInitPos = new Vector3[spawnNum];
        nexusPos = new Vector3[spawnNum];

        nexusPos[mIsHost] = NetworkUnitManager.enemyUnitList[0].transform.position;

        if (Physics.Raycast(neutralUnitSpawnPos[mIsHost].position, Vector3.down, out RaycastHit hit, 100, mGroundLayer))
            neutralUnitInitPos[mIsHost] = hit.point;

    }

    private void FixedUpdate()
    {
        mCurrentTime += Time.deltaTime;
        if (!mIsSpawnNeutralUnit && mCurrentTime >= mDragonEventTime)
        {
            mIsSpawnNeutralUnit = true;
            SpawnNeutralUnit();
        }
    }

    //첫 생성은 호스트가 생성한다.
    public void SpawnNeutralUnit()
    {
        GameObject neutralUnit;
        Vector3 dir;
        Quaternion rot;

        dir = (nexusPos[mIsHost] - neutralUnitInitPos[mIsHost]).normalized;
        rot = Quaternion.LookRotation(dir);
        neutralUnit = PhotonNetwork.Instantiate("OfficialUnit/NeutralUnit/" + "RedDragon",
            neutralUnitSpawnPos[mIsHost].position, rot);

        neutralUnit.GetComponent<NeutralUnit>().Init(neutralUnitInitPos[mIsHost]);

    }

}
