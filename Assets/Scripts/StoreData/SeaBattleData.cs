using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeaBattleData : BaseDataEntity
{
    public ScriptableShipCustom[] shipDatas;

    public float[] transRotJsons;
    public float[] transPosJsons;
    public int playerShipIndex;

    public string windDataJson;
    public float windAccumTime;

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



    public void SetWindData(Vector2 wind, float accumTime)
    {
        windDataJson = JsonUtility.ToJson(wind);
        windAccumTime = accumTime;
    }
}
