using Riptide;
using System.Collections.Generic;
using UnityEngine;

public enum Team : byte
{
    none,
    green,
    orange
}

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new();

    public static Dictionary<ushort, BuildingCard[]> playerBuildings = new();

    public static Dictionary<ushort, BuildingCard> playerNexus = new();

    public ushort ClientID { get; private set; }
    public bool IsLocal { get; set; }
    private string Username { get; set; }

    public void Init(ushort clientID)
    {
        ClientID = clientID;
        playerBuildings[ClientID] = FindObjectOfType<TempUnitData>().GetDeckCardData();
        playerNexus[ClientID] = FindObjectOfType<TempUnitData>().GetNexusCardData();
    }

    private void OnDestroy()
    {
        list.Remove(ClientID);
    }
}
