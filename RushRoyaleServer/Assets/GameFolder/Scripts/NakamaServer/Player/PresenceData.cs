using Newtonsoft.Json;

public class PresenceData
{
    private const string SessionIdKey = "sessionId";

    [JsonProperty(SessionIdKey)] public string SessionId { get; private set; }

    public PresenceData(string sessionId)
    {
        SessionId = sessionId;
    }
}
