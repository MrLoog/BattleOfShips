using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableBattleFlow", menuName = "BoS/Battle Flow", order = 15)]
public class ScriptableBattleFlow : MScriptableObject
{
    public enum FlowType
    {
        GameLevel, Self
    }

    public FlowType type = FlowType.GameLevel;
    public ConditionAction[] battlePoints;
    public ScriptableShipFactory[] shipFactorys;
    public ScriptableShipCustom[] ships;

    public GroupShip[] groupShips;
    public RewardBattle reward;
    public void ActiveFlow() => GEventManager.Instance.RegisterBattleFlow(this);

}

[Serializable]
public class GroupShip
{
    public ScriptableShipFactory[] shipFactorys;
}

[Serializable]
public class RewardBattle
{
    public ScriptableShipCustom[] ships;
    public int[] gold;

    public int[] Gold
    {
        get
        {
            if (!CommonUtils.IsArrayNullEmpty(gold) && goldModRate != 0)
            {
                int[] newArr = new int[gold.Length];
                int i = 0;
                gold?.ToList().ForEach(x =>
               {
                   newArr[i] = (int)(x * (1 + UnityEngine.Random.Range(-goldModRate, goldModRate)));
               });
                return newArr;
            }
            return gold;
        }
    }
    public long[] exp;
    public long[] Exp
    {
        get
        {
            if (!CommonUtils.IsArrayNullEmpty(exp) && expModRate != 0)
            {
                long[] newArr = new long[exp.Length];
                int i = 0;
                exp?.ToList().ForEach(x =>
               {
                   newArr[i] = (int)(x * (1 + UnityEngine.Random.Range(-expModRate, expModRate)));
               });
                return newArr;
            }
            return exp;
        }
    }
    public int[] gem;
    public int[] Gem
    {
        get
        {
            if (!CommonUtils.IsArrayNullEmpty(gem) && gemModRate != 0)
            {
                int[] newArr = new int[gem.Length];
                int i = 0;
                gem?.ToList().ForEach(x =>
               {
                   newArr[i] = (int)(x * (1 + UnityEngine.Random.Range(-gemModRate, gemModRate)));
               });
                return newArr;
            }
            return gem;
        }
    }
    internal string[] goodsCode;
    internal int[] goodsQty;
    public int[] GoodsQty
    {
        get
        {
            if (!CommonUtils.IsArrayNullEmpty(goodsQty) && goodsModRate != 0)
            {
                int[] newArr = new int[goodsQty.Length];
                int i = 0;
                goodsQty?.ToList().ForEach(x =>
               {
                   newArr[i] = (int)(x * (1 + UnityEngine.Random.Range(-goodsModRate, goodsModRate)));
               });
                return newArr;
            }
            return goodsQty;
        }
    }

    [Range(0, 1)]
    public float goldModRate = 0;
    [Range(0, 1)]
    public float expModRate = 0;
    [Range(0, 1)]
    public float gemModRate = 0;
    [Range(0, 1)]
    public float goodsModRate = 0;

    public void Clear()
    {
        ships = null;
        gold = null;
        exp = null;
        gem = null;
        goodsCode = null;
        goodsQty = null;
    }

    internal void Union(RewardBattle reward)
    {
        ships = CommonUtils.AddElemToArray(ships, reward.ships);
        gold = CommonUtils.AddElemToArray(gold, reward.Gold);
        exp = CommonUtils.AddElemToArray(exp, reward.Exp);
        gem = CommonUtils.AddElemToArray(gem, reward.Gem);
        goodsCode = CommonUtils.AddElemToArray(goodsCode, reward.goodsCode);
        goodsQty = CommonUtils.AddElemToArray(goodsQty, reward.GoodsQty);
    }
}