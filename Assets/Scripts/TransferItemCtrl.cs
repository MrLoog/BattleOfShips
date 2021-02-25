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


    // Start is called before the first frame update
    void Start()
    {

    }

    public void ShowShipWeightStatus()
    {
        weightShipFrom.text = string.Format(TEMPLATE_SHIP_WEIGHT,
        fromInvIndex + 1,
        fromShipData.shipName,
        ShipHelper.CalculateAllCargoWeight(fromShipData) - (int)itemQuantitySlider.value * transferGoods.weight,
        fromShipData.PeakData.capacity
        );
        weightShipTo.text = string.Format(TEMPLATE_SHIP_WEIGHT,
        toInvIndex + 1,
        toShipData.shipName,
        ShipHelper.CalculateAllCargoWeight(toShipData) + (int)itemQuantitySlider.value * transferGoods.weight,
        toShipData.PeakData.capacity
        );
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
