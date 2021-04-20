using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransferCrewCtrl : MonoBehaviour
{
    private const string CrewText = "Crew";
    public Image crewImage;
    public Text crewName;
    public Slider crewQuantitySlider;
    public Text txtExtraInfo;
    public Text crewMaxQuanity;

    public Text playerGold;
    public Text crewShipTo;
    public Text crewShipFrom;

    public Image imgTransfer;
    public InputField inputQuantity;

    public System.Func<int[], bool> OnBeforeDoneTransfer;
    public UnityAction<int[]> OnDoneTransfer;
    public UnityAction<int[]> OnDoneHire;

    public int fromInvIndex;
    public int toInvIndex;
    public Ship fromShip;
    public Ship toShip;
    public ScriptableShipCustom fromShipData;
    public ScriptableShipCustom toShipData;

    private MarketStateToday marketState;

    public bool shopMode = false;
    public bool IsShopMode
    {
        get
        {
            return shopMode;
        }
        set
        {
            shopMode = value;
            playerGold.gameObject.SetActive(shopMode);
            crewShipFrom.gameObject.SetActive(!shopMode);
        }
    }
    // public const string TEMPLATE_GOLD_AMOUNT_SELL = "Gold: <color=blue>+{0:N0}</color>";
    public const string TEMPLATE_GOLD_AMOUNT_BUY = "Gold: <color=red>-{0:N0}</color>";
    private const string TEMPLATE_SHIP_CREW = "Ship {0}({1}):{2:N0}/{3:N0}";
    private string template_extra_info = "";
    private string template_ship_from = "";
    private string template_ship_to = "";
    private string template_market = "";


    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        LoadTemplate();
    }

    private void LoadTemplate()
    {
        if (template_extra_info == "") template_extra_info = txtExtraInfo.text;
        if (template_ship_to == "") template_ship_to = crewShipTo.text;
        if (template_ship_from == "") template_ship_from = crewShipFrom.text;
        if (template_market == "") template_market = playerGold.text;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowShipCrewStatus()
    {
        int value = (int)crewQuantitySlider.value;


        if (!IsShopMode)
        {
            crewShipFrom.text = template_ship_from
                .Replace("{code}", (fromInvIndex + 1).ToString())
                .Replace("{name}", fromShipData.shipName?.Length > 0 ? fromShipData.shipName : "No Name")
                .Replace("{value}", CommonUtils.FormatNumber(value))
                .Replace("{curCrew}", CommonUtils.FormatNumber(fromShipData.curShipData.maxCrew - value))
                .Replace("{maxCrew}", CommonUtils.FormatNumber(fromShipData.PeakData.maxCrew));

            // crewShipFrom.text = string.Format(TEMPLATE_SHIP_CREW,
            // fromInvIndex + 1,
            // fromShipData.shipName,
            // fromShipData.curShipData.maxCrew - (int)crewQuantitySlider.value,
            // fromShipData.PeakData.maxCrew
            // );
        }

        crewShipTo.text = template_ship_to
                .Replace("{code}", (toInvIndex + 1).ToString())
                .Replace("{name}", toShipData.shipName?.Length > 0 ? toShipData.shipName : "No Name")
                .Replace("{value}", CommonUtils.FormatNumber(value))
                .Replace("{curCrew}", CommonUtils.FormatNumber(toShipData.curShipData.maxCrew + value))
                .Replace("{maxCrew}", CommonUtils.FormatNumber(toShipData.PeakData.maxCrew));

        // crewShipTo.text = string.Format(TEMPLATE_SHIP_CREW,
        // toInvIndex + 1,
        // toShipData.shipName,
        // toShipData.curShipData.maxCrew + (int)crewQuantitySlider.value,
        // toShipData.PeakData.maxCrew
        // );
    }
    public void ShowGoldAmount()
    {
        if (!IsShopMode) return;
        int crew = (int)crewQuantitySlider.value;

        playerGold.text = template_market
                .Replace("{gold}", CommonUtils.FormatNumber(marketState.crewPrice * (int)crewQuantitySlider.value))
                .Replace("{crew}", CommonUtils.FormatNumber(crew))
                .Replace("{curCrew}", CommonUtils.FormatNumber(marketState.crewAvaiable - crew))
                .Replace("{maxCrew}", CommonUtils.FormatNumber(marketState.crewAvaiable));

        // playerGold.text = string.Format(TEMPLATE_GOLD_AMOUNT_BUY,
        // marketState.crewPrice * (int)crewQuantitySlider.value
        // );
    }
    public void ShowTransferCrew(int quanity, int fromInvIndex, Ship from, Ship to)
    {
        fromShip = from;
        toShip = to;
        this.fromInvIndex = fromInvIndex;

        gameObject.SetActive(true);
        crewName.text = CrewText;
        crewQuantitySlider.maxValue = quanity;
        crewQuantitySlider.value = quanity;
        UpdateSelectedValue();
    }

    public void ShowTransferCrew(int quanity, int fromInvIndex, int toInvIndex, ScriptableShipCustom from, ScriptableShipCustom to)
    {
        Debug.Log("ShowTransferCrew " + quanity);
        IsShopMode = false;
        fromShipData = from;
        toShipData = to;
        this.fromInvIndex = fromInvIndex;
        this.toInvIndex = toInvIndex;

        gameObject.SetActive(true);
        crewName.text = CrewText;
        crewQuantitySlider.maxValue = quanity;
        crewQuantitySlider.value = quanity;

        txtExtraInfo.gameObject.SetActive(false);
        imgTransfer.gameObject.SetActive(true);

        UpdateSelectedValue();
    }

    public void ShowHireCrew(int quanity, int toInvIndex, ScriptableShipCustom to, MarketStateToday marketStateToday)
    {
        Debug.Log("ShowTransferCrew " + quanity);
        IsShopMode = true;
        toShipData = to;
        this.toInvIndex = toInvIndex;
        marketState = marketStateToday;

        gameObject.SetActive(true);
        crewName.text = CrewText;
        crewQuantitySlider.maxValue = quanity;
        crewQuantitySlider.value = quanity;

        imgTransfer.gameObject.SetActive(false);
        txtExtraInfo.gameObject.SetActive(true);
        txtExtraInfo.text = template_extra_info.Replace(
            "{price}",
            CommonUtils.FormatNumber(marketState.crewPrice)
        );

        UpdateSelectedValue();
    }

    private bool CheckCapacityValid()
    {
        return true;
    }

    public void DoneTransfer()
    {
        if (CheckCapacityValid())
        {
            int[] data = new int[] { (int)crewQuantitySlider.value, IsShopMode ? toInvIndex : fromInvIndex };
            gameObject.SetActive(false);
            if (IsShopMode) OnDoneHire?.Invoke(data);
            else OnDoneTransfer?.Invoke(data);
        }
    }

    public void CloseTransfer()
    {
        gameObject.SetActive(false);
    }

    public bool preventChainUpdate = false;

    public void UpdateSelectedValue()
    {
        crewMaxQuanity.text = string.Format("{0}/{1}", (int)crewQuantitySlider.value, (int)crewQuantitySlider.maxValue);
        ShowShipCrewStatus();
        ShowGoldAmount();
        if (preventChainUpdate)
        {
            preventChainUpdate = false;
        }
        else
        {
            preventChainUpdate = true;
            inputQuantity.text = ((int)crewQuantitySlider.value).ToString();
        }
    }
    private string oldValueInput;
    public void OnInputChanged()
    {
        if (preventChainUpdate)
        {
            preventChainUpdate = false;
            return;
        }
        preventChainUpdate = true;
        try
        {
            int value = Int32.Parse(inputQuantity.text);
            if (crewQuantitySlider.minValue <= value && crewQuantitySlider.maxValue >= value)
            {
                Debug.Log("Quantity Ok" + inputQuantity.text + "/" + value);
                if (crewQuantitySlider.value != value)
                {
                    crewQuantitySlider.value = value;
                }
                else
                {
                    preventChainUpdate = false;
                }
            }
            else
            {
                Debug.Log("Quantity Ignore" + inputQuantity.text + "/" + value);
                preventChainUpdate = false;
                inputQuantity.text = crewQuantitySlider.value.ToString();
            }
        }
        catch (FormatException)
        {
            Debug.Log("Quantity Exception [" + inputQuantity.text + "]");
            preventChainUpdate = false;
        }
    }
}
