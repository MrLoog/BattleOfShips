using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableAchievement", menuName = "BoS/Game Achievement", order = 14)]
public class ScriptableAchievement : MScriptableObject
{
    public string codeName;
    public string[] eventTriggers;
    [SerializeField]
    public ActionApi[] actionCheck;

    public ActionApi[] actionResult;
}

[Serializable]
public class ActionApi
{
    public string actionName;
    public string[] actionParams;
}