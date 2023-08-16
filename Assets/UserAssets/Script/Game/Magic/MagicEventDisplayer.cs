using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicEventDisplayer : MonoBehaviour
{
    [SerializeField] private DragableCardManager mDragableCardManager;
    [SerializeField] private MagicCard[] mMagicCards;
    [SerializeField] private RectTransform mUIPos;
    [SerializeField] private int mBatchNum;

    public void Init()
    {

    }

    [ContextMenu("DisplayMagicEvent")]
    public void DisplayMagicEvent()
    {
        gameObject.SetActive(true);
        RectTransform rectTransform;
        MagicCard magicCard;
        for (int i = 0; i < mBatchNum; i++)
        {
            magicCard = Instantiate(mMagicCards[i]);
            magicCard.IsSelectingMode = true;

            rectTransform = magicCard.GetComponent<RectTransform>();
            rectTransform.SetParent(mUIPos);
            rectTransform.SetAsLastSibling();

            magicCard.OnPointerDownSelectAction += ProcessSelect;
        }
    }

    private void ProcessSelect(int cardUniqueID)
    {
        mDragableCardManager.LoadMagicCard(mMagicCards[cardUniqueID]);
        EndMagicEvent();
    }

    private void EndMagicEvent()
    {
        //이미지, 텍스트만 바뀌면 재사용
        //변화가 크게 있을 경우 견본 미리 생성 후 따로 적용
        //그 외는 생성 + 삭제
        gameObject.SetActive(false);
    }
}
