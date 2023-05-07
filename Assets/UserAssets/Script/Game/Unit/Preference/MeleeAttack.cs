using UnityEngine;

/// <summary>
/// 현재 속성 대미지 관련 연산은 빠져있음 
/// <데미지 연산 로직>
/// 공격력 - 방어력
/// 
/// <크리티컬 연산 로직>
/// 공격력 * 치명타데미지 - 방어력
/// </summary>
/// 


public class MeleeAttack : Attackable
{
    private System.Random mRand = new System.Random();

    private float SkillProbability = 20;

    public override AttackType Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        AttackType attackType = AttackType.Normal;
        if (attackedUnit.mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("스킬 발동");
                attackType = AttackType.Skill;
                //attackedUnit.SkillAttackAnimation();
                SpecialMove(attackUnit, attackedUnit);
            }
            else Attack(attackUnit, attackedUnit, ref attackType);
        }
        else Attack(attackUnit, attackedUnit, ref attackType);

        return attackType;
    }

    private void Attack(Damageable attackUnit, Damageable attackedUnit, ref AttackType attackType)
    {
        if (attackUnit.mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
        {
            Debug.Log("크리티컬공격");
            attackType = AttackType.Critical;
            //attackUnit.CriticalAttackAnimation();
            CriticalAttack(attackUnit, attackedUnit);
        }
        else
        {
            Debug.Log("일반공격");
            attackType = AttackType.Critical;
            //attackUnit.NormalAttackAnimation();
            NormalAttack(attackUnit, attackedUnit);
        }
    }

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
    
    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);

    public override void SpecialMove(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.skillDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
}
