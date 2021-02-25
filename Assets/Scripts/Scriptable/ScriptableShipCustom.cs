using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipCustom", menuName = "BoS/Ship Custom", order = 6)]
public class ScriptableShipCustom : MScriptableObject
{
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
        peakData = ShipHelper.GetShipUpgrade(this);
    }
    public ScriptableShip curShipData;
    public ShipInventory inventory;

    public int group = 1;

    public ScriptableCaptain captain;

    public ScriptableShipUpgrade[] upgrades;

    public ScriptableShipSkill[] skills;

}
[Serializable]
public class Test
{
    public string prop;
}