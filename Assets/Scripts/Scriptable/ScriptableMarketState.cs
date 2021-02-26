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
    public string description;

    public int probability;
    public string[] goodsCode;
    public float[] minRatePrice;
    public float[] maxRatePrice;
    public float[] minRateQty;
    public float[] maxRateQty;

    public int[] basePrice;
    public int[] baseQuantiy;

    public int[] crewMinMaxQtyPrice;
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
                if (state.basePrice.Length > index) basePrice = state.basePrice[index];
                if (state.baseQuantiy.Length > index) baseQuantiy = state.baseQuantiy[index];
                prices.Add((int)Random.Range(basePrice * state.minRatePrice[index], basePrice * state.maxRatePrice[index]));
                quantitys.Add((int)Random.Range(baseQuantiy * state.minRateQty[index], baseQuantiy * state.maxRateQty[index]));
            }
            else
            {
                prices.Add(basePrice);
                quantitys.Add(baseQuantiy);
            }

        }
        today.goodsCodes = goodsCodes.ToArray();
        today.prices = prices.ToArray();
        today.quantitys = quantitys.ToArray();

        if (state.crewMinMaxQtyPrice != null && state.crewMinMaxQtyPrice.Length == 4)
        {
            today.crewAvaiable = Random.Range(state.crewMinMaxQtyPrice[0], state.crewMinMaxQtyPrice[1]);
            today.crewPrice = Random.Range(state.crewMinMaxQtyPrice[2], state.crewMinMaxQtyPrice[3]);
        }
        return today;
    }

    public static ScriptableMarketState RandomStateByProbability(ScriptableMarketState[] source)
    {
        return source[CommonUtils.RandomByRate(source.Select(x => x.probability).ToArray())];
    }
}