using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager;

    private BuildingCard [] mBuildingCards;
    private int[] mSelectingCardIndex;
    private int mSelectingCount;
    private bool mPickingComp;
    private void Awake()
    {
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mSelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];

        int i = 0;
        foreach (Transform child in mAllBuildingCard)
        {
            mBuildingCards[i] = child.GetComponent<BuildingCard>();
            mBuildingCards[i].OnPointerDownAction += PickingBuildingCard;
            i++;
        }
    }

    /// <summary>
    /// 건물 카드 UI 클릭 시 발동
    /// </summary>
    /// <param name="cardUniqueNumber">어떤 건물 카드인지 구분하는 인덱스</param>
    private void PickingBuildingCard(int cardUniqueNumber)
    {
        if (mPickingComp) return;

        mSelectingCardIndex[mSelectingCount++] = cardUniqueNumber;

        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);

        if (mSelectingCount == mBuildingCardManager.MaxBuildingCardNum) PickingComplete();
    }

    /// <summary>
    /// 모든 카드 선택 완료시 발동
    /// </summary>
    private void PickingComplete()
    {
        mPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        EndPickEvent();
    }
        

    public void EndPickEvent()
    {
        for(int i = 0; i < mBuildingCards.Length; i++)
            mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;

        gameObject.SetActive(false);
    }
       
}
