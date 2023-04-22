using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAnimationController : MonoBehaviour
{
    private Animator m_Animator;

    #region Animation sting
    private const string IdleState = "IsIdle";
    private const string MoveState = "IsMove";
    private const string AttackState = "Attack";
    #endregion
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        m_Animator.SetTrigger("Attack");
    }

    public void PlayMoveAnimation(bool isActive)
    {
        m_Animator.SetBool("IsMove", isActive);
    }
}
