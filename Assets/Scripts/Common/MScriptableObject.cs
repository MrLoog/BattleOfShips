using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using UnityEngine;
public abstract class MScriptableObject : ScriptableObject
{
    public T Clone<T>() where T : MScriptableObject
    {
        T newIns = InstanceNew<T>();
        // string json = JsonUtility.ToJson(this);
        string jsonValue = MyJsonUtil.SerializeScriptableObject(this); //json for copy value
        string jsonDefaultRef = JsonUtility.ToJson(this); //json for copy default gameobject/prefab
        // Debug.Log("Json string before " + jsonDefaultRef);
        JsonUtility.FromJsonOverwrite(jsonDefaultRef, newIns);
        // Debug.Log("Json string before " + jsonDefaultRef);
        // JsonUtility.FromJsonOverwrite(json, newIns);
        object convert = MyJsonUtil.DeserializeObject<T>(jsonValue);
        DeepOverrideFields(newIns, convert);
        // T newIns = JsonConvert.DeserializeObject<T>(json);
        // newIns = DeepCloneChild(newIns);
        jsonValue = JsonUtility.ToJson(newIns);
        // Debug.Log("Json string after " + jsonValue);
        return (T)newIns;
    }


    public T Restore<T>() where T : MScriptableObject
    {
        T newIns = InstanceNew<T>();
        // string json = JsonUtility.ToJson(this);
        string jsonValue = MyJsonUtil.SerializeScriptableObject(this); //json for copy value

        T defaultData = Resources.LoadAll<T>("ScriptableObjects").Where(x => x.name == this.name).FirstOrDefault();
        if (defaultData == null) defaultData = (T)this;
        else
        {
            //prevent change affect original scriable object
            // defaultData = defaultData.Clone<T>();
        }
        // Debug.Log("Restore Mode found default");
        string jsonDefaultRef = JsonUtility.ToJson(defaultData); //json for copy default gameobject/prefab
                                                                 // Debug.Log("Json string before " + jsonDefaultRef);
        JsonUtility.FromJsonOverwrite(jsonDefaultRef, newIns);
        // Debug.Log("Json string before " + jsonDefaultRef);
        // JsonUtility.FromJsonOverwrite(json, newIns);
        object convert = MyJsonUtil.DeserializeObject<T>(jsonValue);
        DeepOverrideFields(newIns, convert);
        // T newIns = JsonConvert.DeserializeObject<T>(json);
        // newIns = DeepCloneChild(newIns);
        jsonValue = JsonUtility.ToJson(newIns);
        // Debug.Log("Json string after " + jsonValue);
        return (T)newIns;
    }

    private void DeepOverrideFields(object target, object source)
    {
        Debug.Log("DeepOverride 0");
        var props = target.GetType().GetProperties();
        var fields = target.GetType().GetFields().ToList();
        var field2s = target.GetType().BaseType?.GetFields();
        if (field2s.Length > 0) fields.AddRange(field2s);
        if (target.GetType() != source.GetType()) return;
        if (target.GetType().IsSubclassOf(typeof(ScriptableObject)) || target.GetType() == typeof(ScriptableObject))
        {
            //special property name
            PropertyInfo propName = target.GetType().GetProperty("name");
            propName.SetValue(target, propName.GetValue(source));
        }
        foreach (var prop in fields)
        {
            object value1 = prop.GetValue(target);
            object value2 = prop.GetValue(source);
            Debug.Log("DeepOverride 1" + prop.Name + " " + prop.FieldType + ":" + value1 + "/" + value2);
            // Debug.Log("DeepOverride " + prop.FieldType.IsValueType);

            if ((prop.FieldType.IsSubclassOf(typeof(GameObject))
            || prop.FieldType == typeof(GameObject) || prop.FieldType.Namespace == "UnityEngine")
            && !prop.FieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.Log("DeepOverride 1" + prop.Name);
                continue;
            }
            else if (prop.FieldType.IsSubclassOf(typeof(MScriptableObject)))
            {
                Debug.Log("DeepOverride 11" + prop.Name);
                if (value1 != null)
                {
                    Debug.Log("DeepOverride 111" + prop.Name);
                    MScriptableObject cloneValue = (MScriptableObject)value1.GetType()
                    .GetMethod("Clone")
                    .MakeGenericMethod(prop.FieldType)
                    .Invoke(value1, null);
                    value1 = cloneValue; //change reference
                    prop.SetValue(target, cloneValue);
                }
                else
                {
                    Debug.Log("DeepOverride 112" + prop.Name);
                    prop.SetValue(target, value2);
                    continue;
                }
            }

            if (!prop.FieldType.IsValueType
            && !prop.FieldType.IsArray
            && prop.FieldType.Namespace != "System"
            && (prop.FieldType.IsSubclassOf(typeof(object)) || prop.FieldType == typeof(object)))
            {
                Debug.Log("DeepOverride 2" + prop.Name);
                if (value1 == null)
                {
                    Debug.Log("DeepOverride 21" + prop.Name);
                    prop.SetValue(target, value2);
                }
                else if (value2 == null)
                {
                    Debug.Log("DeepOverride 22" + prop.Name);
                    prop.SetValue(target, null);
                }
                else
                {
                    Debug.Log("DeepOverride 23" + prop.Name);
                    DeepOverrideFields(value1, value2);
                }
            }
            else if (prop.FieldType.IsArray)
            {
                Debug.Log("DeepOverride 3" + prop.Name);
                //if array game object keep as origin
                //else array value take from source
                Array arr1 = (Array)value1;
                Array arr2 = (Array)value2;
                if ((arr1 == null || arr1.Length == 0) && (arr2 == null || arr2.Length == 0)) continue;
                Array checkArr = (arr1 != null && arr1.Length > 0) ? arr1 : arr2;
                bool isCopy = true;
                foreach (object item in checkArr)
                {
                    if (
                        (item.GetType().IsSubclassOf(typeof(GameObject)) || item.GetType() == typeof(GameObject))
                        && !item.GetType().IsSubclassOf(typeof(ScriptableObject))
                        )
                    {
                        isCopy = false;
                        break;
                    }
                    break;
                }
                if (isCopy)
                {
                    Debug.Log("DeepOverride 41" + prop.Name);
                    if (arr2 != null && arr2.Length > 0 && arr2.GetValue(0).GetType().IsSubclassOf(typeof(MScriptableObject)))
                    {
                        Debug.Log("DeepOverride 42" + prop.Name);
                        for (int e = 0; e < arr2.Length; e++)
                        {
                            Type typeArr2 = arr2.GetValue(e).GetType();
                            arr2.SetValue(
                            typeArr2
                            .GetMethod("Restore")
                            .MakeGenericMethod(typeArr2).
                            Invoke(arr2.GetValue(e), null)
                            , e);
                        }
                    }
                    prop.SetValue(target, value2);
                }
            }
            else
            {
                Debug.Log("DeepOverride 5" + prop.Name);
                prop.SetValue(target, value2);
            }

        }
    }

    private void DeepOverride<T>(T target, T source)
    {
        Debug.Log("DeepOverride -1 " + target.GetType());
        var props = target.GetType().GetProperties();
        var fields = target.GetType().GetFields();
        Debug.Log("DeepOverride -2");
        if (target.GetType() != source.GetType()) return;
        Debug.Log("DeepOverride -3");
        foreach (var prop in props)
        {
            if (prop.PropertyType.IsArray)
            {
                Debug.Log("DeepOverride Found Array");
            }
            Debug.Log("DeepOverride 0" + prop.PropertyType.GetType());
            object value1 = prop.GetValue(target, null);
            Debug.Log("DeepOverride -4");
            object value2 = prop.GetValue(source, null);
            Debug.Log("DeepOverride 23 " + prop.Name + "/" + prop.PropertyType.GetType() + ":" + value1 + " / " + value2);


            if ((prop.PropertyType.IsSubclassOf(typeof(GameObject)) || prop.PropertyType == typeof(GameObject)) && !prop.PropertyType.IsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.Log("DeepOverride 1");
                continue;
            }
            if ((prop.PropertyType.IsSubclassOf(typeof(object)) || prop.PropertyType == typeof(object)) && prop.PropertyType.GetType() != Type.GetType("System.RuntimeType"))
            {
                Debug.Log("DeepOverride 2");
                if (value1 == null)
                {
                    Debug.Log("DeepOverride 21");
                    prop.SetValue(target, value2);
                }
                else if (value2 == null)
                {
                    Debug.Log("DeepOverride 22");
                    prop.SetValue(target, null);
                }
                else
                {
                    DeepOverride(value1, value2);
                }
            }
            else if (prop.PropertyType.IsArray)
            {
                Debug.Log("DeepOverride 3");
                Array arr1 = (Array)value1;
                Array arr2 = (Array)value2;
                if (arr1.Length == 0 && arr2.Length == 0) continue;
                Array checkArr = arr1.Length > 0 ? arr1 : arr2;
                bool isCopy = true;
                foreach (object item in checkArr)
                {
                    if ((item.GetType().IsSubclassOf(typeof(GameObject)) || item.GetType() == typeof(GameObject)) && !item.GetType().IsSubclassOf(typeof(ScriptableObject)))
                    {
                        isCopy = false;
                        break;
                    }
                    break;
                }
                if (isCopy)
                {
                    Debug.Log("DeepOverride 4");
                    prop.SetValue(target, value2);
                }
            }
            else
            {
                prop.SetValue(target, value2);
            }

        }
    }

    public MScriptableObject DeepCloneChild<MScriptableObject>(MScriptableObject obj)
    {

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

