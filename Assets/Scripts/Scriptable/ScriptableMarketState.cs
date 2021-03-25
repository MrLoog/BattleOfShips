using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableMarketState", menuName = "BoS/Market State", order = 10)]
public class ScriptableMarketState : MScriptableObject
{
    public ScriptableMarketState inheritMarket;

    [TextArea(3, 7)]
    public string description;

    public int probability;
    public string[] goodsCode;


    [Tooltip("override goods base price")]
    public int[] basePrice;
    public int[] baseQuantiy;
    public float[] minRatePrice;
    public float[] maxRatePrice;

    [Tooltip("price of good will be tunning with this rate +/- on base price. This priority over min/max declare")]
    public float[] priceMod;

    [Tooltip("if not declare in min,max base price of good will be tunning with this rate +/-")]
    public float otherPriceMod = 0f;
    public float[] minRateQty;
    public float[] maxRateQty;
    [Tooltip("quantity of good will be tunning with this rate +/- on base quanity. This priority over min/max declare")]
    public float[] qtyMod;

    [Tooltip("if not declare in min,max base quantity of good will be tunning with this rate +/-")]
    public float otherQtyMod = 0f;


    [Tooltip("[0][1] min-max quantity, [2][3] min-max price")]
    public int[] crewMinMaxQtyPrice;

    public MarketStateToday CalculateStateWithInherit()
    {
        MarketStateToday result = CalculateStateToday();
        if (inheritMarket != null)
        {
            MarketStateToday parent = inheritMarket.CalculateStateWithInherit();

            for (int i = 0; i < parent.goodsCodes.Length; i++)
            {
                if (result.goodsCodes.Contains(parent.goodsCodes[i])) continue;

                result.goodsCodes = CommonUtils.AddElemToArray(result.goodsCodes, parent.goodsCodes[i]);
                result.prices = CommonUtils.AddElemToArray(result.prices, parent.prices[i]);
                result.quantitys = CommonUtils.AddElemToArray(result.quantitys, parent.quantitys[i]);
            }

            if (CommonUtils.IsArrayNullEmpty(crewMinMaxQtyPrice))
            {
                result.crewAvaiable = parent.crewAvaiable;
                result.crewPrice = parent.crewPrice;
            }
        }
        return result;
    }

    private MarketStateToday CalculateStateToday()
    {
        MarketStateToday today = new MarketStateToday();
        today.time = DateTime.Now;
        List<string> l_goodsCodes = new List<string>();
        List<int> l_prices = new List<int>();
        List<int> l_quantitys = new List<int>();

        if (!CommonUtils.IsArrayNullEmpty(goodsCode))
        {
            List<ScriptableShipGoods> allGoods = MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS)
            .Where(x => goodsCode.Contains(x.codeName)).ToList();
            ScriptableShipGoods aGoods = null;
            int g_price, g_qty;
            for (int i = 0; i < goodsCode.Length; i++)
            {
                aGoods = allGoods.FirstOrDefault(x => x.codeName == goodsCode[i]);
                if (aGoods == null) continue;

                l_goodsCodes.Add(aGoods.codeName);
                
                g_price = aGoods.basePrice;
                g_qty = aGoods.baseQuantiy;

                //random if config
                if (!CommonUtils.IsArrayNullEmpty(priceMod) && priceMod.Length > i)
                {
                    l_prices.Add((int)Random.Range(g_price * (1 - priceMod[i]), g_price * (1 + priceMod[i])));
                }
                else if (!CommonUtils.IsArrayNullEmpty(minRatePrice) && minRatePrice.Length > i)
                {
                    if (basePrice.Length > i) g_price = basePrice[i];
                    l_prices.Add((int)Random.Range(g_price * minRatePrice[i],
                    g_price * (!CommonUtils.IsArrayNullEmpty(maxRatePrice) && maxRatePrice.Length > i ? maxRatePrice[i] : minRatePrice[i])
                    ));
                }
                else
                {
                    l_prices.Add(g_price);
                }

                if (baseQuantiy.Length > i) g_qty = baseQuantiy[i];
                if (!CommonUtils.IsArrayNullEmpty(qtyMod) && qtyMod.Length > i)
                {
                    l_quantitys.Add((int)Random.Range(g_qty * (1 - qtyMod[i]), g_qty * (1 + qtyMod[i])));
                }
                else if (!CommonUtils.IsArrayNullEmpty(minRateQty) && minRateQty.Length > i)
                {
                    l_quantitys.Add((int)Random.Range(g_qty * minRateQty[i],
                    g_qty * (!CommonUtils.IsArrayNullEmpty(maxRateQty) && maxRateQty.Length > i ? maxRateQty[i] : minRateQty[i])
                    ));
                }
                else
                {
                    l_quantitys.Add(g_qty);
                }
            }
        }

        today.goodsCodes = l_goodsCodes.ToArray();
        today.prices = l_prices.ToArray();
        today.quantitys = l_quantitys.ToArray();

        if (crewMinMaxQtyPrice != null && crewMinMaxQtyPrice.Length == 4)
        {
            today.crewAvaiable = Random.Range(crewMinMaxQtyPrice[0], crewMinMaxQtyPrice[1] + 1);
            today.crewPrice = Random.Range(crewMinMaxQtyPrice[2], crewMinMaxQtyPrice[3] + 1);
        }
        return today;

    }
}
[Serializable]
public class MarketStateToday
{
    public string[] goodsCodes;
    public int[] prices;
    public int[] quantitys;
    [SerializeField]
    public DateTime time;

    public int crewAvaiable;
    public int crewPrice;

    public int GoldReceivedBySell(int indexGoods, int quantity)
    {
        return (int)(prices[indexGoods] * 0.7 * quantity);
    }

    public int GoldReceivedBySell(string goodsCode, int quantity)
    {

        return (int)(prices[goodsCodes.ToList().FindIndex(x => x == goodsCode)] * 0.7 * quantity);
    }

    public int GoldLostByBuy(int indexGoods, int quantity)
    {
        return (int)(prices[indexGoods] * quantity);
    }

    public int GoldLostByBuy(string goodsCode, int quantity)
    {
        return (int)(prices[goodsCodes.ToList().FindIndex(x => x == goodsCode)] * quantity);
    }
}

public class MarketStateFactory
{
    public static MarketStateToday BuildMarketStateToday(ScriptableShipGoods[] allGoods, ScriptableMarketState state)
    {
        Debug.Log("BuildMarketStateToday");
        MarketStateToday today = state.CalculateStateWithInherit();
        today.time = DateTime.Now;


        foreach (var aGoods in allGoods)
        {
            if (!CommonUtils.IsArrayNullEmpty(today.goodsCodes) && today.goodsCodes.Contains(aGoods.codeName)) continue;

            today.goodsCodes = CommonUtils.AddElemToArray(today.goodsCodes, aGoods.codeName);

            int basePrice = aGoods.basePrice;
            int baseQuantiy = aGoods.baseQuantiy;

            today.prices = CommonUtils.AddElemToArray(
                today.prices,
            (int)(basePrice * (1 + Random.Range(-state.otherPriceMod, state.otherPriceMod)))
            );
            today.quantitys = CommonUtils.AddElemToArray(
                today.quantitys,
            (int)(baseQuantiy * (1 + Random.Range(-state.otherQtyMod, state.otherQtyMod)))
            );

        }

        return today;
    }
    public static MarketStateToday BuildMarketStateTodayBackup(ScriptableShipGoods[] allGoods, ScriptableMarketState state)
    {
        Debug.Log("BuildMarketStateToday");
        MarketStateToday today = new MarketStateToday();
        today.time = DateTime.Now;
        List<string> goodsCodes = new List<string>();
        List<int> prices = new List<int>();
        List<int> quantitys = new List<int>();
        foreach (var aGoods in allGoods)
        {
            goodsCodes.Add(aGoods.codeName);
            int index = state.goodsCode.ToList().FindIndex(x => x == aGoods.codeName);
            int basePrice = aGoods.basePrice;
            int baseQuantiy = aGoods.baseQuantiy;
            if (index > -1)
            {
                //random if config
                if (!CommonUtils.IsArrayNullEmpty(state.priceMod))
                {

                }
                if (state.basePrice.Length > index) basePrice = state.basePrice[index];
                if (state.baseQuantiy.Length > index) baseQuantiy = state.baseQuantiy[index];
                prices.Add((int)Random.Range(basePrice * state.minRatePrice[index], basePrice * state.maxRatePrice[index]));
                quantitys.Add((int)Random.Range(baseQuantiy * state.minRateQty[index], baseQuantiy * state.maxRateQty[index]));
            }
            else
            {
                //else default price
                prices.Add((int)(basePrice * (1 + Random.Range(-state.otherPriceMod, state.otherPriceMod))));
                quantitys.Add(baseQuantiy);
            }

        }
        today.goodsCodes = goodsCodes.ToArray();
        today.prices = prices.ToArray();
        today.quantitys = quantitys.ToArray();

        if (state.crewMinMaxQtyPrice != null && state.crewMinMaxQtyPrice.Length == 4)
        {
            today.crewAvaiable = Random.Range(state.crewMinMaxQtyPrice[0], state.crewMinMaxQtyPrice[1] + 1);
            today.crewPrice = Random.Range(state.crewMinMaxQtyPrice[2], state.crewMinMaxQtyPrice[3] + 1);
        }
        return today;
    }

    public static ScriptableMarketState RandomStateByProbability(ScriptableMarketState[] source)
    {
        return source[CommonUtils.RandomByRate(source.Select(x => x.probability).ToArray())];
    }
}