using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipHelper
{
    public static bool IsNeedRepair(ScriptableShip currentState, ScriptableShip originState)
    {
        if (IsBrokenShip(currentState, originState))
        {
            //broken cant repair
            return false;
        }
        if (currentState.hullHealth >= originState.hullHealth && currentState.sailHealth >= originState.sailHealth)
        {
            //good health
            return false;
        }
        return true;
    }

    public static bool ValidCrewQuantity(ScriptableShip currentState, ScriptableShip originState, int change = 0)
    {
        if (currentState.maxCrew + change <= originState.maxCrew)
        {
            return true;
        }
        return false;
    }

    internal static void PerformFullRepair(ScriptableShipCustom data)
    {
        data.curShipData.sailHealth = data.PeakData.sailHealth;
        data.curShipData.hullHealth = data.PeakData.hullHealth;
    }

    internal static bool IsCanSell(ScriptableShipCustom data)
    {
        //main ship cant sell, this method not check
        // if (IsBrokenShip(data.curShipData, data.baseShipData))
        // {
        //     return false;
        // }
        return true;
    }

    public static bool IsBrokenShip(ScriptableShip currentState, ScriptableShip originState)
    {
        if (currentState.hullHealth / originState.hullHealth < 0.05f)
        {
            return true;
        }
        return false;
    }

    internal static int CalculateRepairCost(ScriptableShip curShipData, ScriptableShip baseShipData)
    {
        float hullDamaged = baseShipData.hullHealth - curShipData.hullHealth;
        float sailDamaged = baseShipData.sailHealth - curShipData.sailHealth;
        int basePrice = baseShipData.basePrice;
        float hullSailRate = 3; // trọng số coi trọng hull health hơn sail
        if (basePrice <= 0)
        {
            ScriptableShip template = MyResourceUtils.ResourcesLoadAll<ScriptableShip>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS).Where(x => x.name == baseShipData.name).FirstOrDefault();
            if (template != null)
            {
                basePrice = template.basePrice;
                baseShipData.basePrice = basePrice;
            }
        }
        int cost = (int)(basePrice * (hullDamaged * hullSailRate + sailDamaged) / (baseShipData.hullHealth * hullSailRate + baseShipData.sailHealth));
        return cost;
    }

    internal static int CalculateSellPrice(ScriptableShipCustom data)
    {
        if (IsBrokenShip(data.curShipData, data.PeakData))
        {
            int brokenPrice = 1000;
            return brokenPrice;
        }

        int basePrice = data.baseShipData.basePrice;
        float hullSailRate = 3; // trọng số coi trọng hull health hơn sail
        if (basePrice <= 0)
        {
            ScriptableShip template = MyResourceUtils.ResourcesLoadAll<ScriptableShip>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS).Where(x => x.name == data.baseShipData.name).FirstOrDefault();
            if (template != null)
            {
                basePrice = template.basePrice;
                data.baseShipData.basePrice = basePrice;
            }
        }
        int upgradeValue = CalculateUpgradeValue(data);
        int cost = (int)(upgradeValue + basePrice * (data.curShipData.hullHealth * hullSailRate + data.curShipData.sailHealth) / (data.PeakData.hullHealth * hullSailRate + data.PeakData.sailHealth));
        cost = (int)(cost * 0.7f); //drop value 
        return cost;
    }

    internal static int CalculateUpgradeValue(ScriptableShipCustom data)
    {
        return 0;
    }

    internal static int CalculateAllCargoValue(ScriptableShipCustom data)
    {
        return 0;
    }

    internal static float CalculateAllCargoWeight(ScriptableShipCustom data)
    {
        if (data.inventory == null || data.inventory.goodsCode == null || data.inventory.goodsCode.Length == 0)
        {
            return 0;
        }
        List<ScriptableShipGoods> allGoods = MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS).ToList();
        float weight = 0;
        ScriptableShipGoods aGoods = null;
        for (int i = 0; i < data.inventory.goodsCode.Length; i++)
        {
            aGoods = allGoods.FirstOrDefault(x => x.codeName == data.inventory.goodsCode[i]);
            weight += aGoods == null ? 0 : aGoods.weight * data.inventory.quantity[i];
        }
        return weight;
    }

    internal static float CalculateAvaiableCapacity(ScriptableShipCustom data)
    {
        return data.PeakData.capacity - CalculateAllCargoWeight(data);
    }

    public static ScriptableShip GetShipUpgrade(ScriptableShipCustom data)
    {
        return data.baseShipData;
    }
}
