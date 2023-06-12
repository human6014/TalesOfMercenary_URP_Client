using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MagicCard : Card, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public UnityAction<int> OnPointerDownSelectAction { get; set; }

    private bool mIsSelectingMode;
    private bool mIsBatchMode;

    public bool IsSelectingMode 
    { 
        get => mIsSelectingMode;
        set 
        {
            mIsSelectingMode = value;
            mIsBatchMode = !value;
        }
    }
    public bool IsBatchMode 
    { 
        get => mIsBatchMode;
        set
        {
            mIsBatchMode = value;
            mIsSelectingMode = !value;
        }
    }

    public override void InitializeData()
    {
        IsBatchMode = true;
        CardType = CardType.Magic;
        base.InitializeData();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (mIsBatchMode)
            OnDragAction?.Invoke(CardId, eventData.delta, CardType);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(mIsBatchMode)
            OnPointerUpAction?.Invoke(CardId, CardType);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (mIsBatchMode) OnPointerDownBatchAction?.Invoke(CardId, CardType);
        else if (mIsSelectingMode) OnPointerDownSelectAction?.Invoke(CardUniqueID);
    }
}
