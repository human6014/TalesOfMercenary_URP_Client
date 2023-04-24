using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
/// <summary>
/// 공격 로직
/// 
/// </summary>
public class NeutralUnit : Damageable
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private int maxHP = 1000;
    private float currentHP;
    private float def;
    private float initTime = 3.0f;
    private bool isBatch;
    private Vector3 destPos;

    public bool isAlive { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Init(Vector3 _destPos)
    {
        isBatch = true;
        destPos = _destPos;
        currentHP = maxHP;
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

    public override void GetDamage(int damage)
    {
        if (!isAlive) return;
        if (currentHP <= 0)
        {
            Die();
            return;
        }
        HPbar.value = currentHP;
    }
}
