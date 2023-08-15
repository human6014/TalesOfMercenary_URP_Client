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
/// 
/// <크리티컬 속성 로직>
/// 우위 속성일 경우 상대 방어력 20& 감소
/// 열위 속성일 경우 상대방 방어력 20%증가
/// </summary>
/// 
public class RangeAttack : Attackable
{
    private System.Random mRand = new System.Random();

    private float SkillProbability = 100;

    public override AttackType Attack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement)
    {
        AttackType attackType = AttackType.Normal;

        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("스킬 발동");
                attackType = AttackType.Skill;
                //attackedUnit.SkillAttackAnimation();
                NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
                SpecialMove(attackUnit, attackUnitElement, attackedUnit, attackedUnitElement);
            }
            else Attack(attackUnit, attackUnitElement, attackedUnit, attackedUnitElement, ref attackType);
        }
        else Attack(attackUnit, attackUnitElement, attackedUnit, attackedUnitElement, ref attackType);

        return attackType;
    }

    private void Attack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement, ref AttackType attackType)
    {
        Debug.Log("피격 유닛: " + attackedUnit + "  <--->   공격 유닛: " + attackUnit);
        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
        {
            attackType = AttackType.Critical;
            //attackUnit.CriticalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            CriticalAttack(attackUnit, attackUnitElement, attackedUnit, attackedUnitElement);
        }
        else
        {
            //Debug.Log("일반공격");
            attackType = AttackType.Critical;
            //attackUnit.NormalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            NormalAttack(attackUnit, attackUnitElement, attackedUnit, attackedUnitElement);
        }
    }

    public override void NormalAttack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement)
    {
        CompetitiveEdge competitiveEdge = GetCompetitiveEdge(attackUnitElement, attackedUnitElement);
        int damage;
        switch (competitiveEdge)
        {
            case CompetitiveEdge.Superiority:
                damage =
                    (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 0.8));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);
                break;

            case CompetitiveEdge.Inferiority:
                damage =
                   (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 1.2));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);
                break;

            case CompetitiveEdge.Normal:
                damage =
                   (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);
                break;

            default:
                break;
        }
    }

    public override void CriticalAttack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement)
    {
        CompetitiveEdge competitiveEdge = GetCompetitiveEdge(attackUnitElement, attackedUnitElement);
        int damage;
        switch (competitiveEdge)
        {
            case CompetitiveEdge.Superiority:
                damage =
                    (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str * NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalDamage - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 0.8));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);

                break;

            case CompetitiveEdge.Inferiority:
                damage =
                   (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str * NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalDamage - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 1.2));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);

                break;

            case CompetitiveEdge.Normal:
                damage =
                   (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str * NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def);
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, attackUnit, attackedUnit);
                break;

            default:
                break;
        }
    }

    public override void SpecialMove(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement)
    {
        CompetitiveEdge competitiveEdge = GetCompetitiveEdge(attackUnitElement, attackedUnitElement);
        int damage;
        switch (competitiveEdge)
        {
            case CompetitiveEdge.Superiority:
                damage =
                    (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.skillDamage - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 0.8));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);

                break;

            case CompetitiveEdge.Inferiority:
                damage =
                   (int)Math.Ceiling(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.skillDamage - (NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def * 1.2));
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);
                break;

            case CompetitiveEdge.Normal:
                damage =
                   NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.skillDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def;
                NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(damage, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);
                break;

            default:
                break;
        }
    }
}
