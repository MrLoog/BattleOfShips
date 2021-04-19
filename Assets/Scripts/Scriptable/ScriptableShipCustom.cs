using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipCustom", menuName = "BoS/Ship Custom", order = 6)]
public class ScriptableShipCustom : MScriptableObject
{
    public enum Union
    {
        Unknow = 0, Pirate = 1, Marine = 2, Red = 3, Green = 4, Blue = 5, Yellow = 6
    }
    public string shipName;
    public ScriptableShip baseShipData;
    private ScriptableShip peakData;
    public ScriptableShip PeakData
    {
        get
        {
            if (peakData == null)
            {
                CalculatePeakData();
            }
            return peakData;
        }
    }

    public void CalculatePeakData()
    {
        // peakData = ShipHelper.GetShipUpgrade(this);
        peakData = ShipHelper.CalculatePeakData(this);
    }
    public ScriptableShip curShipData;
    public ShipInventory inventory = new ShipInventory();

    public int group = 1;

    public Union[] unions;

    public ScriptableCaptain captain;

    public ScriptableShipUpgrade[] upgrades;

    public ScriptableShipSkill[] skills;

    public ScriptableShipMaterial hullMaterial;
    public ScriptableShipMaterial sailMaterial;

    public string battleId;

    public RewardBattle reward;

    public Dictionary<string, object> AIMemory;

}