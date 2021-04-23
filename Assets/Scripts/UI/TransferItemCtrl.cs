using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransferItemCtrl : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Slider itemQuantitySlider;
    public Text itemMaxQuanity;

    public Text txtExtraInfo;
    public Text weightShipFrom;
    public Text weightShipTo;

    public InputField inputQuantity;

    public System.Func<int[], bool> OnBeforeDoneTransfer;
    public UnityAction<int[]> OnDoneTransfer;
    public int fromInvIndex;
    public int toInvIndex;

    public ScriptableShipGoods transferGoods;
    public Ship fromShip;
    public Ship toShip;
    public ScriptableShipCustom fromShipData;
    public ScriptableShipCustom toShipData;

    private const string TEMPLATE_SHIP_WEIGHT = "Ship {0}({1}):{2:N0}/{3:N0}";
    private string template_extra_info = "";
    private string template_ship_from = "";
    private string template_ship_to = "";


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
        if (template_ship_from == "") template_ship_from = weightShipFrom.text;
        if (template_ship_to == "") template_ship_to = weightShipTo.text;
    }

    public void ShowShipWeightStatus()
    {
        float value = (int)itemQuantitySlider.value * transferGoods.weight;

        weightShipFrom.text = template_ship_from
        .Replace("{code}", (fromInvIndex + 1).ToString())
        .Replace("{name}", fromShipData.shipName?.Length > 0 ? fromShipData.shipName : "No Name")
        .Replace("{value}", CommonUtils.FormatNumber(value))
        .Replace("{curWeight}", CommonUtils.FormatNumber(ShipHelper.CalculateAllCargoWeight(fromShipData) - value))
        .Replace("{totalWeight}", CommonUtils.FormatNumber(fromShipData.PeakData.capacity));

        weightShipTo.text = template_ship_to
        .Replace("{code}", (toInvIndex + 1).ToString())
        .Replace("{name}", toShipData.shipName?.Length > 0 ? toShipData.shipName : "No Name")
        .Replace("{value}", CommonUtils.FormatNumber(value))
        .Replace("{curWeight}", CommonUtils.FormatNumber(ShipHelper.CalculateAllCargoWeight(toShipData) + value))
        .Replace("{totalWeight}", CommonUtils.FormatNumber(toShipData.PeakData.capacity));

        // weightShipFrom.text = string.Format(TEMPLATE_SHIP_WEIGHT,
        // fromInvIndex + 1,
        // fromShipData.shipName,
        // ShipHelper.CalculateAllCargoWeight(fromShipData) - (int)itemQuantitySlider.value * transferGoods.weight,
        // fromShipData.PeakData.capacity
        // );
        // weightShipTo.text = string.Format(TEMPLATE_SHIP_WEIGHT,
        // toInvIndex + 1,
        // toShipData.shipName,
        // ShipHelper.CalculateAllCargoWeight(toShipData) + (int)itemQuantitySlider.value * transferGoods.weight,
        // toShipData.PeakData.capacity
        // );
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowTransferItem(ScriptableShipGoods goods, int quanity, int fromInvIndex, Ship from, Ship to)
    {
        transferGoods = goods;
        fromShip = from;
        toShip = to;
        this.fromInvIndex = fromInvIndex;

        gameObject.SetActive(true);
        itemImage.sprite = goods.image;
        itemName.text = goods.itemName;
        itemQuantitySlider.maxValue = quanity;
        itemQuantitySlider.value = quanity;
        txtExtraInfo.text = template_extra_info.Replace("{weight}",
            CommonUtils.FormatNumber(goods.weight));
        UpdateSelectedValue();
    }

    public void ShowTransferItem(ScriptableShipGoods goods, int quanity, int fromInvIndex, int toInvIndex, ScriptableShipCustom from, ScriptableShipCustom to)
    {
        transferGoods = goods;
        fromShipData = from;
        toShipData = to;
        this.fromInvIndex = fromInvIndex;
        this.toInvIndex = toInvIndex;

        gameObject.SetActive(true);
        itemImage.sprite = goods.image;
        itemName.text = goods.itemName;
        itemQuantitySlider.maxValue = quanity;
        itemQuantitySlider.value = quanity;
        txtExtraInfo.text = template_extra_info.Replace("{weight}",
            CommonUtils.FormatNumber(goods.weight));
        UpdateSelectedValue();
    }

    private bool CheckWeightValid()
    {
        return true;
    }

    public void DoneTransfer()
    {
        if (CheckWeightValid())
        {
            int[] data = new int[] { (int)itemQuantitySlider.value, fromInvIndex };
            gameObject.SetActive(false);
            OnDoneTransfer?.Invoke(data);
        }
    }

    public void CloseTransfer()
    {
        gameObject.SetActive(false);
    }

    public bool preventChainUpdate = false;

    public void UpdateSelectedValue()
    {
        itemMaxQuanity.text = string.Format("{0}/{1}", (int)itemQuantitySlider.value, (int)itemQuantitySlider.maxValue);
        ShowShipWeightStatus();
        if (preventChainUpdate)
        {
            preventChainUpdate = false;
        }
        else
        {
            preventChainUpdate = true;
            inputQuantity.text = ((int)itemQuantitySlider.value).ToString();
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
            if (itemQuantitySlider.minValue <= value && itemQuantitySlider.maxValue >= value)
            {
                Debug.Log("Quantity Ok" + inputQuantity.text + "/" + value);
                if (itemQuantitySlider.value != value)
                {
                    itemQuantitySlider.value = value;
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
                inputQuantity.text = itemQuantitySlider.value.ToString();
            }
        }
        catch (FormatException e)
        {
            Debug.Log("Quantity Exception [" + inputQuantity.text + "]");
            preventChainUpdate = false;
        }
    }
}
