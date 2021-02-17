using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStoreData
{
    bool SaveData(string key, BaseDataEntity data);
    T LoadData<T>(string key);
}
