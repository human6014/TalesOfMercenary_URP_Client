using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
    public abstract void Attack(Damageable attackUnit, Damageable attackedUnit);

    public abstract void NormalAttack(Damageable attackUnit, Damageable attackedUnit);

    public abstract void CriticalAttack(Damageable attackUnit, Damageable attackedUnit);

}
