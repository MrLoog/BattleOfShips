using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class TownData : BaseDataEntity
{
    public Workshop workshop;
}

[Serializable]
public class Workshop
{
    public ScriptableShipCustom[] workshopShips;

    public bool[] soldStatus;

    public int slot = 5;

    public bool forceReload = false;
    [SerializeField]
    public DateTime timeRefresh;

    internal void MarkShipSold(ScriptableShipCustom data)
    {
        soldStatus[workshopShips.ToList().FindIndex(x => data.Equals(x))] = true;
    }

    internal bool IsSold(ScriptableShipCustom data)
    {
        return soldStatus[workshopShips.ToList().FindIndex(x => data.Equals(x))];
    }
}