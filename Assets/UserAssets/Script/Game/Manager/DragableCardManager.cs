using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    Unit = 0,
    Magic,
    Building
}
public class DragableCardManager : MonoBehaviour
{
    private const int maxUnitCardNum = 4; //패의 개수는 4로 가정
    private const int maxMagicCardNum = 2;

    #region Serialize Member
    [Header("Mouse Layer")]
    [Tooltip("마우스로 감지할 레이어")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Pool")]
    [Tooltip("패에 위치할 카드의 RectTransform")]
    [SerializeField] private RectTransform activeCardPool;

    [Tooltip("패에 올라가기전에 위치할 카드의 RectTransform")]
    [SerializeField] private RectTransform readyCardPool;

    [Tooltip("마법카드의 RectTransform")]
    [SerializeField] private RectTransform magicCardPool;

    [Tooltip("마우스로 유닛을 놓기 전 상태에 위치할 Transform")]
    [SerializeField] private Transform previewHolder;

    [Tooltip("필드에 올라간 유닛이 위치할 Transform")]
    [SerializeField] private Transform unitPool;

    [Tooltip("마법카드가 소환됐을 때 위치할 Transform")]
    [SerializeField] private Transform magicPool;

    [Tooltip("마법카드가 소환됐을 때 위치할 Transform -> Position")]
    [SerializeField] private Transform magicStartPos;
    #endregion

    private GameManager gameManager;
    private BuildingCardManager deckManager;
    private RectTransform backupCardTransform;
    private Card[] unitCards;
    private Card[] magicCards;

    private Vector2 startPos;

    private bool isActiveCard;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        deckManager = GetComponent<BuildingCardManager>();
        unitCards = new Card[maxUnitCardNum];
        magicCards = new Card[maxMagicCardNum];
    }


    private void Start()
    {
        LoadHandCard();
    }


    private void LoadHandCard()
    {
        StartCoroutine(AddUnitCard());
        for (int i = 0; i < maxUnitCardNum; i++)
        {
            StartCoroutine(ObserveCard(i));
            StartCoroutine(AddUnitCard());
        }

        for (int i = 0; i < maxMagicCardNum; i++)
        {
            StartCoroutine(AddMagicCard(i));
        }
    }

    #region 패 보충
    /*
    * 
    * %%%%%%%%%%%%%%%%%%%%%%%%% AddCard, CardObserve %%%%%%%%%%%%%%%%%%%%%%%%%%
    * %%%%%%%%%%%%%%%%%%%%%%%%%  delay잘못 건드리면  %%%%%%%%%%%%%%%%%%%%%%%%%%
    * %%%%%%%%%%%%%%%%%%%%%%%%%       버그 있음      %%%%%%%%%%%%%%%%%%%%%%%%%%
    * 
    * 
    * 
    * 
    *  딜레이 시간 0으로 할꺼면 AddCard, CardObserve 두 함수 코루틴 제거 가능
    */

    /// <summary>
    /// 패로 가져올 카드를 준비시킴
    /// </summary>
    /// <param name="delay">카드 드로우 시간</param>
    /// <returns></returns>
    // private IEnumerator AddCard(GameObject obj, float delay = 0f)
    private IEnumerator AddUnitCard(float delay = 0f)
    {
        //Debug.Log("AddCard");

        yield return new WaitForSeconds(delay);

        backupCardTransform = Instantiate(deckManager.GetRandomUnitCard()).GetComponent<RectTransform>();
        backupCardTransform.SetParent(readyCardPool, true);
        backupCardTransform.localScale = Vector3.one * 0.7f;
        backupCardTransform.anchoredPosition = new Vector2(0, 0);

        Card cardScript = backupCardTransform.GetComponent<Card>();
        cardScript.CardType = CardType.Unit;
        cardScript.InitializeData();
    }

    private IEnumerator AddMagicCard(int cardId, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        RectTransform backupCardTransform = Instantiate(deckManager.GetRandromMagicCard()).GetComponent<RectTransform>();
        backupCardTransform.SetParent(magicCardPool, true);
        backupCardTransform.anchoredPosition = new Vector2(-40 + cardId * 80, 0);

        Card cardScript = backupCardTransform.GetComponent<Card>();
        cardScript.InitializeData();

        cardScript.CardId = cardId;
        magicCards[cardId] = cardScript;
        cardScript.CardType = CardType.Magic;

        cardScript.OnPointerDownAction += CardTapped;
        cardScript.OnDragAction += CardDragged;
        cardScript.OnPointerUpAction += CardReleased;
    }
     
    /// <summary>
    /// AddCard에서 준비된 카드를 패로 가져옴
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1를 가지는 식별번호</param>
    /// <param name="delay">카드 드로우 시간</param>
    /// <returns></returns>
    private IEnumerator ObserveCard(int cardId, float delay = 0f)
    {
        //Debug.Log("CardObserve");

        yield return new WaitForSeconds(delay);

        backupCardTransform.SetParent(activeCardPool, true);
        backupCardTransform.localScale = Vector3.one;
        backupCardTransform.anchoredPosition = new Vector2(-150 + cardId * 100, 0);
        backupCardTransform.GetComponentInChildren<Text>().text = backupCardTransform.GetComponentInChildren<Text>().text + cardId;
        //임시

        Card cardScript = backupCardTransform.GetComponent<Card>();
        cardScript.CardId = cardId;
        unitCards[cardId] = cardScript;

        cardScript.OnPointerDownAction += CardTapped;
        cardScript.OnDragAction += CardDragged;
        cardScript.OnPointerUpAction += CardReleased;
    }
    #endregion

    #region 마우스 처리 담당

    /// <summary>
    /// 카드를 마우스로 클릭 시 발동함
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1를 가지는 식별번호</param>
    private void CardTapped(int cardId, CardType cardType)
    {
        //Debug.Log("CardTapped");

        RectTransform card = null;
        if (cardType == CardType.Unit) card = unitCards[cardId].GetComponent<RectTransform>();
        else if(cardType == CardType.Magic) card = magicCards[cardId].GetComponent<RectTransform>();
        else Debug.LogError("CardType Missing");

        card.SetAsLastSibling(); //UI순서에서 젤 위로 오게 하는겁니당
        startPos = card.anchoredPosition;
    }

    /// <summary>
    /// 카드 클릭 후 드래그 시 발동함
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1를 가지는 식별번호</param>
    /// <param name="dragAmount">eventData.delta값</param>
    private void CardDragged(int cardId, Vector2 dragAmount, CardType cardType)
    {
        //Debug.Log("CardDragged");

        Card usingCard = null;
        if (cardType == CardType.Unit) usingCard = unitCards[cardId];
        else if (cardType == CardType.Magic) usingCard = magicCards[cardId];
        else Debug.LogError("CardType Missing");

        usingCard.transform.Translate(dragAmount);

        if (IsHitToGround(out RaycastHit hit))
        {
            if (!isActiveCard)
            {
                isActiveCard = true;
                previewHolder.position = hit.point;
                usingCard.ChangeActiveState(true);

                Instantiate(usingCard.CardPrefab, hit.point, Quaternion.identity, previewHolder);
            }
            else
            {
                Vector3 cardPos = hit.point;
                if (cardType == CardType.Unit) cardPos = hit.collider.transform.position + Vector3.up * 0.6f;

                previewHolder.position = cardPos;
            }
        }
        else
        {
            if (isActiveCard)
            {
                isActiveCard = false;
                usingCard.ChangeActiveState(false);

                ClearPreviewObject();
            }
        }
    }

    /// <summary>
    /// 카드를 클릭하고 땟을 경우 발동함
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1를 가지는 식별번호</param>
    private void CardReleased(int cardId, CardType cardType)
    {
        //Debug.Log("CardReleased");

        Card usingCard = null;
        if (cardType == CardType.Unit) usingCard = unitCards[cardId];
        else if (cardType == CardType.Magic) usingCard = magicCards[cardId];
        else Debug.LogError("CardType Missing");

        if (IsHitToGround(out RaycastHit hit))
        {
            ClearPreviewObject();

            if (!gameManager.DoValidGold(usingCard.CardCost))
            {
                usingCard.GetComponent<RectTransform>().anchoredPosition = startPos;
                usingCard.ChangeActiveState(false);
                return;
            }

            Destroy(usingCard.gameObject);

            SpawnUnit(cardId, cardType, hit, usingCard, true);

        }
        else usingCard.GetComponent<RectTransform>().anchoredPosition = startPos;
    }



    /// <summary>
    /// 카드로부터 유닛 소환함
    /// </summary>
    /// <param name="cardId">카드번호(RectTransform번호)</param>
    /// <param name="cardType">카드 종류</param>
    /// <param name="hit">지상 Ray정보</param>
    /// <param name="usingCard">현재 소환하고자 하는 카드의 Card객체</param>
    /// <param name="isPlayer">네트워크상 본인의 카드인지</param>
    public void SpawnUnit(int cardId, CardType cardType, RaycastHit hit, Card usingCard, bool isPlayer)
    {
        GameObject obj = null;
        if (cardType == CardType.Unit)
        {
            //Vector3 cardPos = hit.collider.transform.position + Vector3.up * 0.2f;
            //obj = Instantiate(usingCard.CardPrefab, cardPos, Quaternion.identity);
            //obj.GetComponent<Unit>().InitBatch(1);
            //obj.transform.SetParent(unitPool);

            //NetworkUnitManager.SendUnitSpawn(0, hit.point);
            //Invoke(nameof(SendPlayerDrawCard),0.5f);
            StartCoroutine(ObserveCard(cardId));
            StartCoroutine(AddUnitCard());
        }
        else if (cardType == CardType.Magic)
        {
            obj = Instantiate(usingCard.CardPrefab, magicStartPos.position, Quaternion.identity);
            obj.GetComponent<Magic>().Init(hit.point, isPlayer);
            obj.transform.SetParent(magicPool);

            StartCoroutine(AddMagicCard(cardId));
        }
    }

    private void SendPlayerDrawCard()
    {
        NetworkUnitManager.SendPlayerDrawCard();
    }
    /// <summary>
    /// 현재 스크린 좌표계의 마우스 위치에서 월드 좌표계로의 레이에서 GroundLayer에 감지되는지 알아냄
    /// </summary>
    /// <param name="hit">out RaycastHit hit</param>
    /// <returns>Ground 레이어와 충돌 o : true, 충돌 x : false</returns>
    private bool IsHitToGround(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer);
    }

    /// <summary>
    /// previewHolder의 자식 오브젝트 모두 제거
    /// </summary>
    private void ClearPreviewObject()
    {
        for (int i = 0; i < previewHolder.childCount; i++)
            Destroy(previewHolder.GetChild(i).gameObject);
    }

    #endregion
}
