using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class UnitAnimationController : MonoBehaviour
{
    private Animator m_Animator;

    private void Awake() => m_Animator = GetComponent<Animator>();
    
    [PunRPC]
    public void PlayBoolAnimation(string animName, bool isActive) => m_Animator.SetBool(animName, isActive);
    
    [PunRPC]
    public void PlayTriggerAnimation(string animName) => m_Animator.SetTrigger(animName);
}