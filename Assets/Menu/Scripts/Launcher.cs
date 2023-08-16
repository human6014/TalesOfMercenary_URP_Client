using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Net;


public class Launcher : MonoBehaviourPunCallbacks
{
    //WebRequest webRequest = new WebRequest();
    public static Launcher Instance;

    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] TMP_InputField playerPasswordInputField;
    [SerializeField] TMP_Text titleWelcomeText;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Text errorText;

    int pwin;
    int plose;
    int prank;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting to master...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master!");
        PhotonNetwork.JoinLobby();
        // Automatically load scene for all clients when the host loads a scene
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.NickName == "")
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString(); // Set a default nickname, just as a backup
            MenuManager.Instance.OpenMenu("name");
        }
        else MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined lobby");
    }

    public void ReceivePlayerData(int rank, int win, int lose)
    {
        prank = rank;
        pwin = win;
        plose = lose;
    }
    public async void SetName()
    {
        PlayerData pdata = new PlayerData();
        string name = playerNameInputField.text;
        string ppass = playerPasswordInputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            if(!string.IsNullOrEmpty(ppass))
            {
                await WebRequest.RequestPostSignUp(name, ppass);
                //await WebRequest.RequestPostLogin(name, ppass);
                StartCoroutine(WebRequest.RequestGetInfo(name));                
                PhotonNetwork.NickName = name;

                titleWelcomeText.text = $"{name},{prank}, {pwin}, {plose}!";

                MenuManager.Instance.OpenMenu("title");
                playerNameInputField.text = "";
            }
                //Web≈ÎΩ≈
                //bool isSuccess = await WebRequest.RequestGetInfo(name);
                //if(!isSuccess) await WebRequest.RequestPostSignUp(name);
                //await WebRequest.RequestPostLogin(name);         
        }
        else Debug.Log("No player name, password entered");
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
            roomNameInputField.text = "";
        }
        else Debug.Log("No room name entered");
    }

    public override void OnJoinedRoom()
    {
        // Called whenever you create or join a room
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform trans in playerListContent)
            Destroy(trans.gameObject);
        
        for (int i = 0; i < players.Count(); i++)
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        // Only enable the start button if the player is the host of the room
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
            Destroy(trans.gameObject);
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        // 1 is used as the build index of the game scene, defined in the build settings
        // Use this instead of scene management so that *everyone* in the lobby goes into this scene
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
