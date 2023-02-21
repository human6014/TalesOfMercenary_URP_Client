using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataManager : MonoBehaviour
{
    [SerializeField] private DeckCard [] buildingCard;
    [SerializeField] private Card [] magicCard;

    public DeckCard[] GetBuildingCards() => buildingCard;

    public Card[] GetMagicCards() => magicCard;

    public DeckCard GetBuildingCard(int num) => buildingCard[num];

    public Card GetMagicCard(int num) => magicCard[num];
}
