using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataManager : MonoBehaviour
{
    private const int buildingCardLength = 4;
    private const int magicCardLenght = 2;
    //위에 두개 다른곳에서도 선언되있었음
    //곂치니까 나중에 처리합시다

    [SerializeField] private GameObject[] buildingCardObj;
    [SerializeField] private GameObject[] magicCardObj;

    private BuildingCard[] buildingCard;
    private Card[] magicCard;
    private void Awake()
    {
        buildingCard = new BuildingCard[buildingCardObj.Length];
        magicCard = new Card[magicCardObj.Length];

        for(int i = 0; i < buildingCardObj.Length; i++)
        {
            buildingCard[i] = buildingCardObj[i].GetComponent<BuildingCard>();
        }

        for (int i = 0; i < magicCardObj.Length; i++)
        {
            magicCard[i] = magicCardObj[i].GetComponent<Card>();
        }
    }

    public BuildingCard[] GetBuildingCards() => buildingCard;

    public Card[] GetMagicCards() => magicCard;

    public BuildingCard GetBuildingCard(int num) => buildingCard[num];

    public Card GetMagicCard(int num) => magicCard[num];



    public GameObject GetBuildingCardObj(int num) => buildingCardObj[num];

    public GameObject[] GetBuildingCardObjs() => buildingCardObj;

    public GameObject GetMagicCardObj(int num) => magicCardObj[num];

    public GameObject[] GetMagicCardObjs() => magicCardObj;
}
