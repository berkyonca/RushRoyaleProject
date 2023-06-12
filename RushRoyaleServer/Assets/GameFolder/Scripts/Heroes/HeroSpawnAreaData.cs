using Newtonsoft.Json;

public class HeroSpawnAreaData
{
    private const string IsSpawnableKey = "isSpawnable";
    private const string ListNumberKey = "listNumber";


    [JsonProperty(IsSpawnableKey)] public bool IsSpawnable { get; private set; }
    [JsonProperty(ListNumberKey)] public int ListNumber { get; private set; }


    public HeroSpawnAreaData(bool isSpawnable, int listNumber)
    {
        IsSpawnable = isSpawnable;
        ListNumber = listNumber;
    }
}
