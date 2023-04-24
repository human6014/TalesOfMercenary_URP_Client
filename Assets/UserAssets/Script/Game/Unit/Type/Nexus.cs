using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus :  Damageable
{
    public bool isPlayer { get; private set; } = false;

    [SerializeField] private Slider HPbar;
    [SerializeField] private int HP = 1000;
    private bool isGameEnd = false;

    private void Awake()
    {
        mName = "³Ø¼­½º";
        mHp = HP;
    }

    private void GameEnd()
    {
        isGameEnd = true;
    }

    public override void getDamage(int damage)
    {
        if (isGameEnd) return;
        HPbar.value = (mHp -= damage);
        if (mHp <= 0) GameEnd();
    }
}
