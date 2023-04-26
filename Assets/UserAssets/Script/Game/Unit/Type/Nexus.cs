using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus :  Damageable
{
    public bool isPlayer { get; private set; } = false;

    private bool isGameEnd = false;

    private void GameEnd()
    {
        isGameEnd = true;
    }

    public override void GetDamage(int damage)
    {
        if (isGameEnd) return;
        HPbar.value = (Hp -= damage);
        if (Hp <= 0) GameEnd();
    }
}
