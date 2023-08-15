using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Scriptable;

//todo

//

public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager; //카드 갯수 우선 4로 바꿔놓음

    private PhotonView mPhotonView;
    private BuildingCard [] mBuildingCards;

    private int[] mMySelectingCardIndex;
    private int[] mEnemySelectingCardIndex;

    private int mEnemySelectingCount;
    private int mMySelectingCount;

    private bool mMyPickingComp = false;
    private bool mEnemyPickingComp = false;


    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mMySelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];
        mEnemySelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];
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
        if (mMyPickingComp) return;

        mMySelectingCardIndex[mMySelectingCount++] = cardUniqueNumber;

        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);
        mPhotonView.RPC(nameof(PickingEnemyBuildingCard), RpcTarget.OthersBuffered, cardUniqueNumber);

        if (mMySelectingCount == mBuildingCardManager.MaxBuildingCardNum) mMyPickingComp = true;
        if ((mMyPickingComp && PhotonNetwork.PlayerList.Length == 1) || (mMyPickingComp && mEnemyPickingComp)) PickingComplete();
    }

    [PunRPC]
    private void PickingEnemyBuildingCard(int cardUniqueNumber)
    {
        mEnemySelectingCardIndex[mEnemySelectingCount++] = cardUniqueNumber;
        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mEnemyPickingBuilding);

        if (mEnemySelectingCount == mBuildingCardManager.MaxBuildingCardNum) mEnemyPickingComp = true;
        if (mMyPickingComp && mEnemyPickingComp) PickingComplete();
    }

    /// <summary>
    /// 모든 카드 선택 완료시 발동
    /// </summary>
    private void PickingComplete()
    {
        mBuildingCardManager.RegisterSelectingCard(mMySelectingCardIndex);
        mBuildingCardManager.RegisterEnemySelectingCard(mEnemySelectingCardIndex);

        for (int i = 0; i < mBuildingCards.Length; i++) mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
        
        gameObject.SetActive(false);
    }
       
}
