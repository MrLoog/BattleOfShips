using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipCustom", menuName = "BoS/Ship Custom", order = 6)]
public class ScriptableShipCustom : MScriptableObject
{
    public ScriptableShip baseShipData;
    public ScriptableShip curShipData;
    public ShipInventory inventory;

    public int group = 1;

    public ScriptableCaptain captain;

    public ScriptableShipUpgrade[] upgrades;

    public ScriptableShipSkill[] skills;

}
