using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyResourceUtils
{
    public const string RESOURCES_PATH_SCRIPTABLE_OBJECTS = "ScriptableObjects";
    public const string RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS = "ScriptableObjects/ShipGoods";

    public static T ResourcesLoad<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

    public static T[] ResourcesLoadAll<T>(string path) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(path);
    }
}
