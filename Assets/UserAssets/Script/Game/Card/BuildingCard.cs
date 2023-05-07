using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Photon.Pun;
using Scriptable;

public class BuildingCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Card[] cards;
    [SerializeField] private int cardUpgradCost;
    [SerializeField] private int cardUniqueNumber;
    [SerializeField] private int cardMaxLevel;
    private PhotonView mPhotonView;

    private string mID { get; }
    private int cardCurrentLevel;

    public UnityAction<int> OnPointerDownAction { get; set; }

    public int CardUpgradeCost
    {
        get => cardUpgradCost;
        private set => cardUpgradCost = value;
    }

    public int CardId { get; set; } // ÀÎµ¦½º ¹øÈ£(¹è¿­)
    public int CardCurrentLevel { get => cardCurrentLevel; set => cardCurrentLevel = value; }
    public int CardMaxLevel { get => cardMaxLevel; }
    public int CardUniqueNumber { get => cardUniqueNumber; } // °íÀ¯¹øÈ£
    public string CardName { get; set; }
   
    private void Awake()
    {
        CardName = gameObject.name;
        mPhotonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// ¾÷±×·¹ÀÌµå½Ã ¹Ì¸® °ñµå °¡°ÝÀ» È®ÀÎÇÏ°í »ç¿ë
    /// </summary>
    public void BuildingUpgarde()
    {
        cardCurrentLevel++;
        mPhotonView.RPC(nameof(BuildingUpgardeRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void BuildingUpgardeRPC()
    {
        cardCurrentLevel++;
    }

    //ÀûÀýÇÑ À§Ä¡¿¡ ÇÔ¼ö È£­ƒ Ãß°¡¿äÇÔ
    public void Init()
    {
        mPhotonView.RPC(nameof(InitRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void InitRPC()
    {
        NetworkUnitManager.enemyBuildingList.Add(this);
    }


    /// <summary>
    /// °¡Áö°í ÀÖ´Â Ä«µå¸¦ ·¹º§º° ÀÏÁ¤ È®·ü·Î ¹ÝÈ¯ÇÔ
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomCard()
    {
        int rand = Random.Range(0, cards.Length);
        return cards[rand];
    }

    public Card GetCard(int index) => cards[index];

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
        OnPointerDownAction?.Invoke(CardId);
}
