using Newtonsoft.Json;

public class OpponentData
{
    private const string UnitTypeKey = "unitType";
    private const string SpeedKey = "speed";
    private const string HealthKey = "health";


    [JsonProperty(UnitTypeKey)] public string UnitType { get; private set; }
    [JsonProperty(SpeedKey)] public float Speed { get; private set; }
    [JsonProperty(HealthKey)] public float Health { get; private set; }


    public OpponentData(string unitType, float speed, float health)
    {
        UnitType = unitType;
        Speed = speed;
        Health = health;
    }
}
