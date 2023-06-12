using System.Collections.Generic;
using Newtonsoft.Json;

public static class JSONExtensions
{
    public static string Serialize(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static T Deserialize<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static Dictionary<string, string> Deserialize(this string json)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }
}
