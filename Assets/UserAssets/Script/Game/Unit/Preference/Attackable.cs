using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Normal,
    Critical,
    Skill
}
public abstract class Attackable : MonoBehaviour
{
    public abstract AttackType Attack(string attackUnit, string attackedUnit);

    public abstract void NormalAttack(string attackUnit, string attackedUnit);

    public abstract void CriticalAttack(string attackUnit, string attackedUnit);

    public abstract void SpecialMove(string attackUnit, string attackedUnit);
}
