using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Nexus : Damageable
{
    [SerializeField] private int HasPlayerNumber;
    public bool IsMine { get; set; }
    private bool isGameEnd = false;

    //private PhotonView mPhotonView;
    private Damageable mTarget;
    public string UUID;

    [SerializeField] private GameObject[] mFocusArea;

    private void Awake()
    {
        //mPhotonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient && HasPlayerNumber == 0) IsMine = true;
        if (!PhotonNetwork.IsMasterClient && HasPlayerNumber == 1) IsMine = true;

        IsAlive = true;
        if(IsMine) MouseController.ClickAction += DisplayMoveable;

        if(IsMine) Debug.Log(transform.name + " Mine ");
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
        HPbar.value = (Hp -= damage);
        if (Hp <= 0) GameEnd();
    }

    private void DisplayMoveable(bool isClicked)
    {
        
        Debug.Log("Display : " + isClicked);
    }
}
