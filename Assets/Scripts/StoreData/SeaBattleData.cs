using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SeaBattleData : BaseDataEntity
{
    public ScriptableShipCustom[] shipDatas;

    public float[] transRotJsons;
    public float[] transPosJsons;
    public int playerShipIndex;

    public string windDataJson;
    public float windPower;
    public float windAccumTime;

    public ScriptableShipCustom[] levelShipDatas;
    public bool[] IsLevelShipSpawn;
    public bool[] IsRewardShipTake;

    private RewardBattle reward;
    public RewardBattle Reward
    {
        get
        {
            if (reward == null)
            {
                reward = new RewardBattle();
            }
            return reward;
        }
    }
    public ScriptableBattleFlow activeFlow;

    public bool IsBattle = false;

    public SeaBattleData()
    {

    }

    public void SetShipData(Ship[] scriptShips)
    {
        shipDatas = new ScriptableShipCustom[scriptShips.Length];
        transRotJsons = new float[shipDatas.Length];
        transPosJsons = new float[shipDatas.Length * 2];
        for (int i = 0; i < scriptShips.Length; i++)
        {
            shipDatas[i] = scriptShips[i].CustomData;
            Transform transform = scriptShips[i].gameObject.transform;
            transRotJsons[i] = transform.localRotation.eulerAngles.z;
            transPosJsons[2 * i] = transform.localPosition.x;
            transPosJsons[2 * i + 1] = transform.localPosition.y;
            if (scriptShips[i].IsPlayerShip())
            {
                playerShipIndex = i;
            }
        }
    }



    public void SetWindData(Vector2 wind, float windPower, float accumTime)
    {
        windDataJson = JsonUtility.ToJson(wind);
        this.windPower = windPower;
        windAccumTime = accumTime;
    }

    internal ScriptableShipCustom[] GetLevelShipDataSpawn(string group, string indexInG)
    {
        List<ScriptableShipCustom> result = new List<ScriptableShipCustom>();
        levelShipDatas.Select((item, index) => new { i = index, elem = item })
        .Where(x => ScriptableGameLevel.IsBattleIdMatch(x.elem.battleId, group, indexInG))
        .ToList()
        .ForEach(x =>
        {
            IsLevelShipSpawn[x.i] = true;
            result.Add(x.elem);
        });
        return result.ToArray();
    }
}
