using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCardManager : MonoBehaviour
{
    private const int maxDeckCardNum = 2;

    #region Serialize Member
    [Header("Prefab or CS")]

    [Tooltip("게임에 가져갈 덱에 해당하는 카드")] // 일단 다 넣어~
    [SerializeField] private BuildingCard[] deckCardPrefab;

    [Tooltip("게임에 가져갈 마법 카드")]
    [SerializeField] private Card[] magicCard;


    [Header("Pool")]

    [Tooltip("덱에 위치할 카드의 RectTransform")]
    [SerializeField] private RectTransform activeDeckPool;

    [Tooltip("넥서스 카드가 위치할 RectTransform")]
    [SerializeField] private RectTransform nexusCardPool;

    #endregion
    private UnitJsonData[] unitJsonDatas;
    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] deckCards;

    /// <summary>
    /// 가지고 있는 덱에서 일정 확률로 생산 카드의 유닛 카드를 반환함
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomUnitCard()
    {
        int rand = Random.Range(1, deckCards.Length);
        return deckCards[rand].GetRandomCard();
    }

    /// <summary>
    /// 가지고 있는 마법카드에서 일정 확률로 마법카드를 반환함
    /// </summary>
    /// <returns></returns>
    public Card GetRandromMagicCard()
    {
        int rand = Random.Range(0, magicCard.Length);
        return magicCard[rand];
    }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        deckCards = new BuildingCard[maxDeckCardNum + 1];
        //유닛 생산 카드 + 넥서스 카드
    }

    private void Start()
    {
        LoadDeckInfo();
        for (int i = 0; i < maxDeckCardNum + 1; i++) LoadDeck(i);

        nexusCard = deckCards[0];
    }

    #region 덱 관련
    private void LoadDeckInfo()
    {
        unitJsonDatas = gameManager.GetDeckInfo();
    }

    /// <summary>
    /// 최초 한번 덱을 로드
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1를 가지는 식별번호</param>
    private void LoadDeck(int cardId)
    {
        //Debug.Log("LoadDeck");
        //DeckCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[unitJsonDatas[cardId - 1].unitID + 1];
        BuildingCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[cardId % 2 + 1];

        RectTransform deckCardTransform = Instantiate(usingDeckCard).GetComponent<RectTransform>();

        RectTransform parentTransform = cardId == 0 ? nexusCardPool : activeDeckPool;
        deckCardTransform.SetParent(parentTransform, true);
        deckCardTransform.anchoredPosition = new Vector2(cardId == 0 ? 0 : (-50 + (cardId - 1) * 100), 0);

        deckCardTransform.TryGetComponent(out BuildingCard deckCard);

        deckCard.CardId = cardId;
        deckCard.CardLevel = 1;
        deckCard.CardMaxLevel = 3;
        deckCard.OnPointerDownAction += PromoteDeckCard;
        deckCards[cardId] = deckCard;
    }

    /// <summary>
    /// 덱 카드를 업그레이드 함
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1를 가지는 식별번호</param>
    private void PromoteDeckCard(int cardId)
    {
        //Debug.Log("PromoteDeckCard" + cardId);

        if (deckCards[cardId].CardLevel >= deckCards[cardId].CardMaxLevel) return;
        if (!gameManager.DoValidGold(deckCards[cardId].CardUpgradeCost)) return;
        if (cardId == 0)
        {
            gameManager.UpgradeNexus(0.05f);
            NetworkUnitManager.SendNeuxsUpgrade();
        }
        else NetworkUnitManager.SendBuildingUpgrade(deckCards[cardId].CardUniqueNumber);
        deckCards[cardId].CardLevel += 1;
        Text cardText = deckCards[cardId].GetComponentInChildren<Text>();
        cardText.text = deckCards[cardId].CardName + "\n" + deckCards[cardId].CardLevel.ToString();
    }
    #endregion
}
