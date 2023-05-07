using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAnimationController : MonoBehaviour
{
    private Animator m_Animator;

    #region Animation sting
    private const string MoveState = "IsMove";
    private const string AttackState = "Attack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "IsDie";
    private const string HitState = "Hit";
    #endregion

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void PlayMoveAnimation(bool isActive)
    {
        m_Animator.SetBool(MoveState, isActive);
    }

    public void PlayAttackAnimation()
    {
        m_Animator.SetTrigger(AttackState);
    }

    public void PlaySkillAttackAnimation()
    {
        m_Animator.SetTrigger(SkillAttackState);
    }

    public void PlayDieAnimation(bool isActive)
    {
        m_Animator.SetBool(DieState, isActive);
    }

    public void PlayDieAnimationOn()
    {
        m_Animator.SetBool(DieState, true);
    }

    public void PlayDieAnimationOff()
    {
        m_Animator.SetBool(DieState, false);
    }



    public void PlayHitAnimation()
    {
        m_Animator.SetTrigger(HitState);
    }
}
