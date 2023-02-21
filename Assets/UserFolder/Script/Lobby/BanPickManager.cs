using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanPickManager : MonoBehaviour
{
    private bool isBanPick;
    private bool isMyTurn;

    [SerializeField] private CardDataManager cardDataManager;
    [SerializeField] private RectTransform cardPool;
    [SerializeField] private RectTransform mySelectingCardPool;
    [SerializeField] private RectTransform opponentSelectingCardPool;
    [SerializeField] private RectTransform banPickPool;

    private readonly int[] selectingCardNum = new int[2];
    private readonly int[] selectingBanCardNum = new int[2] { 2, 3 };

    private DeckCard clickedDeckCard;
    private DeckCard[] deckCards = new DeckCard[2];
    private Card[] magicCards = new Card[2];

    private int selectingCardCount = 0;
    private int selectingBanCardCount = 0;

    private const int rowMax= 4;

    private void Start()
    {
        deckCards = cardDataManager.GetBuildingCards();
        magicCards = cardDataManager.GetMagicCards();

        for(int i = 0; i < deckCards.Length; i++)
        {
            LoadDeckCards(i);
        }
        for(int i = 0; i < magicCards.Length; i++)
        {
            LoadMagicCard(i);
        }
    }

    private void LoadDeckCards(int cardId)
    {
        RectTransform deckCardTransform = Instantiate(deckCards[cardId]).GetComponent<RectTransform>();

        int rowPos = (-150 + (cardId % rowMax) * 100);
        int colPos = 80 + (cardId / rowMax * -80);

        deckCardTransform.SetParent(cardPool, true);
        deckCardTransform.anchoredPosition = new Vector2(rowPos, colPos);
        deckCardTransform.TryGetComponent(out DeckCard deckCard);

        deckCard.CardId = cardId;
        deckCard.CardLevel = 1;
        deckCard.CardMaxLevel = 3;
        deckCard.OnPointerDownAction += OnClicked;
        deckCards[cardId] = deckCard;
    }

    private void OnClicked(int cardId)
    {
        clickedDeckCard = deckCards[cardId];
    }

    private void LoadMagicCard(int cardId)
    {

    }

    public void PickDeckCard()
    {
        if (isBanPick)
        {
            if (selectingBanCardCount >= selectingBanCardNum.Length) return;

            selectingBanCardNum[selectingBanCardCount] = clickedDeckCard.CardUniqueNumber;

            RectTransform deckCardTransform = Instantiate(clickedDeckCard).GetComponent<RectTransform>();

            deckCardTransform.SetParent(banPickPool,true);
            deckCardTransform.anchoredPosition = new Vector2(-150 + selectingCardCount * 80, 0);

            selectingBanCardCount++;
        }
        else
        {
            if (selectingCardCount >= selectingCardNum.Length) return;

            selectingCardNum[selectingCardCount] = clickedDeckCard.CardUniqueNumber;

            RectTransform deckCardTransform = Instantiate(clickedDeckCard).GetComponent<RectTransform>();

            deckCardTransform.SetParent(mySelectingCardPool, true);
            deckCardTransform.anchoredPosition = new Vector2(0, 100 - selectingCardCount * 80);

            selectingCardCount++;
        }
    }


    public int[] GetUsingCardNum() => selectingCardNum;
}
