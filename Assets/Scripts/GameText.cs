﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameText
{
    public const string CONFIRM_REPAIR_TITLE = "cf_repair_tit";
    public const string CONFIRM_REPAIR_CONTENT = "cf_repair_cont";
    public const string CONFIRM_COMMON_TITLE = "cf_cm_tit";
    public const string CONFIRM_COMMON_YES = "cf_cm_yes";
    public const string CONFIRM_COMMON_NO = "cf_cm_no";
    public const string CONFIRM_COMMON_OK = "cf_cm_ok";
    public const string CONFIRM_GOLD_NOT_ENOUGH_TITLE = "cf_gold_short_tit";

    public const string CONFIRM_GOLD_NOT_ENOUGH_CONTENT = "cf_gold_short_cont";
    public const string CONFIRM_DEFAULT_TITLE = "cf_df_tit";
    public const string CONFIRM_DEFAULT_CONTENT = "cf_df_cont";
    public const string CONFIRM_DEFAULT_DONE = "cf_df_done";
    public const string CONFIRM_DEFAULT_CANCEL = "cf_df_cancel";
    public const string CONFIRM_SELL_TITLE = "cf_sell_tit";
    public const string CONFIRM_SELL_CONTENT = "cf_sell_cont";
    public const string CONFIRM_SELL_CONTENT_EXTRA = "cf_sell_cont_extra";
    public const string CONFIRM_BUY_SHIP_TITLE = "cf_buy_ship_tit";
    public const string CONFIRM_BUY_SHIP_CONTENT = "cf_buy_ship_cont";
    public const string CONFIRM_SHOP_REFRESH_TITLE = "cf_shop_refresh_tit";
    public const string CONFIRM_SHOP_REFRESH_CONTENT = "cf_shop_refresh_cont";
    public const string CONFIRM_BATTLE_RESTART_TITLE = "cf_battle_restart_tit";
    public const string CONFIRM_BATTLE_RESTART_CONTENT = "cf_battle_restart_cont";
    public const string CONFIRM_RETURN_TOWN_RUN_TITLE = "cf_return_town_run_tit";
    public const string CONFIRM_RETURN_TOWN_RUN_CONTENT = "cf_return_town_run_cont";
    public const string TOAST_DECLARE_WIN = "toast_declare_win";
    public const string TOAST_NOT_ENOUGH_GOLD = "toast_not_enough_gold";
    public const string TOAST_NOT_ENOUGH_GEM = "toast_not_enough_gem";
    public const string TOAST_CANNOT_RETURN_TOWN = "toast_return_town_refuse";
    public const string CONFIRM_RETURN_TOWN_TITLE = "cf_return_town_tit";
    public const string CONFIRM_RETURN_TOWN_CONTENT = "cf_return_town_cont";
    public static Dictionary<string, string> dict = new Dictionary<string, string>()
    {
        {CONFIRM_REPAIR_TITLE,"Are You Sure?"},
        {CONFIRM_REPAIR_CONTENT,"Repair cost {0:N0} gold, are you sure?"},
        {CONFIRM_COMMON_TITLE,"Are You Sure?"},
        {CONFIRM_COMMON_YES,"Yes"},
        {CONFIRM_COMMON_NO,"No"},
        {CONFIRM_COMMON_OK,"OK"},
        {CONFIRM_GOLD_NOT_ENOUGH_TITLE,"Not Enough Gold"},
        {CONFIRM_GOLD_NOT_ENOUGH_CONTENT,"{0:N0} gold required"},
        {CONFIRM_DEFAULT_TITLE,"No Title"},
        {CONFIRM_DEFAULT_CONTENT,"Are you sure?"},
        {CONFIRM_DEFAULT_DONE,"Done"},
        {CONFIRM_DEFAULT_CANCEL,"Cancel"},
        {CONFIRM_SELL_TITLE,"Sell Ship?"},
        {CONFIRM_SELL_CONTENT,"Ship price is {0:N0} gold. Cargo price is {1:N0} gold.{2} Are you sure sell ship?"},
        {CONFIRM_SELL_CONTENT_EXTRA,"{0} Crew will be lost."},
        {CONFIRM_BUY_SHIP_TITLE,"Buy this Ship?"},
        {CONFIRM_BUY_SHIP_CONTENT,"Are you sure you want buy this ship?. Price {0:N0}"},
        {CONFIRM_SHOP_REFRESH_TITLE,"Refresh Shop?"},
        {CONFIRM_SHOP_REFRESH_CONTENT,"Refresh Shop cost {0:N0} gem,Are you sure?"},
        {CONFIRM_BATTLE_RESTART_TITLE,"The Game will be restart"},
        {CONFIRM_BATTLE_RESTART_CONTENT,"You are defeated. Try Again."},
        {TOAST_DECLARE_WIN,"Congratulations, You are Winner!!! Let's Loot all them and return Town"},
        {TOAST_CANNOT_RETURN_TOWN,"Cannot Return Town when you are in battle."},
        {CONFIRM_RETURN_TOWN_TITLE,"Are you sure return Town?"},
        {CONFIRM_RETURN_TOWN_CONTENT,"Make sure you take all before return to town."},
        {TOAST_NOT_ENOUGH_GOLD,"Not Enough Gold"},
        {TOAST_NOT_ENOUGH_GEM,"Not Enough Gem"},
        {CONFIRM_RETURN_TOWN_RUN_TITLE,"Run from Battle?"},
        {CONFIRM_RETURN_TOWN_RUN_CONTENT,"Are you sure, you want to run from battle return to Town?"},
    };

    public static string GetText(string key)
    {
        string rs = "";
        dict.TryGetValue(key, out rs);
        return rs;
    }

    public static string GetText(string key, params object[] args)
    {
        string rs = "";
        dict.TryGetValue(key, out rs);
        return string.Format(rs, args);
    }
}