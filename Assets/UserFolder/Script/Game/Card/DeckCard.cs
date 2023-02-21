using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class DeckCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Card[] cards;
    [SerializeField] private int cardUpgradCost;
    [SerializeField] private int cardUniqueNumber;
    [SerializeField] private int cardLevel;

    private UnitJsonData unitJsonData;
    public UnityAction<int> OnPointerDownAction { get; set; }

    public int CardUpgradeCost
    {
        get => cardUpgradCost;
        private set => cardUpgradCost = value;
    }
    public int CardId { get; set; }
    public int CardLevel { get => cardLevel; set => cardLevel = value; }
    public int CardMaxLevel { get; set; }
    public int CardUniqueNumber { get => cardUniqueNumber; private set => cardUniqueNumber = value; }
    public string CardName { get; set; }
    

    private void Awake()
    {
        CardName = gameObject.name;
    }

    public void SetCardData(UnitJsonData _unitJsonData)
    {
        unitJsonData = _unitJsonData;
    }

    /// <summary>
    /// 가지고 있는 카드를 레벨별 일정 확률로 반환함
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomCard()
    {
        int rand = Random.Range(0, cards.Length);
        return cards[rand];
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
        OnPointerDownAction?.Invoke(CardId);
}
