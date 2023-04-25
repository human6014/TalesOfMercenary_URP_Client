using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
/// <summary>
/// 공격 로직
/// 
/// </summary>
public class NeutralUnit : Damageable
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private Slider HPbar;
    [SerializeField] private int maxHP = 1000;
    private float initTime = 3.0f;
    private bool isBatch;
    private Vector3 destPos;
    private Damageable mTarget;
    PhotonView mPhotonView;

    public bool isAlive { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        mPhotonView = GetComponent<PhotonView>();
    }

    public void Init()
    {
        isBatch = true;
        mHp = maxHP;
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
        if(PhotonNetwork.IsMasterClient)
        {
            NetworkUnitManager.myUnitList.Add(this);
        }
    }

    public void Update()
    {
        if (isBatch && !isAlive)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, Time.deltaTime * initTime);
            if (transform.position == destPos)
            {
                isAlive = true;
                navMeshAgent.enabled = true;
                animator.SetBool("isIdle",true);
            }
        }
    }

    private void Die()
    {
        isAlive = false;
    }

    public override void getDamage(int damage)
    {
        if (!isAlive) return;
        if (mHp <= 0)
        {
            Die();
            return;
        }
        HPbar.value = mHp;
    }

    [PunRPC]
    public void SyncInitBatch() //적이 소환한 유닛 초기화
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void FindenemyOrNull(Vector3 destination) // 벡터 기준으로 공격 사거리의 적 탐지 null반환 시 적이 없음
    {
        Debug.Log("새로운 공격대상 발견(조건부)");
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (destination - key.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key;
            }
        }
        if (minDis <= mAttackRange) // 공격대상 존재
        {
            Debug.Log("새로운 공격대상 발견");
            mTarget = target;
        }
    }


}
