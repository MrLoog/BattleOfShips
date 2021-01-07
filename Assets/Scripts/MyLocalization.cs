using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLocalization
{
    public static Dictionary<string, string> defaultDict = new Dictionary<string, string>
    {
        {"hello","hello"}
    };

    public static string GetValue(string key)
    {
        try
        {
            return defaultDict[key];
        }
        catch (UnityException)
        {
            Debug.Log("not found string locale");
            return "Nul";
        }
    }
}
