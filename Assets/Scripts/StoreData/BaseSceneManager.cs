using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    protected bool autoRestoreState = true;
    public virtual void Awake()
    {
        if (autoRestoreState)
        {
            RestoreFromData();
        }
    }
    public virtual BaseDataEntity GetDataForSave()
    {
        // throw new System.NotImplementedException();
        FieldInfo p = this.GetType().GetField(GetFieldDataName());
        if (p != null)
        {
            return (BaseDataEntity)p.GetValue(this);
        }
        return null;
    }

    public string GetFieldDataName()
    {
        string name = this.GetType().Name.Replace("Manager", "Data");
        name = name[0].ToString().ToLower() + name.Substring(1);
        return name;
    }

    public virtual void RestoreFromData()
    {
        // var props = this.GetType().GetFields();
        FieldInfo p = this.GetType().GetField(GetFieldDataName());
        if (p != null)
        {
            object value = GameManager.Instance.GetType().GetMethod("LoadDataScene").MakeGenericMethod(p.FieldType).Invoke(GameManager.Instance, new object[] { this.GetType().Name });
            p.SetValue(this, value);
        }

        // foreach (var p in props)
        // {
        //     Debug.Log("Found field reload " + p.Name + " " + this.GetType().Name);
        //     if (p.Name.ToLower() == this.GetType().Name.ToLower().Replace("manager", "data"))
        //     {

        //         p.SetValue(this, value);
        //         return;
        //     }
        // }
        // throw new System.NotImplementedException();
    }

}
