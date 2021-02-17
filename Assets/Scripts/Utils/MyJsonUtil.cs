using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class MyJsonUtil
{
    public static string SerializeScriptableObject(object data)
    {
        return JsonConvert.SerializeObject(data, GetSettingsScriptableObject());
    }


    public static T DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T DeserializeScriptableObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GetSettingsScriptableObject());
    }

    public static JsonSerializerSettings GetSettingsScriptableObject()
    {
        return new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnoreGuidsResolver()
        };
    }
}
class IgnoreGuidsResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);
        if ((prop.PropertyType.IsSubclassOf(typeof(GameObject)) || prop.PropertyType == typeof(GameObject)
        || prop.PropertyType.Namespace == "UnityEngine") && !prop.PropertyType.IsSubclassOf(typeof(ScriptableObject)))
        {
            prop.Ignored = true;
        }
        return prop;
    }
}