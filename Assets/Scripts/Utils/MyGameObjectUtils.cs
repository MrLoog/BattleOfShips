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
}
