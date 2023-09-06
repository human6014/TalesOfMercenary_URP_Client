using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardProbability
{
    [SerializeField] private float[] probability;

    public float[] ProbabilityArray { get => probability; }
    public float this[int index] { get => probability[index]; }
}

public class BuildingCardManager : MonoBehaviour
{
    #region Serialize Member
    [Header("Prefab or CS")]
    [Tooltip("넥서스 카드")]
    [SerializeField] private BuildingCard mNexusCard;

    [Tooltip("게임에 가져갈 덱에 해당하는 카드")]
    [SerializeField] private BuildingCard[] deckCardPrefab;

    [Tooltip("게임에 가져갈 마법 카드")]
    [SerializeField] private Card[] magicCard;

    [Header("Pool")]
    [Tooltip("내 건물카드가 위치할 RectTransform")]
    [SerializeField] private RectTransform mMyBuildingCardPool;

    [Tooltip("적 건물카드가 위치할 RectTransform")]
    [SerializeField] private RectTransform mEnemyBuildingCardPool;

    [Tooltip("넥서스 카드가 위치할 RectTransform")]
    [SerializeField] private RectTransform nexusCardPool;

    [Header("CardProbability")]
    [SerializeField] private CardProbability[] mCardProbability;
    #endregion

    public int MaxBuildingCardNum { get; } = 2;
    private const float mTotal = 100;
    private int[] mSelectingCard;
    private int[] mEnemySelectingCard;

    private PhotonView mPhotonView;
    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] mBuildingCards;

    public System.Action CardBatchAction { get; set; }

    private int GetCardProbability(int level)
    {
        float randomPoint = Random.value * mTotal;
        int length = mCardProbability[level].ProbabilityArray.Length;
        for (int i = 0; i < length; i++)
        {
            if (randomPoint < mCardProbability[level][i]) return i;
            else randomPoint -= mCardProbability[level][i];
        }
        return length - 1;
    }

    /// <summary>
    /// 가지고 있는 덱에서 일정 확률로 생산 카드의 유닛 카드를 반환함
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomUnitCard()
    {
        int rand = Random.Range(0, mBuildingCards.Length);
        Debug.Log(rand);
        int level = mBuildingCards[rand].CardCurrentLevel - 1;
        return mBuildingCards[rand].GetCard(GetCardProbability(level));
    }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[MaxBuildingCardNum];
        //유닛 생산 카드 + 넥서스 카드
    }

    public void RegisterSelectingCard(int[] index)
    {
        mSelectingCard = new int[index.Length];

        for (int i = 0; i < mSelectingCard.Length; i++) mSelectingCard[i] = index[i];

        LoadNexusCard();
        for (int i = 0; i < MaxBuildingCardNum; i++) LoadBuildingCard(i);

        CardBatchAction?.Invoke();
    }

    public void RegisterEnemySelectingCard(int[] index)
    {
        mEnemySelectingCard = new int[index.Length];

        for (int i = 0; i < mEnemySelectingCard.Length; i++) mEnemySelectingCard[i] = index[i];
        for (int i = 0; i < MaxBuildingCardNum; i++) LoadEnemyBuildingCard(i);
    }

    #region 덱 관련

    private void LoadNexusCard()
    {
        BuildingCard usingBuildingCard = mNexusCard;

        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();

        deckCardTransform.SetParent(nexusCardPool, true);
        deckCardTransform.anchoredPosition = new Vector2(0, 0);
        deckCardTransform.TryGetComponent(out BuildingCard buildingCard);

        buildingCard.CardID = 0;
        buildingCard.CardCurrentLevel = 1;
        buildingCard.OnPointerDownAction += PromoteBuildingCard;
        nexusCard = buildingCard;
    }

    /// <summary>
    /// 최초 한번 건물을 로드
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1를 가지는 식별번호</param>
    private void LoadBuildingCard(int cardId)
    {
        //DeckCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[unitJsonDatas[cardId - 1].unitID + 1];

        BuildingCard usingBuildingCard = deckCardPrefab[mSelectingCard[cardId]];

        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();

        deckCardTransform.SetParent(mMyBuildingCardPool, true);
        deckCardTransform.TryGetComponent(out BuildingCard buildingCard);

        buildingCard.Init();
        buildingCard.CardID = cardId;
        buildingCard.CardCurrentLevel = 1;
        buildingCard.OnPointerDownAction += PromoteBuildingCard;
        mBuildingCards[cardId] = buildingCard;
    }

    private void LoadEnemyBuildingCard(int cardId)
    {
        BuildingCard usingBuildingCard = deckCardPrefab[mEnemySelectingCard[cardId]];
        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();

        deckCardTransform.SetParent(mEnemyBuildingCardPool, true);
    }

    /// <summary>
    /// 건물을 업그레이드 함
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1를 가지는 식별번호</param>
    private void PromoteBuildingCard(int cardId)
    {
        //Debug.Log("PromoteDeckCard" + cardId);
        if (mBuildingCards[cardId].CardCurrentLevel >= mBuildingCards[cardId].CardMaxLevel) return;
        if (!gameManager.DoValidGold(mBuildingCards[cardId].CardUpgradeCost)) return;
        //if (cardId == 0)
        //{
        //    gameManager.UpgradeNexus(0.05f);
        //    NetworkUnitManager.SendNeuxsUpgrade();
        //}
        //else NetworkUnitManager.SendBuildingUpgrade(deckCards[cardId].CardUniqueNumber);
        mBuildingCards[cardId].CardCurrentLevel += 1;
        Text cardText = mBuildingCards[cardId].GetComponentInChildren<Text>();
        cardText.text = mBuildingCards[cardId].CardCurrentLevel.ToString();
    }
    #endregion
}