using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MScriptableObject : ScriptableObject
{
    public T Clone<T>() where T : MScriptableObject
    {
        T newIns = InstanceNew<T>();
        string json = JsonUtility.ToJson(this);
        Debug.Log("Json string " + json);
        JsonUtility.FromJsonOverwrite(json, newIns);
        newIns = DeepCloneChild(newIns);
        return newIns;
    }

    public MScriptableObject DeepCloneChild<MScriptableObject>(MScriptableObject obj)
    {
        SerializationUtility.SerializeValue<
        var props = obj.GetType().GetProperties();
        foreach (var prop in props)
        {
            object value = prop.GetValue(obj, null);
            if (value != null)
            {
                Debug.Log("Json string test prop " + value.GetType());
                if (value.GetType().IsSubclassOf(typeof(MScriptableObject)))
                {
                    Debug.Log("Json string detect prop");
                    MScriptableObject scriptableObject = (MScriptableObject)value;
                    prop.SetValue(obj, scriptableObject.GetType().GetMethod("Clone").Invoke(scriptableObject, null));

                }
                else if (prop.PropertyType.IsArray)
                {
                    List<object> newList = new List<object>();
                    Debug.Log("Json string detect array");
                    foreach (object item in (Array)value)
                    {
                        if (item.GetType().IsSubclassOf(typeof(MScriptableObject)))
                        {
                            MScriptableObject scriptableObject = (MScriptableObject)item;
                            newList.Add(item.GetType().GetMethod("Clone").Invoke(scriptableObject, null));
                        }
                        else
                        {
                            newList.Add(item);
                        }
                    }
                    prop.SetValue(obj, newList.ToArray());
                }
            }

        }
        return obj;
    }

    public T InstanceNew<T>() where T : MScriptableObject
    {
        return (T)ScriptableObject.CreateInstance(typeof(T));
    }
}
