using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDataFactory
{


    internal static IStoreData GetDatabase()
    {

#if UNITY_ANDROID
        return AndroidStoreData.Instance;
#else
        return null;
#endif
    }
}
