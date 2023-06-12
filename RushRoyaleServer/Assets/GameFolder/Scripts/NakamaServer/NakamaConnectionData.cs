using UnityEngine;

[CreateAssetMenu(menuName = "Nakama/Connection data")]
public class NakamaConnectionData : ScriptableObject
{
    [SerializeField] private string scheme = null;
    [SerializeField] private string host = null;
    [SerializeField] private int port = default(int);
    [SerializeField] private string serverKey = null;

    public string Scheme { get => scheme; }
    public string Host { get => host; }
    public int Port { get => port; }
    public string ServerKey { get => serverKey; }

}
