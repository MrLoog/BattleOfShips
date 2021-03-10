using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class GameData : BaseDataEntity
{
    public const int PROCESS_INIT_FIRST_SHIP = 0;
    public const int PROCESS_FIRST_TIME_BATTLE = 1;
    public const int PROCESS_DONE_BATTLE_GUIDE = 2;
    public const int PROCESS_DONE_TOWN_GUIDE = 3;
    public string SceneCurrentName;
    public string ScenePrevName;

    public string playLevelName;

    public int process = 0; //mark general process 
    public ScriptableShipCustom playerShip;
    public ScriptableShipCustom[] otherShips;

    public int gold;
    public int gem;

    public MarketStateToday market;

    public ScriptableShipFactory shipShopFactory;

    public string[] levelCleared;
    public string[] achievementCleared;

    internal void AddSubShip(ScriptableShipCustom scriptableShipCustom)
    {
        if (otherShips == null)
        {
            otherShips = new ScriptableShipCustom[] { scriptableShipCustom };
        }
        else
        {
            otherShips = otherShips.Concat(new[] { scriptableShipCustom }).ToArray();
        }
    }

    internal bool IsLevelCleared(string levelCodeName)
    {
        if (!CommonUtils.IsArrayNullEmpty(levelCleared) && levelCleared.Contains(levelCodeName))
        {
            return true;
        }
        return false;
    }

    internal bool IsLevelClearedPattern(string levelNamePattern)
    {
        if (CommonUtils.IsArrayNullEmpty(levelCleared)) return false;
        object find = levelCleared.FirstOrDefault(x => CommonUtils.IsStrMatchPattern(x, levelNamePattern));

        return find != null;
    }

    internal bool MakeClearedLevel(string levelCodeName)
    {
        if (!CommonUtils.IsArrayNullEmpty(levelCleared) && levelCleared.Contains(levelCodeName))
        {
            return false;
        }
        levelCleared = CommonUtils.AddElemToArray(levelCleared, levelCodeName);
        return true;
    }

    internal void MakeClearedAchivement(string achievementCode)
    {
        achievementCleared = CommonUtils.AddElemToArray(achievementCleared, achievementCode);
    }

    internal bool IsAchivementCleared(string codeName)
    {
        if (!CommonUtils.IsArrayNullEmpty(achievementCleared) && achievementCleared.Contains(codeName))
        {
            return true;
        }
        return false;
    }
    internal bool IsAchivementClearedPattern(string achievementPattern)
    {
        if (CommonUtils.IsArrayNullEmpty(achievementCleared)) return false;
        object find = achievementCleared.FirstOrDefault(x => CommonUtils.IsStrMatchPattern(x, achievementPattern));

        return find != null;
    }
}
