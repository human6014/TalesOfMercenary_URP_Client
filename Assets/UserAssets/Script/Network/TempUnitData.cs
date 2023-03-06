using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUnitData : MonoBehaviour
{
    [SerializeField] Unit[] unit;
    [SerializeField] BuildingCard[] deckCards;
    [SerializeField] BuildingCard nexusCard;
    public Unit[] GetUnitData() => unit;
    public BuildingCard[] GetDeckCardData() => deckCards;
    public BuildingCard GetNexusCardData() => nexusCard;

}
