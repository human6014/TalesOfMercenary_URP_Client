using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus :  Damageable
{
    public bool isPlayer { get; private set; } = false;

    [SerializeField] private Slider HPbar;
    [SerializeField] private int HP = 1000;
    private int currentHP;
    private bool isGameEnd = false;

    private void Awake()
    {
        currentHP = HP;
    }

    public override void Hit(int damage)
    {
        if (isGameEnd) return;
        HPbar.value = (currentHP -= damage);
        if (currentHP <= 0) GameEnd();
    }

    private void GameEnd()
    {
        isGameEnd = true;
    }
}
