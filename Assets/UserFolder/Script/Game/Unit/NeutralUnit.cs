using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class NeutralUnit : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private Slider HPbar;
    [SerializeField] private int maxHP = 1000;

    private float currentHP;
    private float def;

    private float initTime = 3.0f;
    private bool isBatch;
    public bool isAlive { get; private set; }

    private Vector3 destPos;
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

    public void Hit(float _str)
    {
        if (!isAlive) return;
        currentHP -= (_str - def);
        if (currentHP <= 0)
        {
            Die();
            return;
        }
        HPbar.value = currentHP;
    }

    private void Die()
    {
        isAlive = false;
    }
}
