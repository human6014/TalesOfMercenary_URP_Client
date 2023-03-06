using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataManager : MonoBehaviour
{
    [SerializeField] private BuildingCard [] buildingCard;
    [SerializeField] private Card [] magicCard;

    public BuildingCard[] GetBuildingCards() => buildingCard;

    public Card[] GetMagicCards() => magicCard;

    public BuildingCard GetBuildingCard(int num) => buildingCard[num];

    public Card GetMagicCard(int num) => magicCard[num];
}
