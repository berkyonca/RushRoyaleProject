using System;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;

public class NakamaManager : MonoBehaviour
{
    private const string UD_ID_KEY = "udid";

    [SerializeField] private NakamaConnectionData connectionData = null;

    private IClient client = null;
    private ISession session = null;
    private ISocket socket = null;


    public event Action OnConnecting = null;
    public event Action OnConnected = null;
    public event Action OnDisconnected = null;
    public event Action OnLoginSuccess = null;
    public event Action OnLoginFail = null;

    public string Username { get => session == null ? string.Empty : session.Username; }
    public bool IsLoggedIn { get => socket != null && socket.IsConnected; }
    public ISocket Socket { get => socket; }
    public ISession Session { get => session; }
    public IClient Client { get => client; }

    public static NakamaManager Instance;

    private void Awake()
    {
        Instance = this;    
    }

    private void OnApplicationQuit()
    {
        if (socket != null)
            socket.CloseAsync();
    }

    public void LoginWithUdid()
    {
        var udid = PlayerPrefs.GetString(UD_ID_KEY, Guid.NewGuid().ToString());
        PlayerPrefs.SetString(UD_ID_KEY, udid);
        client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey, UnityWebRequestAdapter.Instance);
        LoginAsync(connectionData, client.AuthenticateDeviceAsync(udid));
    }

    public void LoginWithDevice()
    {
        client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey, UnityWebRequestAdapter.Instance);
        LoginAsync(connectionData, client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier));

    }

    public void LoginWithCustomId(string customId, string username)
    {
        client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey, UnityWebRequestAdapter.Instance);
        LoginAsync(connectionData, client.AuthenticateCustomAsync(customId, username));
    }

    private async void LoginAsync(NakamaConnectionData connectionData, Task<ISession> sessionTask)
    {
        OnConnecting?.Invoke();
        try
        {
            session = await sessionTask;
            socket = client.NewSocket(true);
            socket.Connected += Connected;
            socket.Closed += Disconnected;
            await socket.ConnectAsync(session);
            OnLoginSuccess?.Invoke();
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            OnLoginFail?.Invoke();
        }
    }

    public void LogOut()
    {
        socket.CloseAsync();
    }

    private void Connected()
    {
        OnConnected?.Invoke();
    }

    private void Disconnected()
    {
        OnDisconnected?.Invoke();
    }

    public async Task<IApiRpc> SendRPC(string rpc, string payload = "{}")
    {
        if (client == null || session == null)
            return null;

        return await client.RpcAsync(session, rpc, payload);
    }
}

