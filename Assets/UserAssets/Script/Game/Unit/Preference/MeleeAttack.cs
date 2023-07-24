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

    public override AttackType Attack(string attackUnit, string attackedUnit)
    {
        Debug.Log(attackUnit + ": 공격 ,      " + attackedUnit + ": 피격");
        AttackType attackType = AttackType.Normal;
        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("스킬 발동");
                attackType = AttackType.Skill;
                //attackedUnit.SkillAttackAnimation();
                NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
                SpecialMove(attackUnit, attackedUnit);
            }
            else Attack(attackUnit, attackedUnit, ref attackType);
        }
        else Attack(attackUnit, attackedUnit, ref attackType);

        return attackType;
    }

    private void Attack(string attackUnit, string attackedUnit, ref AttackType attackType)
    {
        Debug.Log("피격 유닛: "+ attackedUnit + "  <--->   공격 유닛: " + attackUnit);
        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
        {
            //Debug.Log("크리티컬공격");ㄴ
            attackType = AttackType.Critical;
            //attackUnit.CriticalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            CriticalAttack(attackUnit, attackedUnit);
        }
        else
        {
            //Debug.Log("일반공격");
            attackType = AttackType.Critical;
            //attackUnit.NormalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            NormalAttack(attackUnit, attackedUnit);
        }
    }

    public override void NormalAttack(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);

    public override void CriticalAttack(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str * NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);

    public override void SpecialMove(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.skillDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);
}
