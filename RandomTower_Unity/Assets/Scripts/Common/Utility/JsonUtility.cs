using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class JsonUtility
{
    private static readonly JsonSerializerSettings settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static string SerializeObject(object target)
    {
        return JsonConvert.SerializeObject(target, settings);
    }

    public static T DeserializeObject<T> (string json)
    {
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}
