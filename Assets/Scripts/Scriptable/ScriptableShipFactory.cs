using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableShipFactory", menuName = "BoS/Ship Factory", order = 11)]
public class ScriptableShipFactory : MScriptableObject
{
    public bool IsUseCustom => !CommonUtils.IsArrayNullEmpty(shipCustomList);
    public ScriptableShip[] shipList;
    public ScriptableShipCustom[] shipCustomList;
    public int[] shipRates;

    public int[] slotRate;

    public ScriptableShipSkill[] skillList;

    public ScriptableShipMaterial[] hullMaterials;
    public int[] hullMaterialRates;
    public ScriptableShipMaterial[] sailMaterials;
    public int[] sailMaterialRates;

    public ShipInventory fixedInventory;
    public string[] goodsCode;
    public int[] minQty;
    public int[] maxQty;

    public float maxWeight;
    public RewardBattle reward;

    public ScriptableShipCustom[] GetRandomShip(int quantity = 1, bool noCrew = false)
    {
        ScriptableShipCustom[] result = new ScriptableShipCustom[quantity];
        List<string> goodsPool = goodsCode?.ToList();
        List<MScriptableObject> shipPool = IsUseCustom ?
        shipCustomList.Where(x => x != null).Cast<MScriptableObject>().ToList() :
        shipList.Where(x => x != null).Cast<MScriptableObject>().ToList();

        List<ScriptableShipSkill> skillPool = skillList?.Where(x => x != null).ToList();

        for (int n = 0; n < quantity; n++)
        {
            List<ScriptableShipSkill> skillPool2 = skillPool?.ToList();
            int choiceShip = -1;
            if (!CommonUtils.IsArrayNullEmpty(shipRates))
            {
                choiceShip = CommonUtils.RandomByRate(shipRates);
            }
            else
            {
                choiceShip = Random.Range(0, shipPool.Count);
            }
            ScriptableShipCustom resultShip = null;
            if (IsUseCustom)
            {
                resultShip = shipPool[choiceShip].Clone<ScriptableShipCustom>();
            }
            else
            {
                resultShip = ScriptableShipCustom.CreateInstance<ScriptableShipCustom>();
                resultShip.baseShipData = shipPool[choiceShip].Clone<ScriptableShip>();
            }


            //random skill
            if (slotRate != null && slotRate.Length > 0)
            {
                int numberSlot = 1 + CommonUtils.RandomByRate(slotRate);
                ScriptableShipSkill[] choiceSkills = new ScriptableShipSkill[numberSlot];
                for (int i = 0; i < numberSlot; i++)
                {
                    int choiceSkill = Random.Range(0, skillPool2.Count);
                    choiceSkills[i] = skillPool2[choiceSkill];
                    skillPool2.RemoveAt(choiceSkill);
                }
                resultShip.skills = choiceSkills;
            }
            //random material hull
            if (!CommonUtils.IsArrayNullEmpty(hullMaterials))
            {
                int choice = -1;
                if (!CommonUtils.IsArrayNullEmpty(hullMaterialRates) && hullMaterials.Length == hullMaterialRates.Length)
                {
                    choice = CommonUtils.RandomByRate(hullMaterialRates);
                }
                else
                {
                    choice = Random.Range(0, hullMaterials.Length);
                }
                resultShip.hullMaterial = hullMaterials[choice];
            }
            //random material sail
            if (!CommonUtils.IsArrayNullEmpty(sailMaterials))
            {
                int choice = -1;
                if (!CommonUtils.IsArrayNullEmpty(sailMaterialRates) && sailMaterials.Length == sailMaterialRates.Length)
                {
                    choice = CommonUtils.RandomByRate(sailMaterialRates);
                }
                else
                {
                    choice = Random.Range(0, sailMaterials.Length);
                }
                resultShip.sailMaterial = sailMaterials[choice];
            }


            if (fixedInventory != null)
            {
                if (resultShip.inventory == null) resultShip.inventory = new ShipInventory();

                resultShip.inventory.quantity = CommonUtils.AddElemToArray(resultShip.inventory.quantity, fixedInventory.quantity);
                resultShip.inventory.goodsCode = CommonUtils.AddElemToArray(resultShip.inventory.goodsCode, fixedInventory.goodsCode);

            }
            //random inventory
            if (!CommonUtils.IsArrayNullEmpty(goodsCode))
            {
                float totalWeight = 0;
                float limitWeight = maxWeight <= 0 ? ShipHelper.CalculateAvaiableCapacity(resultShip) : maxWeight;
                if (resultShip.inventory == null)
                {
                    resultShip.inventory = new ShipInventory();
                }
                List<string> lstGoodsRandom = goodsPool.ToList();
                // resultShip.inventory.goodsCode = goodsCode;
                bool atLimit = false;
                bool isMaxDeclare = !CommonUtils.IsArrayNullEmpty(maxQty);

                string selectGoods;
                ScriptableShipGoods good;
                int i;
                int qty, rMin = -1, rMax = 0;
                float weight;
                bool isRepeatRandom = CommonUtils.IsArrayNullEmpty(minQty) || minQty.Length == 1;

                while (lstGoodsRandom.Count > 0)
                {
                    selectGoods = CommonUtils.RandomElemFromList(lstGoodsRandom, 1, true).FirstOrDefault();
                    good = MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS).FirstOrDefault(x => x.codeName == selectGoods);
                    i = goodsPool.IndexOf(selectGoods);

                    if (isRepeatRandom && rMin == -1)
                    {
                        rMin = minQty[0];
                        rMax = isMaxDeclare ? (maxQty[0] + 1) : (minQty[0] + 1);
                    }
                    else if (!isRepeatRandom)
                    {
                        rMin = minQty[i];
                        rMax = isMaxDeclare ? (maxQty[i] + 1) : (minQty[i] + 1);
                    }

                    qty = Random.Range(rMin, rMax);

                    if (good.weight <= 0) continue;

                    weight = qty * good.weight;
                    if (totalWeight + weight > limitWeight)
                    {
                        qty = (int)((limitWeight - totalWeight) / good.weight);
                        atLimit = true;
                    }
                    if (qty > 0)
                    {
                        totalWeight += (qty * good.weight);
                        resultShip.inventory.quantity = CommonUtils.AddElemToArray(resultShip.inventory.quantity, qty);
                        resultShip.inventory.goodsCode = CommonUtils.AddElemToArray(resultShip.inventory.goodsCode, goodsCode[i]);
                    }
                    if (atLimit) break;
                }
            }
            if (reward != null)
            {
                resultShip.reward = reward;
            }

            if (resultShip.curShipData == null)
            {
                resultShip.curShipData = resultShip.PeakData.Clone<ScriptableShip>();
            }
            if (noCrew)
            {
                resultShip.curShipData.maxCrew = 0;
            }

            result[n] = resultShip;
        }


        return result;
    }
}


