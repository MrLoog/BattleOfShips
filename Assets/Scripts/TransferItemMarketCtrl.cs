using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransferItemMarketCtrl : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Slider itemQuantitySlider;
    public Text itemMaxQuanity;
    public Text weightShipTo;

    public InputField inputQuantity;

    public System.Func<int[], bool> OnBeforeDoneTransfer;
    public UnityAction<int[]> OnDoneTransfer;
    public int fromInvIndex;

    public ScriptableShipGoods transferGoods;
    public ScriptableShipCustom fromShipData;
    MarketStateToday marketState;

    public bool IsSellMode => fromInvIndex == 0;
    public Text lblAmount;
    public const string TEMPLATE_GOLD_AMOUNT_SELL = "Gold: <color=blue>+{0:N0}</color>";
    public const string TEMPLATE_GOLD_AMOUNT_BUY = "Gold: <color=red>-{0:N0}</color>";
    private const string TEMPLATE_SHIP_WEIGHT = "Ship {0}({1}):{2:N0}/{3:N0}";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowShipWeightStatus()
    {
        weightShipTo.text = string.Format(TEMPLATE_SHIP_WEIGHT,
        fromInvIndex + 1,
        fromShipData.shipName,
        ShipHelper.CalculateAllCargoWeight(fromShipData) + ((int)itemQuantitySlider.value * transferGoods.weight * (IsSellMode ? -1 : 1)),
        fromShipData.PeakData.capacity
        );
    }
    public void ShowGoldAmount()
    {
        lblAmount.text = string.Format(IsSellMode ? TEMPLATE_GOLD_AMOUNT_SELL : TEMPLATE_GOLD_AMOUNT_BUY,
        IsSellMode ?
        marketState.GoldReceivedBySell(transferGoods.codeName, (int)itemQuantitySlider.value)
        : marketState.GoldLostByBuy(transferGoods.codeName, (int)itemQuantitySlider.value)
        );
    }
    public void ShowTransferItem(ScriptableShipGoods goods, int quanity, int fromInvIndex, ScriptableShipCustom from, MarketStateToday marketStateToday)
    {
        transferGoods = goods;
        fromShipData = from;
        marketState = marketStateToday;

        this.fromInvIndex = fromInvIndex;

        gameObject.SetActive(true);
        itemImage.sprite = goods.image;
        itemName.text = goods.itemName;
        itemQuantitySlider.maxValue = quanity;
        itemQuantitySlider.value = quanity;
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
        ShowGoldAmount();
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
        catch (FormatException)
        {
            Debug.Log("Quantity Exception [" + inputQuantity.text + "]");
            preventChainUpdate = false;
        }
    }
}
