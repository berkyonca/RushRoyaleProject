using System;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using Newtonsoft.Json;

public partial class MultiplayerManager : MonoBehaviour
{
    private const int TickRate = 5;
    private const float SendRate = 1f / (float)TickRate;
    private const string JoinMatchRpc = "JoinMatchRpc";
    private const string CreateMatchRpc = "CreateMatchRpc";
    private const string FindMatchRpc = "FindMatchRpc";
    private const string LogFormat = "{0} with code {1}:\n{2}";
    private const string SendingDataLog = "Sending data";
    private const string ReceivedDataLog = "Received data";
    private const string Query = "+skill:>100 mode:sabotage";

    [SerializeField] private bool enableLog = false;

    private Dictionary<Code, Action<MultiplayerMessage>> onReceiveData = new Dictionary<Code, Action<MultiplayerMessage>>();
    private IMatch match = null;

    public event Action onMatchJoin = null;
    public event Action onMatchLeave = null;
    public event Action onLocalTick = null;

    public static MultiplayerManager Instance;

    public IUserPresence Self { get => match == null ? null : match.Self; }
    public IMatch Match { get => match; }
    public bool IsOnMatch { get => match != null; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(LocalTickPassed), SendRate, SendRate);
    }

    private void LocalTickPassed()
    {
        onLocalTick?.Invoke();
    }

    public async void JoinMatchAsync(int matchNumber, string lobbyName, string password)
    {
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
        NakamaManager.Instance.OnDisconnected += Disconnected;

        var matchStatus = new Dictionary<string, string> { { "lobbyName", lobbyName }, { "password", password }, { "matchNumber", matchNumber.ToString() } };
        var context = JsonConvert.SerializeObject(matchStatus);
        IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinMatchRpc, context);
        string matchId = rpcResult.Payload;

        //var weaponData = new Dictionary<string, string> { { "collection", "Ýtems" }, { "key", "Weapon" } };
        //var data = JsonConvert.SerializeObject(weaponData);
        //await NakamaManager.Instance.SendRPC("WriteWeaponData", data);

        if (matchId == "")
        {
            Debug.Log("There is no valid match!");
            return;
        }
        match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId, new Dictionary<string, string> { { "lobbyName", lobbyName }, { "password", password }, { "matchNumber", matchNumber.ToString() } });
        Debug.Log("Joined match!");

        var moneyData = new Dictionary<string, string> { { "userId", NakamaManager.Instance.Username }, { "money", "500" } };
        var money = JsonConvert.SerializeObject(moneyData);
        var collectionData = new Dictionary<string, string> { { "collection", "Items" }, { "key", "Money" }, { "value", money } };
        var data = JsonConvert.SerializeObject(collectionData);
        await NakamaManager.Instance.SendRPC("WriteMoneyData", data);
        onMatchJoin?.Invoke();
    }

    public async Task CreateMatchAsync(string lobbyName, string password)
    {
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
        NakamaManager.Instance.OnDisconnected += Disconnected;

        var matchStatus = new Dictionary<string, string> { { "password", password }, { "lobbyName", lobbyName } };
        var context = JsonConvert.SerializeObject(matchStatus);
        IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(CreateMatchRpc, context);
        string matchId = rpcResult.Payload;

        //var weaponData = new Dictionary<string, WeaponData> { { "AK47", new WeaponData("AK47", 15, 0.25f) } };
        //var weapon = JsonConvert.SerializeObject(weaponData);
        //var collectionData = new Dictionary<string, string> { { "collection", "Items" }, { "key", "Weapon" }, { "value", weapon } };
        //var data = JsonConvert.SerializeObject(collectionData);
        //await NakamaManager.Instance.SendRPC("writeWeaponData", data);

        if (matchId == "")
        {
            Debug.Log("There is no valid match!");
            return;
        }
        match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId, new Dictionary<string, string> { { "password", password }, { "lobbyName", lobbyName } });
        Debug.Log("Match created!");

        var moneyData = new Dictionary<string, string> { { "userId", NakamaManager.Instance.Username }, { "money", "500" } };
        var money = JsonConvert.SerializeObject(moneyData);
        var collectionData = new Dictionary<string, string> { { "collection", "Items" }, { "key", "Money" }, { "value", money } };
        var data = JsonConvert.SerializeObject(collectionData);
        await NakamaManager.Instance.SendRPC("WriteMoneyData", data);
        onMatchJoin?.Invoke();
    }

    public async void FindMatchAsync()
    {
        var minPlayers = 2;
        var maxPlayers = 2;
        var query = "";

        await NakamaManager.Instance.Socket.AddMatchmakerAsync(query, minPlayers, maxPlayers);
    }

    public async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
        NakamaManager.Instance.OnDisconnected += Disconnected;

        match = await NakamaManager.Instance.Socket.JoinMatchAsync(matched);
        if (match != null)
            onMatchJoin?.Invoke();
    }

    private void Disconnected()
    {
        NakamaManager.Instance.OnDisconnected -= Disconnected;
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        match = null;
        onMatchLeave?.Invoke();
    }

    public async void LeaveMatchAsync()
    {
        NakamaManager.Instance.Socket.ReceivedMatchmakerMatched += async matchmakerMatched =>
        {
            match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchmakerMatched);
        };

        NakamaManager.Instance.OnDisconnected -= Disconnected;
        NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
        await NakamaManager.Instance.Socket.LeaveMatchAsync(match);
        match = null;
        onMatchLeave?.Invoke();
    }

    public void Send(Code code, object data = null)
    {
        if (match == null)
            return;

        string json = data != null ? data.Serialize() : string.Empty;
        if (enableLog)
            LogData(SendingDataLog, (long)code, json);

        NakamaManager.Instance.Socket.SendMatchStateAsync(match.Id, (long)code, json);
    }

    public void Send(Code code, byte[] bytes)
    {
        if (match == null)
            return;

        if (enableLog)
            LogData(SendingDataLog, (long)code, String.Empty);

        NakamaManager.Instance.Socket.SendMatchStateAsync(match.Id, (long)code, bytes);
    }

    private void Receive(IMatchState newState)
    {
        if (enableLog)
        {
            var encoding = System.Text.Encoding.UTF8;
            var json = encoding.GetString(newState.State);
            LogData(ReceivedDataLog, newState.OpCode, json);
        }

        MultiplayerMessage multiplayerMessage = new MultiplayerMessage(newState);
        if (onReceiveData.ContainsKey(multiplayerMessage.DataCode))
            onReceiveData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
    }

    public void Subscribe(Code code, Action<MultiplayerMessage> action)
    {
        if (!onReceiveData.ContainsKey(code))
            onReceiveData.Add(code, null);

        onReceiveData[code] += action;
    }

    public void Unsubscribe(Code code, Action<MultiplayerMessage> action)
    {
        if (onReceiveData.ContainsKey(code))
            onReceiveData[code] -= action;
    }

    private void LogData(string description, long dataCode, string json)
    {
        Debug.Log(string.Format(LogFormat, description, (Code)dataCode, json));
    }
}
