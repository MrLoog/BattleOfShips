using System.Collections;
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
