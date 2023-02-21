using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUnitData : MonoBehaviour
{
    [SerializeField] Unit[] unit;
    [SerializeField] DeckCard[] deckCards;
    [SerializeField] DeckCard nexusCard;
    public Unit[] GetUnitData() => unit;
    public DeckCard[] GetDeckCardData() => deckCards;
    public DeckCard GetNexusCardData() => nexusCard;

}
