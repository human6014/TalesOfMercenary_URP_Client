using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MagicCard : Card, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public override void InitializeData()
    {
        //¹¹ÇÏÁö
        cardType = CardType.Magic;
        base.InitializeData();
    }

    void IDragHandler.OnDrag(PointerEventData eventData) =>
        OnDragAction?.Invoke(CardId, eventData.delta, CardType);

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) =>
        OnPointerUpAction?.Invoke(CardId, CardType);

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
        OnPointerDownAction?.Invoke(CardId, CardType);
}
