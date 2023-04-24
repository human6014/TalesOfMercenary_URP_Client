using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    #region 스탯
    [SerializeField] protected int mMaxHP;         // 최대 체력
    protected int mHp;     // 현재 체력
    [SerializeField] protected int mDef;           // 방어력
    [SerializeField] protected int mMp;            // 마나
    [SerializeField] protected int mStr;           // 공격력
    [SerializeField] protected float mSpeed;         // 이동속도
    [Header("Additional stats")]
    [SerializeField] protected int mCriticalRate;    // 크리티컬율
    [SerializeField] protected int mCriticalDamage;  // 크리티컬 데미지
    [SerializeField] protected float mAttackRange = 0.8f; // 공격 사거리
    [SerializeField] protected float mAttackSpeed = 1.5f; // 공격 속도
    [SerializeField] protected EElement mEelement;  
    private bool mIsAlive { get; set; }
    #endregion
    public string mName;

    public int getCriticalRate()
    {
        return mCriticalRate;
    }

    public bool isAlive()
    {
        return mIsAlive;
    }

    public int getDef()
    {
        return mDef;
    }
    public int getCriticalDamage()
    {
        return mCriticalDamage;
    }
    public int getHp()
    {
        return mHp;
    }

    public int getStr()
    {
        return mStr;
    }

    public abstract void getDamage(int damage);
}
