using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidStoreData : BaseStoreData
{
    private AndroidStoreData() { }
    private static object m_Lock = new object();
    private static AndroidStoreData m_Instance;

    public static AndroidStoreData Instance
    {
        get
        {

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = new AndroidStoreData();
                }
                return m_Instance;
            }
        }
    }

    public override bool SaveData(string key, BaseDataEntity data)
    {
        return true;
    }

    public override BaseDataEntity LoadData(string key)
    {
        return null;
    }
}
