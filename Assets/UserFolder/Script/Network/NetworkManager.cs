using Riptide;
using Riptide.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ServerToClientId : ushort
{
    sync = 1,
    activeScene,
    //
    playerJoined,
    playerDrawedCard,
    playerBuildingUpgraded,
    playerSpawnedUnit,
    playerNexusUpgraded,
    unitAttack,
    unitMovement,
    unitDied,
    playerUseMagicCard,
    GameEnd,
    GameStart,
    unitTrackMovemnt
}

public enum ClientToServerId : ushort
{
    name = 1,
    spawnUnit,
    playerDrawedCard,
    nexusUpgraded,
    unitDestinationInput,
    useMagicCard,
    buildingUpgrad,
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager NetworkManagerSingleton { get; private set; }

    public Client Client { get; private set; }

    private ushort serverTick;
    public ushort ServerTick
    {
        get => serverTick;
        private set
        {
            serverTick = value;
            InterpolationTick = (ushort)(value - TicksBetweenPositionUpdates);
        }
    }


    private ushort ticksBetweenPositionUpdates = 2;
    public ushort TicksBetweenPositionUpdates
    {
        get => ticksBetweenPositionUpdates;
        private set
        {
            ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(ServerTick - value);
        }
    }
    public ushort InterpolationTick { get; private set; }
    public bool IsReversed { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;
    [SerializeField] private Player playerPrefab;

    [Space(10)]
    [SerializeField] private ushort tickDivergenceTolerance = 1;

    [Space(12)]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera reversedCamera;
    [SerializeField] private MouseController mouseController;


    private void Awake()
    {
        if (NetworkManagerSingleton == null)
        {
            NetworkManagerSingleton = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);

    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connect("127.0.0.1:8888");
        Client.Connected += DidConnect;
        //Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        //Client.Disconnected += DidDisconnect;

        ServerTick = TicksBetweenPositionUpdates;
    }

    private void FixedUpdate()
    {
        Client.Update();
        ServerTick++;
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.SendName();
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.AddString("Hyup");
        NetworkManager.NetworkManagerSingleton.Client.Send(message);

        //Debug.Log(Client.Id);
        Instantiate(playerPrefab).Init(Client.Id);

        //이 순간 아이디 정해짐
        IsReversed = Client.Id != 1;

        mainCamera.enabled = !IsReversed;
        reversedCamera.enabled = IsReversed;
        mouseController.SetPlayerMouseLayer(IsReversed);
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
            Destroy(player.gameObject);
    }


    private void SetTick(ushort serverTick)
    {
        if (Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance)
        {
            //Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerJoined)]
    private static void PlayerJoined(Message message)
    {
        ushort id = message.GetUShort();
        Player player = Instantiate(NetworkManagerSingleton.playerPrefab);
        if (id == NetworkManagerSingleton.Client.Id)
            player.IsLocal = true;
        else
            player.IsLocal = false;

        Player.list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.sync)]
    private static void Sync(Message message)
    {
        //Debug.LogFormat("Recivesync()");
        NetworkManagerSingleton.SetTick(message.GetUShort());
    }
}
