using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameData : BaseDataEntity
{
    public const int PROCESS_INIT_FIRST_SHIP = 0;
    public const int PROCESS_FIRST_TIME_BATTLE = 1;
    public const int PROCESS_DONE_BATTLE_GUIDE = 2;
    public const int PROCESS_DONE_TOWN_GUIDE = 3;
    public string SceneCurrentName;

    public int process = 0; //mark general process 
    public ScriptableShipCustom playerShip;
}
