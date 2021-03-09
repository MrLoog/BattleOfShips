using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;

public class GEventManager : Singleton<GEventManager>
{
    protected GEventManager() { }

    private EventDict eventDict = new EventDict();


    public EventDict EventDict
    {
        get
        {
            return eventDict;
        }
    }

    public const string EVENT_CLEAR_LEVEL = "CLEAR_LEVEL";
    public const string EVENT_TEST = "EVENT_TEST";
    public const string EVENT_START_BATTLE = "START_BATTLE";
    public const string EVENT_PLAYER_DEFEATED = "PLAYER_DEFEATED";
    public const string EVENT_SHIP_DEFEAT = "SHIP_DEFEAT";
    ScriptableAchievement[] allEvents;

    private Dictionary<string, UnityAction[]> battleFlowActions;

    private void ClearBattleFlowActions()
    {
        if (battleFlowActions == null || battleFlowActions.Count == 0) return;
        foreach (var item in battleFlowActions)
        {
            foreach (var uAction in item.Value)
            {
                EventDict.RegisterListener(item.Key).RemoveListener(uAction);
            }
        }
    }
    internal void RegisterBattleFlow(ScriptableBattleFlow battleFlow)
    {
        ClearBattleFlowActions();
        if (!CommonUtils.IsArrayNullEmpty(battleFlow.battlePoints))
        {
            battleFlowActions = new Dictionary<string, UnityAction[]>();
            // battleFlowActions = new UnityAction[battleFlow.battlePoints.Length];
            for (int i = 0; i < battleFlow.battlePoints.Length; i++)
            {
                ConditionAction bp = battleFlow.battlePoints[i];
                UnityAction newAction = delegate ()
                {
                    ProcessActionApi(bp);
                };
                // battleFlowActions[i] = newAction;
                if (!CommonUtils.IsArrayNullEmpty(bp.eventTrigger))
                {
                    foreach (var eventCode in bp.eventTrigger)
                    {
                        if (eventCode.Length > 0)
                        {
                            if (!battleFlowActions.ContainsKey(eventCode))
                            {
                                battleFlowActions.Add(eventCode, null);
                            }
                            // EventDict.RegisterListener(eventCode).RemoveAllListeners();
                            battleFlowActions[eventCode] = CommonUtils.AddElemToArray(battleFlowActions[eventCode], newAction);
                            EventDict.RegisterListener(eventCode).AddListener(newAction);
                        }
                    }
                }
            }
        }
    }

    public ScriptableAchievement[] AllEvents
    {
        get
        {
            return allEvents;
        }
    }
    public void InvokeEvent(string key)
    {
        if (allEvents == null)
        {
            LoadEvents();
        }
        ScriptableEventResponse(key);
        eventDict.InvokeOnAction(key);
        // Thread thread = new Thread(new ThreadStart(delegate ()
        // {
        // }));
        // thread.Start();
    }


    public bool ProcessActionApi(ConditionAction battlePoint)
    {
        bool check = true;
        foreach (var aCheck in battlePoint.actionCheck)
        {
            if (aCheck.expectResult != null && aCheck.expectResult.Length > 0)
            {
                check = (aCheck.expectResult == (string)GActionManager.Instance.Check(aCheck));
            }
            else
            {
                check = (bool)GActionManager.Instance.Check(aCheck);
            }
            if (!check) break;
        }

        if (check)
        {
            foreach (var aAction in battlePoint.actionResult)
            {
                GActionManager.Instance.Perform(aAction);
            }
            return true;
        }
        return false;
    }
    public void ScriptableEventResponse(string key)
    {
        ScriptableAchievement[] allEvent = AllEvents.Where(x => !CommonUtils.IsArrayNullEmpty(x.eventTriggers) && x.eventTriggers.Contains(key)).ToArray();
        if (!CommonUtils.IsArrayNullEmpty(allEvent))
        {
            for (int i = 0; i < allEvent.Length; i++)
            {
                if (ProcessActionApi(allEvent[i].ToConditionAction()))
                {
                    GameManager.Instance.gameData.MakeClearedAchivement(allEvent[i].codeName);
                }
                // bool check = true;
                // foreach (var aCheck in allEvent[i].actionCheck)
                // {
                //     if (aCheck.expectResult != null && aCheck.expectResult.Length > 0)
                //     {
                //         check = (aCheck.expectResult == (string)GActionManager.Instance.Check(aCheck));
                //     }
                //     else
                //     {
                //         check = (bool)GActionManager.Instance.Check(aCheck);
                //     }
                //     if (!check) break;
                // }

                // if (check)
                // {
                //     foreach (var aAction in allEvent[i].actionResult)
                //     {
                //         GActionManager.Instance.Perform(aAction);
                //     }
                // }
            }
        }
    }


    public void LoadEvents()
    {
        if (allEvents == null)
            allEvents = MyResourceUtils.ResourcesLoadAll<ScriptableAchievement>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS)
            .Where(x => !GameManager.Instance.gameData?.IsAchivementCleared(x.codeName) ?? true).ToArray();
    }
}


[Serializable]
public class ConditionAction
{
    public string[] eventTrigger;
    public float[] battleTime;
    public ActionApi[] actionCheck;

    public ActionApi[] actionResult;
}

[Serializable]
public class ActionApi
{
    public string actionName;
    public string[] actionParams;

    public string expectResult;
}