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

    public System.Func<int[], bool> OnBeforeDoneTransfer;
    public UnityAction<int[]> OnDoneTransfer;
    public int fromInvIndex;

    public ScriptableShipGoods transferGoods;
    public Ship fromShip;
    public Ship toShip;


    // Start is called before the first frame update
    void Start()
    {

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

    public void UpdateSelectedValue()
    {
        itemMaxQuanity.text = string.Format("{0}/{1}", (int)itemQuantitySlider.value, (int)itemQuantitySlider.maxValue);
    }
}
