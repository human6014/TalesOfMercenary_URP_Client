using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Scriptable;
using System.ComponentModel;

//todo

//

public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager; //카드 갯수 우선 4로 바꿔놓음

    private BuildingCard [] mBuildingCards;
    private BuildingCard[] mEnemyBuildingCards; 
    private int[] mSelectingCardIndex;
    private int[] mSelectingEnemyCardIndex;
    private int mEnemySelectingCount;
    private int mSelectingCount;
    private bool mPickingComp = false;
    private bool mEnemyPickingComp = false;
    private PhotonView mPhotonView;


    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mSelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum / 2];
        mSelectingEnemyCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum / 2];
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
        Debug.Log("아군 카드 뽑기" + cardUniqueNumber + "    :    " + mSelectingCount);
        mPhotonView.RPC(nameof(PickingBuildingCardRPC), RpcTarget.Others, cardUniqueNumber);

        if (mSelectingCount == mBuildingCardManager.MaxBuildingCardNum) PickingComplete();
    }

    
    [PunRPC]
    public void PickingBuildingCardRPC(int cardUniqueNumber) //적이 픽 한 카드 데이트 받기
    {
        mSelectingEnemyCardIndex[mEnemySelectingCount++] = cardUniqueNumber;

        //상대방 빌딩카드 인스턴시에이트 해주세요//
        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);
        Debug.Log("적 카드 뽑기" + cardUniqueNumber + "    :    " + mSelectingCount);
        if (mEnemySelectingCount == (mBuildingCardManager.MaxBuildingCardNum / 2)) EnemyPickingComplete();
    }
   
    /// <summary>
    /// 모든 카드 선택 완료시 발동
    /// </summary>
    private void PickingComplete()
    {
        mPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        if(mPickingComp == true && mEnemyPickingComp == true)
        {
            EndPickEvent();
        }
    }

    private void EnemyPickingComplete()
    {
        mEnemyPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        if (mPickingComp == true && mEnemyPickingComp == true)
        {
            EndPickEvent();
        }
    }


    //수정 필요
    public void EndPickEvent()
    {
        for(int i = 0; i < mBuildingCards.Length; i++)
        {
            mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
            mEnemyBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
        }


        gameObject.SetActive(false);
    }
       
}
