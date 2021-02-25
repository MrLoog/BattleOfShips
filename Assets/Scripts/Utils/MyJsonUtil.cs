using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class MyJsonUtil
{
    public static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = CustomContractResolver.Instance
    };
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
        // T result = JsonConvert.DeserializeObject<T>(json, GetSettingsScriptableObject());
        return JsonConvert.DeserializeObject<T>(json, GetSettingsScriptableObject());
    }


    public static JsonSerializerSettings GetSettingsScriptableObject()
    {
        return settings;
    }
}
class CustomContractResolver : DefaultContractResolver
{
    static CustomContractResolver instance;
    static CustomContractResolver() { instance = new CustomContractResolver(); }
    public static CustomContractResolver Instance { get { return instance; } }
    class MScriptableObjectValueProvider : ValueProviderDecorator
    {
        private string propName;
        private Type valueType;
        public MScriptableObjectValueProvider(IValueProvider baseProvider, string propName, Type valueType) : base(baseProvider)
        {
            this.propName = propName;
            this.valueType = valueType;
        }

        public override void SetValue(object target, object value)
        {
            Debug.Log("Contract CreateProperty SetValue " + target.GetType().ToString() + ":" + propName + ":" + value);
            // if (target.GetType() == typeof(MScriptableObject) || target.GetType().IsSubclassOf(typeof(MScriptableObject)))
            // {

            // }

            base.SetValue(target, value);
        }
    }
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);
        Debug.Log("Contract CreateProperty" + prop.PropertyName);
        if ((prop.PropertyType.IsSubclassOf(typeof(GameObject)) || prop.PropertyType == typeof(GameObject)
        || prop.PropertyType.Namespace == "UnityEngine") && !prop.PropertyType.IsSubclassOf(typeof(ScriptableObject)))
        {
            prop.Ignored = true;
        }
        return prop;
    }


}
public abstract class ValueProviderDecorator : IValueProvider
{
    readonly IValueProvider baseProvider;

    public ValueProviderDecorator(IValueProvider baseProvider)
    {
        if (baseProvider == null)
            throw new ArgumentNullException();
        this.baseProvider = baseProvider;
    }

    public virtual object GetValue(object target) { return baseProvider.GetValue(target); }

    public virtual void SetValue(object target, object value) { baseProvider.SetValue(target, value); }
}