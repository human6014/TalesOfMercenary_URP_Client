using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 속성 대미지 관련 연산은 빠져있음 
/// <데미지 연산 로직>
/// 공격력 - 방어력
/// 
/// <크리티컬 연산 로직>
/// 공격력 * 치명타데미지 - 방어력
/// </summary>
public class MeleeAttack : Attackable
{
    private System.Random mRand = new System.Random();

    public override void Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        //크리티컬 발생 시
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

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
    }



    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
}
