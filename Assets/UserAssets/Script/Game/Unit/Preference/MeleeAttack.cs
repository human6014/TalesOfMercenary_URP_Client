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
        if (attackUnit.getCriticalRate() >= mRand.Next(101))//크리티컬 발생 시
        {
            CriticalAttack(attackUnit, attackedUnit);
        }
        NormalAttack(attackUnit, attackedUnit);
    }

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() - attackedUnit.getDef());
    }

    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() * attackedUnit.getCriticalDamage() - attackedUnit.getDef());
    }
}
