using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// 반드시 시리얼라이즈 등록 해야함 모든 클래스는
/// </summary>
public abstract class Damageable : MonoBehaviour
{

    [SerializeField] public Scriptable.UnitScriptable mUnitScriptable;
    [SerializeField] protected Slider HPbar;
    public int Hp { get; protected set; } //현재 체력

    public bool IsAlive { get; protected set; }

    public abstract void GetDamage(int damage, string attackUnitUUID);
}

