using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Nexus : Damageable
{
    [SerializeField] private int HasPlayerNumber;
    [SerializeField] private GameObject[] mFocusArea;
    private float mMaximum;
    
    private bool isGameEnd = false;

    //private PhotonView mPhotonView;
    private Damageable mTarget;
    public string UUID;
    public bool IsMine { get; set; }

    private void Awake()
    {
        //mPhotonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient && HasPlayerNumber == 0) IsMine = true;
        if (!PhotonNetwork.IsMasterClient && HasPlayerNumber == 1) IsMine = true;

        FindMaximumArea();
        IsAlive = true;

        if (IsMine) Debug.Log(transform.name + " Mine ");
    }

    public override string getUUID()
    {
        return "1";
    }

    private void GameEnd()
    {
        isGameEnd = true;
    }

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        //mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.All, damage, attackUnit);
    }

    public void GetDamageRPC(int damage, Damageable attackUnit)
    {
        if (isGameEnd) return;
        mCurrentHp -= damage;
        if (mCurrentHp <= 0) GameEnd();
    }

    private void FindMaximumArea()
    {
        foreach(GameObject g in mFocusArea)
            mMaximum = Mathf.Max(mMaximum,Vector3.Distance(transform.position, g.transform.position));
        mMaximum += 0.25f;
    }

    public void DisplaySpawnAbleArea(bool isActive)
    {
        foreach(GameObject g in mFocusArea) g.transform.GetChild(0).gameObject.SetActive(isActive);
    }

    public bool IsInArea(Vector3 hitPos) 
        => Vector3.Distance(transform.position, hitPos) <= mMaximum;
    
}
