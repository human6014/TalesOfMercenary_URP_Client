using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building 
{
    protected string mName;
    protected int mUpgradePoint;
    protected int mLevel;

    public Building(string name)
    {
        mName = name;
        mUpgradePoint = 0;
        mLevel = 1;
    }
}
