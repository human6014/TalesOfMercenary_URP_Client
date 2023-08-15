using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Normal,
    Critical,
    Skill
}

public enum CompetitiveEdge
{
    Inferiority,
    Superiority,
    Normal
}
public abstract class Attackable : MonoBehaviour
{
    public abstract AttackType Attack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement);

    public abstract void NormalAttack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement);

    public abstract void CriticalAttack(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement);

    public abstract void SpecialMove(string attackUnit, EElement attackUnitElement, string attackedUnit, EElement attackedUnitElement);

    public CompetitiveEdge GetCompetitiveEdge(EElement attackUnitElement, EElement attackedUnitElement)
    {
        if (attackUnitElement == attackedUnitElement)
        {
            return CompetitiveEdge.Normal;
        }
        else
        {
            if (attackUnitElement == EElement.WATER && attackedUnitElement == EElement.FIRE)
            {
                return CompetitiveEdge.Superiority;
            }
            else if (attackUnitElement == EElement.WATER && attackedUnitElement == EElement.NATURER)
            {
                return CompetitiveEdge.Inferiority;
            }
            else if (attackUnitElement == EElement.WATER && attackedUnitElement == EElement.NONE)
            {
                return CompetitiveEdge.Normal;
            }

            else if (attackUnitElement == EElement.FIRE && attackedUnitElement == EElement.WATER)
            {
                return CompetitiveEdge.Inferiority;
            }
            else if (attackUnitElement == EElement.FIRE && attackedUnitElement == EElement.NATURER)
            {
                return CompetitiveEdge.Superiority;
            }
            else if (attackUnitElement == EElement.FIRE && attackedUnitElement == EElement.NONE)
            {
                return CompetitiveEdge.Normal;
            }

            else if (attackUnitElement == EElement.NATURER && attackedUnitElement == EElement.FIRE)
            {
                return CompetitiveEdge.Inferiority;
            }
            else if (attackUnitElement == EElement.NATURER && attackedUnitElement == EElement.WATER)
            {
                return CompetitiveEdge.Superiority;
            }
            else if (attackUnitElement == EElement.NATURER && attackedUnitElement == EElement.NONE)
            {
                return CompetitiveEdge.Normal;
            }

            else
            {
                return CompetitiveEdge.Normal;
            }
        }
    }
}
