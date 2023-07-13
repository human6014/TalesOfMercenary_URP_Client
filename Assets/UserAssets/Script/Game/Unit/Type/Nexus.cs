using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Nexus : Damageable
{
    [SerializeField] private int HasPlayerNumber;
    [SerializeField] private GameObject[] mFocusArea;
    [SerializeField] private UnitUIController mUnitUIController;
    private float mMaximum;
    private PhotonView mPhotonView;
    private bool isGameEnd = false;

    private Damageable mTarget;
    public string UUID;
    public GameObject EndUI;
    public TMP_Text Gameovert;

    public override string getUUID()
    {
        return mUnitScriptable.UUID;
    }

    public bool IsMine { get; set; }

    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient && HasPlayerNumber == 0) IsMine = true;
        if (!PhotonNetwork.IsMasterClient && HasPlayerNumber == 1) IsMine = true;
        FindMaximumArea();
        if (IsMine) Debug.Log(transform.name + " Mine ");
        IsAlive = true;
        mCurrentHp = mUnitScriptable.maxHP;
        //mUnitUIController.Init(mCurrentHp);
        mPhotonView.RPC(nameof(Init), RpcTarget.All);
    }

    [PunRPC]
    private void Init()
    {
        mUnitUIController.Init(mCurrentHp, IsMine);
    }

    public void SetUUID(string uuid)
    {
        mUnitScriptable.UUID = uuid;
        UUID = uuid;
    }

    private void GameEnd(string WorL)
    {
        if (!IsMine)
        {
            Gameovert.text = WorL;
        }
        isGameEnd = true;
        EndUI.SetActive(true);
        Invoke("GameLeave", 10f);
        //mPhotonView.RPC(nameof(GameEndRPC), RpcTarget.All);
    }
    private void GameLeave()
    {
        mPhotonView.RPC(nameof(GameEndRPC), RpcTarget.All);
    }
    [PunRPC]
    public void GameEndRPC()
    {
        PhotonNetwork.LoadLevel("Menu");
        PhotonNetwork.LeaveRoom();
    }
    public override void GetDamage(int damage, string attackUnitUUID, string attackedUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.All, damage);
    }

    [PunRPC]
    public void GetDamageRPC(int damage)
    {
        //if (isGameEnd) return;
        if (damage <= 0)
        {
            Debug.Log("넥서스 데미지 안입음 ");
        }

        if (mCurrentHp <= damage || mCurrentHp == 0)
        {
            string WorL = "You Win";
            mCurrentHp = 0;
            Debug.Log("체력 0임");
            GameEnd(WorL);
            return;
        }
        else mCurrentHp -= damage;

        Debug.Log("넥서스 데미지 입음 : " + mCurrentHp);
        mUnitUIController.GetDamage(mCurrentHp);
    }

    private void FindMaximumArea()
    {
        foreach (GameObject g in mFocusArea)
            mMaximum = Mathf.Max(mMaximum, Vector3.Distance(transform.position, g.transform.position));
        mMaximum += 0.25f;
    }

    public void DisplaySpawnAbleArea(bool isActive)
    {
        foreach (GameObject g in mFocusArea) g.transform.GetChild(0).gameObject.SetActive(isActive);
    }

    public bool IsInArea(Vector3 hitPos)
        => Vector3.Distance(transform.position, hitPos) <= mMaximum;
}
