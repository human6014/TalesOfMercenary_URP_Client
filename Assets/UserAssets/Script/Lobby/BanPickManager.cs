using UnityEngine;

public class BanPickManager : MonoBehaviour
{
    private bool isBanPick;
    private bool isMyTurn;
    private bool isEnableOnce;

    [SerializeField] private CardDataManager cardDataManager;
    [SerializeField] private RectTransform cardPool;
    [SerializeField] private RectTransform mySelectingCardPool;
    [SerializeField] private RectTransform opponentSelectingCardPool;
    [SerializeField] private RectTransform banPickPool;

    private readonly int[] selectingCardNum = new int[2];
    private readonly int[] selectingBanCardNum = new int[2] { 2, 3 };

    private BuildingCard clickedDeckCard;
    private BuildingCard[] buildingCards = new BuildingCard[4];
    private Card[] magicCards = new Card[2];

    private int selectingCardCount = 0;
    private int selectingBanCardCount = 0;

    private const int rowMax = 4;

    private void Awake()
    {

    }
    private void Start()
    {
        //buildingCards = cardDataManager.GetBuildingCards();
        //magicCards = cardDataManager.GetMagicCards();

    }


    public void SetCardList()
    {
        foreach(var iter in mySelectingCardPool.GetComponentsInChildren<RectTransform>())
        {
            if (iter != mySelectingCardPool) Destroy(iter.gameObject);
        }

        foreach (var iter in opponentSelectingCardPool.GetComponentsInChildren<RectTransform>())
        {
            if (iter != opponentSelectingCardPool) Destroy(iter.gameObject);
        }

        foreach (var iter in banPickPool.GetComponentsInChildren<RectTransform>())
        {
            if (iter != banPickPool) Destroy(iter.gameObject);
        }
        clickedDeckCard = null;
        selectingCardCount = 0;
        selectingBanCardCount = 0;

        if (isEnableOnce) return;
        isEnableOnce = true;
        for (int i = 0; i < buildingCards.Length; i++)
        {
            LoadDeckCards(i);
        }
        for (int i = 0; i < magicCards.Length; i++)
        {
            LoadMagicCard(i);
        }
    }

    private void LoadDeckCards(int cardId)
    {
        RectTransform deckCardTransform = Instantiate(cardDataManager.GetBuildingCardObj(cardId)).GetComponent<RectTransform>();

        int rowPos = (-150 + (cardId % rowMax) * 100);
        int colPos = 80 + (cardId / rowMax * -80);

        deckCardTransform.SetParent(cardPool, true);
        deckCardTransform.anchoredPosition = new Vector2(rowPos, colPos);
        deckCardTransform.SetAsLastSibling();
        BuildingCard deckCard = deckCardTransform.GetComponent<BuildingCard>();

        deckCard.CardId = cardId;
        deckCard.CardCurrentLevel = 1;
        deckCard.OnPointerDownAction += OnClicked;
        buildingCards[cardId] = deckCard;
    }

    private void OnClicked(int cardId)
    {
        clickedDeckCard = buildingCards[cardId];
    }

    private void LoadMagicCard(int cardId)
    {

    }

    public void PickBuildingCard()
    {
        if (clickedDeckCard == null) return;
        if (isBanPick)
        {
            if (selectingBanCardCount >= selectingBanCardNum.Length) return;

            selectingBanCardNum[selectingBanCardCount] = clickedDeckCard.CardUniqueNumber;

            RectTransform deckCardTransform = Instantiate(clickedDeckCard).GetComponent<RectTransform>();

            deckCardTransform.SetParent(banPickPool, true);
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
