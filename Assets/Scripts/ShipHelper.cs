using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipHelper
{
    public const float SHIP_TECH_VALUE_RATE = 0.9f;
    public const float SHIP_WORK_VALUE_RATE = 0.1f;

    public const int MAX_SHIP_SKILL = 4;
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

    internal static int CalculateRepairCost(ScriptableShipCustom data)
    {
        int cost = CalculateShipPrice(data, false, false, false) //exclude upgrade, exclude skill, full health
        - CalculateShipPrice(data, false, false, true); //exclude upgrade, exclude skill, current health
        return cost > 10 ? cost : 10;  //min value 
    }

    internal static int CalculateSellPrice(ScriptableShipCustom data)
    {
        if (IsBrokenShip(data.curShipData, data.PeakData))
        {
            int brokenPrice = 1000;
            return brokenPrice;
        }

        int price = CalculateShipPrice(data);
        return (int)(price * 0.7f);//drop value 
    }

    internal static int CalculateShipPrice(ScriptableShipCustom data, bool upgradeApply = true, bool skillApply = true, bool stateHealth = true)
    {
        int basePrice = data.baseShipData.basePrice;
        // if (basePrice <= 0)
        // {
        //     //update base
        //     ScriptableShip template = MyResourceUtils.ResourcesLoadAll<ScriptableShip>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS).Where(x => x.name == data.baseShipData.name).FirstOrDefault();
        //     if (template != null)
        //     {
        //         basePrice = template.basePrice;
        //         data.baseShipData.basePrice = basePrice;
        //     }
        // }
        int numberSlotSkill = GetNumberSkillSlot(data);
        int price = (int)(
        (upgradeApply ? CalculateUpgradeValue(data) : 0)
        + (skillApply ? Mathf.Pow(3, numberSlotSkill) : 1)
        + basePrice //giá trị công nghệ cố định
        + (basePrice * ((1 - SHIP_TECH_VALUE_RATE) / SHIP_TECH_VALUE_RATE)) * (stateHealth ? CalculateStateHealth(data) : 1) //base price is technology
            );
        return price;
    }

    public static float CalculateStateHealth(ScriptableShipCustom data)
    {
        float hullSailRate = 3; // trọng số coi trọng hull health hơn sail
        return (data.curShipData.hullHealth * hullSailRate + data.curShipData.sailHealth) / (data.PeakData.hullHealth * hullSailRate + data.PeakData.sailHealth);
    }
    private static int GetNumberSkillSlot(ScriptableShipCustom data)
    {
        return data.skills?.Length ?? 0;
    }

    private static int CalculateSkillsValue(ScriptableShipCustom data)
    {
        throw new NotImplementedException();
    }

    internal static int CalculateUpgradeValue(ScriptableShipCustom data)
    {
        return 0;
    }

    internal static int CalculateAllCargoValue(ScriptableShipCustom data)
    {
        if (data.inventory == null || data.inventory.goodsCode == null || data.inventory.goodsCode.Length == 0)
        {
            return 0;
        }
        List<ScriptableShipGoods> allGoods = MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS).ToList();
        int gold = 0;

        MarketStateToday todayMarket = GameManager.Instance.GetMarketStateToday();

        for (int i = 0; i < data.inventory.goodsCode.Length; i++)
        {
            gold += todayMarket.GoldReceivedBySell(data.inventory.goodsCode[i], data.inventory.quantity[i]);
        }
        return gold;
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


    internal static ScriptableShip CalculatePeakData(ScriptableShipCustom data)
    {
        ScriptableShip peakState = data.baseShipData;
        if (peakState != null)
        {
            peakState = peakState.Clone<ScriptableShip>();

            //apply material buff
            peakState.hullHealth = peakState.hullHealth * (data.hullMaterial?.effectRate ?? 1);
            peakState.sailHealth = peakState.sailHealth * (data.sailMaterial?.effectRate ?? 1);
        }

        return peakState;
    }

    internal static string[] GetAvaiableShotType(ShipInventory inventory)
    {
        if (inventory == null || CommonUtils.IsArrayNullEmpty(inventory.goodsCode)) return null;
        List<string> avaiableShot = new List<string>();
        ScriptableCannonBall[] avaiableCannonBall = SeaBattleManager.Instance.avaiableCannonBall;
        for (int i = 0; i < avaiableCannonBall.Length; i++)
        {
            for (int j = 0; j < inventory.goodsCode.Length; j++)
            {
                if (inventory.goodsCode[j].Equals(avaiableCannonBall[i].goodsCodeCost))
                {
                    if (inventory.quantity[j] > 0)
                    {
                        avaiableShot.Add(avaiableCannonBall[i].codeName);
                    }
                    break;
                }
            }
        }
        return avaiableShot.ToArray();
    }

    internal static float GetRangeCannonType(string cannonTypeCode)
    {
        Debug.Log("cannon type " + cannonTypeCode);
        return SeaBattleManager.Instance.avaiableCannonBall.Where(x => x.codeName == cannonTypeCode).FirstOrDefault()?.range ?? 0;
    }

    internal static int DirectionToIndex(SeaBattleManager.CannonDirection direction)
    {
        switch (direction)
        {
            case SeaBattleManager.CannonDirection.Front: return 0;
            case SeaBattleManager.CannonDirection.Right: return 1;
            case SeaBattleManager.CannonDirection.Left: return 2;
            case SeaBattleManager.CannonDirection.Back: return 3;
            default: return -1;
        }
    }

    internal static string GetBallType(string cannonType)
    {
        return SeaBattleManager.Instance.avaiableCannonBall.Where(x => x.codeName == cannonType).FirstOrDefault()?.goodsCodeCost ?? "";
    }
}
