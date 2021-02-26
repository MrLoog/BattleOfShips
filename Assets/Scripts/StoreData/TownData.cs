using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public DateTime timeRefresh;
}