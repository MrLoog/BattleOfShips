using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

public class GEventManager : Singleton<GEventManager>
{
    protected GEventManager() { }

    public EventDict eventDict = new EventDict();

    public const string EVENT_CLEAR_LEVEL = "CLEAR_LEVEL";
    public const string EVENT_TEST = "EVENT_TEST";

    ScriptableAchievement[] allEvents;

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


    public void ScriptableEventResponse(string key)
    {
        ScriptableAchievement[] allEvent = AllEvents.Where(x => !CommonUtils.IsArrayNullEmpty(x.eventTriggers) && x.eventTriggers.Contains(key)).ToArray();
        if (!CommonUtils.IsArrayNullEmpty(allEvent))
        {
            for (int i = 0; i < allEvent.Length; i++)
            {
                bool check = true;
                foreach (var aCheck in allEvent[i].actionCheck)
                {
                    check = GActionManager.Instance.Check(aCheck);
                    if (!check) break;
                }

                if (check)
                {
                    foreach (var aAction in allEvent[i].actionResult)
                    {
                        GActionManager.Instance.Perform(aAction);
                    }
                    GameManager.Instance.gameData.MakeClearedAchivement(allEvent[i].codeName);
                }
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
