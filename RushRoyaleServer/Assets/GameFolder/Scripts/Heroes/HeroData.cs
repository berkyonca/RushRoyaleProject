using Newtonsoft.Json;

public class HeroData
{
    private const string UnitTypeKey = "unitType";
    private const string TargetKey = "target";
    private const string RangeKey = "range";
    private const string DamageKey = "damage";
    private const string AttackIntervalKey = "attackInterval";
    private const string SpecialAbilityKey = "specialAbility";


    [JsonProperty(UnitTypeKey)] public string UnitType { get; private set; }
    [JsonProperty(TargetKey)] public string Target { get; private set; }
    [JsonProperty(RangeKey)] public float Range { get; private set; }
    [JsonProperty(DamageKey)] public int Damage { get; private set; }
    [JsonProperty(AttackIntervalKey)] public float AttackInterval { get; private set; }
    [JsonProperty(SpecialAbilityKey)] public string SpecialAbility { get; private set; }


    public HeroData(string unitType, string target, float range, int damage, float attackInterval, string specialAbility)
    {
        UnitType = unitType;
        Target = target;
        Range = range;
        Damage = damage;
        AttackInterval = attackInterval;
        SpecialAbility = specialAbility;
    }
}
