using Newtonsoft.Json;

public class PlayerData
{
    private const string PresenceKey = "presence";
    private const string DisplayNameKey = "displayName";


    [JsonProperty(PresenceKey)] public PresenceData Presence { get; private set; }
    [JsonProperty(DisplayNameKey)] public string DisplayName { get; private set; }

    public PlayerData(PresenceData presence, string displayName)
    {
        Presence = presence;
        DisplayName = displayName;
    }
}
