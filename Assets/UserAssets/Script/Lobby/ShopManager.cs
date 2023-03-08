using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private RectTransform randomPickCheckingPanel;
    private CardDataManager cardDataManager;
    private readonly float[] probs = new float[] { 25, 25, 25, 25 };
    private float total = 0;

    private RectTransform currentPickedBuildingCard;
    private void Awake()
    {
        foreach (float elem in probs) total += elem;
        cardDataManager = GetComponent<CardDataManager>();
    }

    private int RandomBuildingCardIndex()
    {
        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i]) return i;
            else randomPoint -= probs[i];
        }
        return probs.Length - 1;
    }

    public void PickRandomBuildingCard()
    {
        int randomIndex = RandomBuildingCardIndex();

        if (currentPickedBuildingCard != null) Destroy(currentPickedBuildingCard.gameObject); //»ø¿≤ æ»¡¡¿Ω -> πŸ≤‹≤®¿”
        currentPickedBuildingCard = Instantiate(cardDataManager.GetBuildingCardObj(randomIndex)).GetComponent<RectTransform>();

        currentPickedBuildingCard.SetParent(randomPickCheckingPanel);
        currentPickedBuildingCard.anchoredPosition = new Vector2(0, 60);
        currentPickedBuildingCard.SetAsLastSibling();
    }
}
