using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Nexus : Damageable
{
    public bool isPlayer { get; private set; } = false;
    private bool isGameEnd = false;

    //private PhotonView mPhotonView;
    private Damageable mTarget;
    public string UUID;
    private void Awake()
    {
        //mPhotonView = GetComponent<PhotonView>();
        IsAlive = true;
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
}
