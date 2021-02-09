using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStoreData : IStoreData
{
    protected BaseStoreData() { }
    public virtual BaseDataEntity LoadData(string key)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool SaveData(string key, BaseDataEntity data)
    {
        throw new System.NotImplementedException();
    }
}
