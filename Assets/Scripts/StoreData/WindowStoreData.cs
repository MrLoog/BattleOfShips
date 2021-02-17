using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowStoreData : BaseStoreData
{
    private WindowStoreData() { }
    private static object m_Lock = new object();
    private static WindowStoreData m_Instance;

    public static WindowStoreData Instance
    {
        get
        {

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = new WindowStoreData();
                }
                return m_Instance;
            }
        }
    }

    public override bool SaveData(string key, BaseDataEntity data)
    {
        return true;
    }

    public override T LoadData<T>(string key)
    {
        return default(T);
    }
}
