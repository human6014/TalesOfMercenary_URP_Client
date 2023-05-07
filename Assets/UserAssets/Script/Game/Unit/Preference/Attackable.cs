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
    public abstract AttackType Attack(Damageable attackUnit, Damageable attackedUnit);

    public abstract void NormalAttack(Damageable attackUnit, Damageable attackedUnit);

    public abstract void CriticalAttack(Damageable attackUnit, Damageable attackedUnit);

    public abstract void SpecialMove(Damageable attackUnit, Damageable attackedUnit);
}
