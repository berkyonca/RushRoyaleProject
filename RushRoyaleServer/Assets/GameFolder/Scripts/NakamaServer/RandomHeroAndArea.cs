using Newtonsoft.Json;

public class RandomHeroAndArea
{
    private const string RandomAreaKey = "randomArea";
    private const string RandomHeroKey = "randomHero";


    [JsonProperty(RandomAreaKey)] public int RandomArea { get; private set; }
    [JsonProperty(RandomHeroKey)] public int RandomHero { get; private set; }


    public RandomHeroAndArea(int randomArea, int randomHero)
    {
        RandomArea = randomArea;
        RandomHero = randomHero;
    }
}
