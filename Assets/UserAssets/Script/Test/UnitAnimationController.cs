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
    private const string DieState = "IsDie";
    private const string HitState = "Hit";
    #endregion
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    [ContextMenu("Attack")]
    public void PlayAttackAnimation()
    {
        m_Animator.SetTrigger(AttackState);
    }

    [ContextMenu("Hit")]
    public void PlayHitAnimation()
    {
        m_Animator.SetTrigger(HitState);
    }

    public void PlayMoveAnimation(bool isActive)
    {
        m_Animator.SetBool(MoveState, isActive);
    }

    /*
    [ContextMenu("Die")]
    public void PlayDieAnimation(bool isActive)
    {
        m_Animator.SetBool(DieState, isActive);
    }
    */

    [ContextMenu("DieOn")]
    public void PlayDieAnimationOn()
    {
        m_Animator.SetBool(DieState,true);
    }

    [ContextMenu("DieOff")]
    public void PlayDieAnimationOff()
    {
        m_Animator.SetBool(DieState,false);
    }
}
