using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipCustom", menuName = "BoS/Ship Custom", order = 6)]
public class ScriptableShipCustom : ScriptableObjectPrefab
{
    public ScriptableShip baseShipData;
    public ScriptableShip curShipData;
    public ShipInventory inventory;

    public ScriptableCaptain captain;

    public ScriptableShipUpgrade[] upgrades;
    public ScriptableShipSkill[] skills;

    public Transform transform;
}
