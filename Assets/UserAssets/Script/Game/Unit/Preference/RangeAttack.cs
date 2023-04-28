using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 현재 속성 대미지 관련 연산은 빠져있음 
/// <데미지 연산 로직>
/// 공격력 - 방어력
/// 
/// <크리티컬 연산 로직>
/// 공격력 * 치명타데미지 - 방어력
/// </summary>
public class RangeAttack : Attackable
{
    private static System.Random mRand = new System.Random();

    public override void Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        if (attackUnit.mUnitScriptable.criticalRate >= mRand.Next(101))
        {
            Debug.Log("크리티컬공격");
            CriticalAttack(attackUnit, attackedUnit);
        }
        else
        {
            Debug.Log("일반공격");
            NormalAttack(attackUnit, attackedUnit);
        }
    }
    //속성 추가피해는 빠진 로직
    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);

    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
}
