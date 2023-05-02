using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameEventManager : MonoBehaviour
{
    [SerializeField] private MagicEventDisplayer mMagicEventDisplayer;
    [SerializeField] private LayerMask mGroundLayer;
    [SerializeField] private Transform[] mNeutralUnitSpawnPos;

    [SerializeField] private float mDragonEventTime;
    [SerializeField] private float mMagicEventTime;

    [SerializeField] private bool mDoDragonEvent;
    [SerializeField] private bool mDoMagicEvent;
    [SerializeField] private int mMaxMagicEvent;

    private Vector3[] mNeutralUnitInitPos;
    private Vector3[] mNexusPos;
    private float mCurrentTime;
    private bool mIsSpawnNeutralUnit;
    private int mLastMagicEventNum;
    private int mSpawnNum;
    private int mIsHost;

    private void Awake()
    {
        mIsHost = PhotonNetwork.IsMasterClient ? 0 : 1;

        mSpawnNum = mNeutralUnitSpawnPos.Length;
        mNeutralUnitInitPos = new Vector3[mSpawnNum];
        mNexusPos = new Vector3[mSpawnNum];

        if (Physics.Raycast(mNeutralUnitSpawnPos[mIsHost].position, Vector3.down, out RaycastHit hit, 100, mGroundLayer))
            mNeutralUnitInitPos[mIsHost] = hit.point;
    }

    private void Start()
    {
        mNexusPos[mIsHost] = NetworkUnitManager.enemyUnitList["1"].transform.position;
    }

    private void Update()
    {
        mCurrentTime += Time.deltaTime;
        if (mDoDragonEvent && !mIsSpawnNeutralUnit && mCurrentTime >= mDragonEventTime)
        {
            mIsSpawnNeutralUnit = true;
            SpawnNeutralUnit();
        }

        if (mDoMagicEvent && mCurrentTime >= mMagicEventTime && mLastMagicEventNum < mMaxMagicEvent)
        {
            mMagicEventDisplayer.DisplayMagicEvent();
            mLastMagicEventNum++;
        }
    }

    //첫 생성은 호스트가 생성한다.
    public void SpawnNeutralUnit()
    {
        Vector3 dir = (mNexusPos[mIsHost] - mNeutralUnitInitPos[mIsHost]).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject neutralUnit = (PhotonNetwork.Instantiate("OfficialUnit/NeutralUnit/" + "RedDragon",
            mNeutralUnitSpawnPos[mIsHost].position, rot));

        neutralUnit.GetComponent<NeutralUnit>().Init(mNeutralUnitInitPos[mIsHost]);
    }

    public void SpawnNexus()
    {
        PhotonNetwork.Instantiate("OfficialUnit/Nexus/" + "Nexus",
           mNeutralUnitSpawnPos[mIsHost].position, Quaternion.identity);
    }

}
