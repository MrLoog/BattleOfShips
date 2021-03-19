using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableGameLevel", menuName = "BoS/Game Level", order = 12)]
public class ScriptableGameLevel : MScriptableObject
{
    public ScriptableGameLevel[] nextLevel;

    public bool isMainLevel = true;
    public string codeName;
    public string shortName;
    public string levelName;
    [TextArea(4, 100)]
    public string description;
    public ScriptableBattleFlow battleFlow;
    public GroupShip[] groupShips;
    public RewardBattle reward;

    public ScriptableShipCustom[] GetAllShipData()
    {
        return GetAllShipDataFromGroup(groupShips);
    }

    public static ScriptableShipCustom[] GetAllShipDataFromGroup(GroupShip[] groupShips)
    {
        List<ScriptableShipCustom> datas = null;
        if (!CommonUtils.IsArrayNullEmpty(groupShips))
        {
            ScriptableShipCustom newData = null;
            datas = new List<ScriptableShipCustom>();
            for (int i = 0; i < groupShips.Length; i++)
            {
                if (CommonUtils.IsArrayNullEmpty(groupShips[i].shipFactorys)) continue;
                for (int j = 0; j < groupShips[i].shipFactorys.Length; j++)
                {
                    newData = groupShips[i].shipFactorys[j].GetRandomShip(1)[0];
                    newData.battleId = BuildBattleId(i.ToString(), j.ToString());
                    newData.group = 1;
                    datas.Add(newData);
                }
            }
        }
        return datas?.ToArray();
    }

    public static string BuildBattleId(string group, string index)
    {
        return group + "_" + index;
    }

    internal static bool IsBattleIdMatch(string battleId, string group, string index)
    {
        string pat = group + "_";
        if (index != "") pat += index + "$";
        else pat += ".*";
        Regex rgx = new Regex(pat);
        return rgx.IsMatch(battleId);
    }
}
