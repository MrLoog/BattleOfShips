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

    public ConditionAction ToConditionAction()
    {
        ConditionAction ca = new ConditionAction();
        ca.eventTrigger = eventTriggers;
        ca.actionCheck = actionCheck;
        ca.actionResult = actionResult;
        return ca;
    }
}
