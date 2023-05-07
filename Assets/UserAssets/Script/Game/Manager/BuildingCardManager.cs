using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Tooltip("게임에 가져갈 덱에 해당하는 카드")] // 일단 다 넣어~
    [SerializeField] private BuildingCard[] deckCardPrefab;

    [Tooltip("게임에 가져갈 마법 카드")]
    [SerializeField] private Card[] magicCard;

    [Header("Pool")]
    [Tooltip("덱에 위치할 카드의 RectTransform")]
    [SerializeField] private RectTransform activeDeckPool;

    [Tooltip("넥서스 카드가 위치할 RectTransform")]
    [SerializeField] private RectTransform nexusCardPool;

    [Header("CardProbability")]
    [SerializeField] private CardProbability[] mCardProbability;
    #endregion

    private const int maxDeckCardNum = 2;
    private const float mTotal = 100;

    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] deckCards;
    //deckCards 0번 index = NexusCard


}
