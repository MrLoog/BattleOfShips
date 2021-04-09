using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameObjectUtils
{
    public static void ClearAllChilds(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Object.Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }

    public static void ClearAllChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
