﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShipInventoryCtrl : MonoBehaviour
{
    public enum InventoryMode
    {
        None, View, Transfer, Shop
    }

    private InventoryMode mode = InventoryMode.View;
    public InventoryMode Mode
    {
        set
        {
            mode = value;

            if (InventoryMode.Transfer.Equals(value))
            {
                panelGroup2.SetActive(true);
            }
            else
            {
                panelGroup2.SetActive(false);
            }

            if (InventoryMode.Shop.Equals(value))
            {
                ShipSelected1.GetComponent<DropdownItemDisabled>().Clear();
                ShipCrew1Btn.GetComponentInChildren<Text>().text = "Hire Crew";
                marketStateToday = GameManager.Instance.GetMarketStateToday();
                DisplayMarket();
                panelGroup3.SetActive(true);
            }
            else
            {
                ShipCrew1Btn.GetComponentInChildren<Text>().text = "Add Crew";
                panelGroup3.SetActive(false);
            }
        }
        get
        {
            return mode;
        }
    }


    public MarketStateToday marketStateToday;
    private List<ScriptableShipCustom> AvaiableShips = new List<ScriptableShipCustom>();

    private int firstIndex;

    public int FirstIndex
    {
        set
        {
            ShipSelected2.GetComponent<DropdownItemDisabled>().EnableOption(firstIndex, true);
            firstIndex = value;
            ShipSelected2.GetComponent<DropdownItemDisabled>().EnableOption(firstIndex, false);
            OnSetShipSelected1();
        }
        get
        {
            return firstIndex;
        }
    }
    private int secondIndex;
    public int SecondIndex
    {
        set
        {
            ShipSelected1.GetComponent<DropdownItemDisabled>().EnableOption(secondIndex, true);
            secondIndex = value;
            ShipSelected1.GetComponent<DropdownItemDisabled>().EnableOption(secondIndex, !InventoryMode.Transfer.Equals(Mode));
            OnSetShipSelected2();
        }
        get
        {
            return secondIndex;
        }
    }

    public GameObject panelInventory;
    public GameObject panelInventoryOther;
    public GameObject panelInventoryMarket;
    public GameObject prefabItem;

    public GameObject panelTransferItem;
    public GameObject panelTransferCrew;
    public GameObject panelTransferItemMarket;

    public GameObject InventoryCanvas;
    public GameObject panelGroup1;

    public GameObject panelGroup2;
    public GameObject panelGroup3;

    public Dropdown ShipSelected1;
    public Dropdown ShipSelected2;

    private List<ShipInventory> inventories = new List<ShipInventory>();
    public bool isShow = false;

    private List<ScriptableShipGoods> goods;

    List<ScriptableShipGoods> Goods
    {
        get
        {
            if (goods == null)
            {
                goods = MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS).ToList();
            }
            return goods;
        }
    }
    TransferItemCtrl transferPanel;
    TransferCrewCtrl transferCrewCtrl;
    TransferItemMarketCtrl transferItemMarketCtrl;
    public Button ShipCrew1Btn;
    public Button ShipCrew2Btn;

    public Text ShipCrew1;
    public Text ShipCrew2;

    public Text CapacityShip1;
    public Text CapacityShip2;

    public Text PlayerGold;
    private const string TEMPLATE_CREW_STATUS = "{0:N0}/{1:N0}";
    private const string TEMPLATE_PLAYER_GOLD = "<color=yellow>Gold: {0:N0}</color>";
    private const string TEMPLATE_SHIP_CAPACITY = "{0:N0}/{1:N0}";

    public UnityAction OnHideInventory;

    // Start is called before the first frame update
    void Start()
    {
        transferPanel = panelTransferItem.GetComponent<TransferItemCtrl>();
        transferPanel.OnDoneTransfer += TransferItemSelected;

        transferCrewCtrl = panelTransferCrew.GetComponent<TransferCrewCtrl>();
        transferCrewCtrl.OnDoneTransfer += TransferCrewSelected;
        transferCrewCtrl.OnDoneHire += HireCrewSelected;

        transferItemMarketCtrl = panelTransferItemMarket.GetComponent<TransferItemMarketCtrl>();
        transferItemMarketCtrl.OnDoneTransfer += TransferItemMarketSelected;

        GameManager.Instance.OnGoldAccountChanged += DisplayPlayerGold;
        DisplayPlayerGold();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGoldAccountChanged += DisplayPlayerGold;
        DisplayPlayerGold();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldAccountChanged -= DisplayPlayerGold;
    }

    public void DisplayCapacity(int choice = 0)
    {
        if (choice >= 0 && FirstIndex >= 0 && FirstIndex < AvaiableShips.Count)
        {
            CapacityShip1.text = string.Format(
                TEMPLATE_SHIP_CAPACITY,
                AvaiableShips[FirstIndex].PeakData.capacity - ShipHelper.CalculateAvaiableCapacity(AvaiableShips[FirstIndex]),
                AvaiableShips[FirstIndex].PeakData.capacity
                );
        }
        if (choice <= 0 && SecondIndex >= 0 && SecondIndex < AvaiableShips.Count)
        {
            CapacityShip2.text = string.Format(
                TEMPLATE_SHIP_CAPACITY,
                AvaiableShips[SecondIndex].PeakData.capacity - ShipHelper.CalculateAvaiableCapacity(AvaiableShips[SecondIndex]),
                AvaiableShips[SecondIndex].PeakData.capacity
                );
        }
    }

    private void DisplayPlayerGold(int gold = 0)
    {
        PlayerGold.text = string.Format(TEMPLATE_PLAYER_GOLD, GameManager.Instance.GameData.gold);
    }

    public void RegisterAvaiableShip(ScriptableShipCustom[] lstAvaiable, int mainIndex = 0)
    {
        AvaiableShips = lstAvaiable.ToList();
        DisplayDropdownShip();
        FirstIndex = mainIndex;
    }

    private void TransferItemMarketSelected(int[] result)
    {
        int quantity = result[0];
        int fromInvIndex = result[1]; //0 mean sell, 1 mean buy

        ShipInventory inventoryShip = AvaiableShips[FirstIndex].inventory;
        string[] goodsCodesAdd = null;
        int[] quantityAdd = null;
        string[] goodsCodesDeduct = null;
        int[] quantityDeduct = null;


        if (fromInvIndex == 0)
        {
            //sell
            goodsCodesAdd = marketStateToday.goodsCodes;
            quantityAdd = marketStateToday.quantitys;
            goodsCodesDeduct = inventoryShip.goodsCode;
            quantityDeduct = inventoryShip.quantity;
        }
        else
        {
            //buy
            goodsCodesAdd = inventoryShip.goodsCode;
            quantityAdd = inventoryShip.quantity;
            goodsCodesDeduct = marketStateToday.goodsCodes;
            quantityDeduct = marketStateToday.quantitys;
        }

        quantityDeduct[indexTransfer] -= quantity;
        int i = 0;
        for (; i < goodsCodesAdd.Length; i++)
        {
            if (goodsCodesAdd[i] == goodsCodesDeduct[indexTransfer])
            {
                quantityAdd[i] += quantity;
                break;
            }
        }
        if (i == goodsCodesAdd.Length)
        {
            goodsCodesAdd.Concat(new string[] { goodsCodesDeduct[indexTransfer] });
            quantityAdd.Concat(new int[] { quantity });
        }

        if (quantityDeduct[indexTransfer] == 0)
        {
            // inventoryDeduct.goodsCode[indexTransfer] = null;
            // inventoryDeduct.quantity = inventoryDeduct.quantity.Where(x => x > 0).ToArray();
            // inventoryDeduct.goodsCode = inventoryDeduct.goodsCode.Where(x => x != null).ToArray();
        }

        //gold account balance
        if (fromInvIndex == 0)
        {
            //sell 70% price
            GameManager.Instance.AddGold(marketStateToday.GoldReceivedBySell(
                goodsCodesDeduct[indexTransfer],
                quantity
            ));
        }
        else
        {
            //buy
            GameManager.Instance.DeductGold(
                marketStateToday.GoldLostByBuy(indexTransfer, quantity)
            );
        }

        ShowInventoryDetails(
                panelInventory,
                FirstIndex
            );

        DisplayMarket();
    }

    private void TransferItemSelected(int[] result)
    {
        int quantity = result[0];
        int fromInvIndex = result[1];
        ShipInventory inventoryDeduct = AvaiableShips[fromInvIndex].inventory;
        ShipInventory inventoryAdd = AvaiableShips[fromInvIndex == FirstIndex ? SecondIndex : FirstIndex].inventory;
        inventoryDeduct.quantity[indexTransfer] -= quantity;
        int i = 0;
        for (; i < inventoryAdd.goodsCode.Length; i++)
        {
            if (inventoryAdd.goodsCode[i] == inventoryDeduct.goodsCode[indexTransfer])
            {
                inventoryAdd.quantity[i] += quantity;
                break;
            }
        }
        Debug.Log("transfer inventory " + i);
        if (i == inventoryAdd.goodsCode.Length)
        {
            inventoryAdd.goodsCode = inventoryAdd.goodsCode.Concat(new string[] { inventoryDeduct.goodsCode[indexTransfer] }).ToArray();
            inventoryAdd.quantity = inventoryAdd.quantity.Concat(new int[] { quantity }).ToArray();
        }

        if (inventoryDeduct.quantity[indexTransfer] == 0)
        {
            inventoryDeduct.goodsCode[indexTransfer] = null;
            inventoryDeduct.quantity = inventoryDeduct.quantity.Where(x => x > 0).ToArray();
            inventoryDeduct.goodsCode = inventoryDeduct.goodsCode.Where(x => x != null).ToArray();
        }

        ShowInventoryDetails(
                panelInventory,
                FirstIndex
            );

        if (ShipInventoryCtrl.InventoryMode.Transfer.Equals(Mode))
        {
            ShowInventoryDetails(
                    panelInventoryOther,
                    SecondIndex
                );
        }
    }

    private void TransferCrewSelected(int[] result)
    {
        int quantity = result[0];
        int fromInvIndex = result[1];

        ScriptableShipCustom shipFrom = AvaiableShips[fromInvIndex];
        ScriptableShipCustom shipTo = AvaiableShips[fromInvIndex == FirstIndex ? SecondIndex : FirstIndex];

        shipFrom.curShipData.maxCrew -= quantity;
        shipTo.curShipData.maxCrew += quantity;

        ShowStatusCrew();
    }

    private void HireCrewSelected(int[] result)
    {
        int quantity = result[0];
        int toInvIndex = result[1];

        ScriptableShipCustom shipTo = AvaiableShips[toInvIndex];

        shipTo.curShipData.maxCrew += quantity;
        marketStateToday.crewAvaiable -= quantity;

        GameManager.Instance.DeductGold(
            quantity * marketStateToday.crewPrice
        );

        ShowStatusCrew();
    }

    private void ShowStatusCrew()
    {
        ShipCrew1.text = string.Format(TEMPLATE_CREW_STATUS,
        AvaiableShips[FirstIndex].curShipData.maxCrew,
        AvaiableShips[FirstIndex].PeakData.maxCrew
        );
        ShipCrew1Btn.enabled = AvaiableShips[FirstIndex].PeakData.maxCrew > AvaiableShips[FirstIndex].curShipData.maxCrew;
        if (secondIndex >= 0 && SecondIndex < AvaiableShips.Count)
        {
            ShipCrew2.text = string.Format(TEMPLATE_CREW_STATUS,
        AvaiableShips[SecondIndex].curShipData.maxCrew,
        AvaiableShips[SecondIndex].PeakData.maxCrew
        );
            ShipCrew2Btn.enabled = AvaiableShips[SecondIndex].PeakData.maxCrew > AvaiableShips[SecondIndex].curShipData.maxCrew;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ToggerInventory()
    {
        isShow = !isShow;
        if (isShow) ShowInventory();
        else HideInventory();
    }

    public void ShowInventory(InventoryMode mode = InventoryMode.None, int first = -1, int second = -1)
    {
        if (AvaiableShips.Count == 0) return;
        if (mode != InventoryMode.None) Mode = mode;

        if (first != -1 && AvaiableShips.Count > first) FirstIndex = first;
        else if (FirstIndex != 0 && AvaiableShips.Count <= FirstIndex) FirstIndex = 0;

        if (InventoryMode.Transfer.Equals(Mode))
        {
            if (AvaiableShips.Count == 1) Mode = InventoryMode.View;
            else
            {
                if (second != -1 && second != FirstIndex && AvaiableShips.Count > second)
                {
                    SecondIndex = second;
                }
                else
                {
                    SecondIndex = (FirstIndex + 1) < AvaiableShips.Count ? (FirstIndex + 1) : (FirstIndex - 1);
                }
            }
        }

        InventoryCanvas.SetActive(true);
        // InventoryCanvas.SetActive(true);
        // ShowInventoryDetails(
        //         panelInventory,
        //         FirstIndex
        //     );

        if (InventoryMode.Transfer.Equals(Mode))
        {
            // panelInventoryOther.SetActive(true);
            // ShowInventoryDetails(
            //     panelInventoryOther,
            //     SecondIndex
            // );
        }
        else
        {
            // panelInventoryOther.SetActive(false);
        }
    }


    private void ClearAllChilds(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }

    public const string TEMPLATE_DROPDOWN_TEXT = "Ship #{0} - {1}";
    private void DisplayDropdownShip()
    {
        List<Dropdown.OptionData> lst1 = new List<Dropdown.OptionData>();
        for (int i = 0; i < AvaiableShips.Count; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(string.Format(TEMPLATE_DROPDOWN_TEXT, i + 1, AvaiableShips[i].shipName));
            lst1.Add(option);
        }
        ShipSelected1.options = lst1;
        ShipSelected2.options = lst1;
    }

    public void OnChangedSelected1()
    {
        FirstIndex = ShipSelected1.value;
    }

    public void OnChangedSelected2()
    {
        SecondIndex = ShipSelected2.value;
    }

    private void OnSetShipSelected1()
    {
        if (FirstIndex >= 0)
        {
            if (ShipSelected1.value != FirstIndex)
            {
                ShipSelected1.value = FirstIndex;
            }
            ShowInventoryDetails(
                    panelInventory,
                    FirstIndex
                );
            ShowStatusCrew();
        }
    }

    private void OnSetShipSelected2()
    {
        if (SecondIndex >= 0)
        {
            if (ShipSelected2.value != SecondIndex)
            {
                ShipSelected2.value = SecondIndex;
            }
            ShowInventoryDetails(
                panelInventoryOther,
                SecondIndex
            );
            ShowStatusCrew();
        }
    }

    private void ShowInventoryDetails(GameObject panel, int indexInv)
    {
        MyGameObjectUtils.ClearAllChilds(panel);
        ShipInventory inventory = AvaiableShips[indexInv].inventory;
        if (inventory != null && inventory.goodsCode != null)
        {
            for (int i = 0; i < inventory.goodsCode.Length; i++)
            {
                ScriptableShipGoods aGoods = Goods.Where(x => x.codeName.Equals(inventory.goodsCode[i])).FirstOrDefault();
                if (aGoods != null)
                {
                    GameObject item = Instantiate(prefabItem, panel.transform, false);
                    item.GetComponent<Image>().sprite = aGoods.image;
                    item.GetComponentInChildren<Text>().text = inventory.quantity[i].ToString();
                    item.SetActive(true);
                    int indexPass = i;

                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ShowTransferItem(indexInv, indexPass);
                    });
                }
                else
                {
                    Debug.Log("Item " + inventory.goodsCode[i]);
                }
            }
        }
        DisplayCapacity();
    }
    private void DisplayMarket()
    {

        MyGameObjectUtils.ClearAllChilds(panelInventoryMarket);


        for (int i = 0; i < marketStateToday.goodsCodes.Length; i++)
        {
            ScriptableShipGoods aGoods = Goods.Where(x => x.codeName.Equals(marketStateToday.goodsCodes[i])).FirstOrDefault();
            if (aGoods != null)
            {
                GameObject item = Instantiate(prefabItem, panelInventoryMarket.transform, false);
                item.GetComponent<Image>().sprite = aGoods.image;
                item.GetComponentInChildren<Text>().text = marketStateToday.quantitys[i].ToString();
                item.SetActive(true);
                int indexPass = i;

                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ShowShopItem(1, indexPass);
                });
            }
            else
            {
                Debug.Log("Item " + marketStateToday.goodsCodes[i]);
            }
        }
    }

    private int indexTransfer;
    private void ShowTransferItem(int indexInv, int index)
    {
        if (InventoryMode.Shop.Equals(mode))
        {
            ShowShopItem(0, index);
            return;
        }
        indexTransfer = index;
        int indexFrom = indexInv;
        int indexTo = indexInv == FirstIndex ? SecondIndex : FirstIndex;
        ShipInventory inventory = AvaiableShips[indexInv].inventory;
        ScriptableShipGoods goods = Goods.Where(x => x.codeName == inventory.goodsCode[index]).FirstOrDefault();
        int maxQty = inventory.quantity[index]; //quantity in ship from

        //ship to limit by weight
        int qtyWeight = goods.weight <= 0 ? maxQty : Mathf.FloorToInt(
                ShipHelper.CalculateAvaiableCapacity(AvaiableShips[indexTo])
                / goods.weight);
        Debug.Log(string.Format("Shop Buy Limit weight/qty : {0}/{1}", qtyWeight, maxQty));
        maxQty = Mathf.Min(maxQty, qtyWeight);

        if (transferPanel != null)
        {
            transferPanel.ShowTransferItem(
                goods,
                maxQty,
                indexInv,
                indexTo,
                AvaiableShips[indexFrom],
                AvaiableShips[indexTo]
            );
        }
    }

    private void ShowShopItem(int indexInv, int index)
    {
        indexTransfer = index;
        string[] goodsCodes = indexInv == 0 ? AvaiableShips[FirstIndex].inventory.goodsCode : marketStateToday.goodsCodes;
        int[] quantitys = indexInv == 0 ? AvaiableShips[FirstIndex].inventory.quantity : marketStateToday.quantitys;
        int maxQty = quantitys[index];
        ScriptableShipGoods goods = Goods.Where(x => x.codeName == goodsCodes[index]).FirstOrDefault();
        Debug.Assert(goods != null, "Goods Should found");
        if (indexInv == 1)
        {
            //buy
            //limit by weight ship
            int qtyWeight = goods.weight <= 0 ? maxQty : Mathf.FloorToInt(
                ShipHelper.CalculateAvaiableCapacity(AvaiableShips[FirstIndex])
                / goods.weight);
            //limit by user gold
            int qtyGold = marketStateToday.prices[index] <= 0 ? maxQty : Mathf.FloorToInt(GameManager.Instance.GameData.gold / marketStateToday.prices[index]);
            Debug.Log(string.Format("Shop Buy Limit weight/gold/qty : {0}/{1}/{2}", qtyWeight, qtyGold, maxQty));
            maxQty = Mathf.Min(maxQty, Mathf.Min(qtyWeight, qtyGold));

        }
        else
        {
            //sell
        }

        if (transferItemMarketCtrl != null)
        {
            transferItemMarketCtrl.ShowTransferItem(
                goods,
                maxQty,
                indexInv,
                AvaiableShips[FirstIndex],
                marketStateToday
            );
        }
    }

    public void HideInventory()
    {
        // panelInventoryOther.SetActive(false);
        InventoryCanvas.SetActive(false);
        OnHideInventory?.Invoke();
        GameManager.Instance.ResumeGamePlay();
    }

    public void PressMoveCrew(int source)
    {
        int indexFrom = source == 0 ? SecondIndex : FirstIndex;
        int indexTo = source == 0 ? FirstIndex : SecondIndex;
        ScriptableShipCustom fromShip = AvaiableShips[indexFrom];
        ScriptableShipCustom toShip = AvaiableShips[indexTo];
        int quantityCanTake = toShip.PeakData.maxCrew - toShip.curShipData.maxCrew;
        if (quantityCanTake <= 0)
        {
            return;
        }

        if (transferCrewCtrl != null)
        {
            if (InventoryMode.Transfer.Equals(Mode))
            {
                transferCrewCtrl.ShowTransferCrew(
                    quantityCanTake > fromShip.curShipData.maxCrew ? fromShip.curShipData.maxCrew : quantityCanTake,
                    indexFrom,
                    indexTo,
                    fromShip,
                    toShip
                );
            }
            else
            {
                if (marketStateToday.crewAvaiable <= 0) return;
                int qtyCanBuy = marketStateToday.crewPrice <= 0 ? marketStateToday.crewAvaiable : (GameManager.Instance.GameData.gold / marketStateToday.crewPrice);
                int qty = Mathf.Min(quantityCanTake, Mathf.Min(qtyCanBuy, marketStateToday.crewAvaiable));
                transferCrewCtrl.ShowHireCrew(
                    qty,
                    FirstIndex,
                    AvaiableShips[FirstIndex],
                    marketStateToday
                );
            }
        }
    }
}