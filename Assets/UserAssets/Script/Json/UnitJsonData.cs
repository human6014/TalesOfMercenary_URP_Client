using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitJsonData
{
    public int unitID;
    public string unitName;

    public EElement element;

    public int[] maxHP = new int[3];
    public int[] def = new int[3];
    public int[] mp = new int[3];
    public int[] str = new int[3];
    public float[] speed = new float[3];

    public int[] criticalDamage = new int[3];
    public float[] criticalRate = new float[3];
    public float[] attackRange = new float[3];
    public float[] detectRange = new float[3];
    public float[] attackSpeed = new float[3];

    public UnitJsonData(bool isSet)
    {
        /*
        if (!isSet) return;

        unitID = 0;
        unitName = "Warrior";
        element = EElement.FIRE;


        maxHP[0] = 100;
        def[0] = 10;
        mp[0] = 0;
        str[0] = 30;
        speed[0] = 1;
        criticalDamage[0] = 0;
        criticalRate[0] = 0;
        attackRange[0] = 0.5f;
        detectRange[0] = 2.5f;
        attackSpeed[0] = 1.5f;


        maxHP[1] = 150;
        def[1] = 15;
        mp[1] = 0;
        str[1] = 35;
        speed[1] = 1;
        criticalDamage[1] = 5;
        criticalRate[1] = 15;
        attackRange[1] = 0.5f;
        detectRange[1] = 2.5f;
        attackSpeed[1] = 1.5f;


        maxHP[2] = 200;
        def[2] = 20;
        mp[2] = 0;
        str[2] = 40;
        speed[2] = 1;
        criticalDamage[2] = 10;
        criticalRate[2] = 30;
        attackRange[2] = 0.5f;
        detectRange[2] = 2.5f;
        attackSpeed[2] = 1.5f;
        */
    }

    public void Print()
    {
        Debug.Log("id = " + unitID);
        Debug.Log("name = " + unitName);
        //Debug.Log("level = " + element.ToString());

        /*
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("level = " + (i + 1));
            Debug.Log("maxHP = " + maxHP[i]);
            Debug.Log("def = " + def[i]);
            Debug.Log("mp = " + mp[i]);
            Debug.Log("str = " + str[i]);
            Debug.Log("speed = " + speed[i]);
            Debug.Log("criticalDamage = " + criticalDamage[i]);
            Debug.Log("criticalRate = " + criticalRate[i]);
            Debug.Log("attackRange = " + attackRange[i]);
            Debug.Log("detectRange = " + detectRange[i]);
            Debug.Log("attackSpeed = " + attackSpeed[i]);
        }
        */

    }
}
