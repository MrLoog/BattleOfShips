using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ShipInventoryMenu : MonoBehaviour
{
    public static ShipInventoryMenu Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public GameObject panelInventory;
    public GameObject panelInventoryOther;
    public GameObject prefabItem;

    public GameObject panelTransferItem;
    private List<Ship> ships = new List<Ship>();

    private List<ShipInventory> inventories = new List<ShipInventory>();
    public bool isShow = false;

    private List<ScriptableShipGoods> goods;

    List<ScriptableShipGoods> Goods
    {
        get
        {
            if (goods == null)
            {
                goods = SeaBattleManager.Instance.goods.ToList();
            }
            return goods;
        }
    }
    TransferItemCtrl transferPanel;



    // Start is called before the first frame update
    void Start()
    {

        transferPanel = panelTransferItem.GetComponent<TransferItemCtrl>();
        transferPanel.OnDoneTransfer += TransferItemSelected;
    }


    private int AddInventoryIndex(ShipInventory inventory, int wantIndex = -1)
    {
        if (wantIndex >= 0)
        {
            if (inventories.Count <= wantIndex)
            {
                for (int i = inventories.Count; i <= wantIndex; i++)
                {
                    inventories.Add(null);
                }
            }
            inventories[wantIndex] = inventory;
            return wantIndex;
        }
        else
        {
            inventories.Add(inventory);
            return inventories.Count - 1;
        }
    }

    private int AddShipIndex(Ship ship, int wantIndex = -1)
    {
        if (wantIndex >= 0)
        {
            if (ships.Count <= wantIndex)
            {
                for (int i = ships.Count; i <= wantIndex; i++)
                {
                    ships.Add(null);
                }
            }
            ships[wantIndex] = ship;
            return wantIndex;
        }
        else
        {
            ships.Add(ship);
            return ships.Count - 1;
        }
    }

    private void TransferItemSelected(int[] result)
    {
        int quantity = result[0];
        int fromInvIndex = result[1];
        ShipInventory inventoryDeduct = inventories[fromInvIndex == 0 ? 0 : 1];
        ShipInventory inventoryAdd = inventories[fromInvIndex == 0 ? 1 : 0];
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
        if (i == inventoryAdd.goodsCode.Length)
        {
            inventoryAdd.goodsCode = inventoryAdd.goodsCode.Concat(new string[] { inventoryDeduct.goodsCode[indexTransfer] }).ToArray();
            inventoryAdd.quantity = inventoryAdd.quantity.Concat(new int[] { quantity }).ToArray();
        }

        ShowInventoryDetails(
                panelInventory,
                0
            );
        ShowInventoryDetails(
                panelInventoryOther,
                1
            );
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

    public void ShowInventory(Ship otherShip = null)
    {
        gameObject.SetActive(true);
        if (SeaBattleManager.Instance.playerShip != null)
        {
            AddShipIndex(SeaBattleManager.Instance.playerShip.GetComponent<Ship>(), 0);
            ShowInventoryDetails(
                panelInventory,
                AddInventoryIndex(ships[0].Inventory, 0)
            );
            /*
            for (int i = slots.Count; i > 0; i--)
            {
                Destroy(slots[i - 1]);
            }
            slots.Clear();
            ShipInventory inventory = GameManager.instance.playerShip.GetComponent<Ship>().inventory;
            for (int i = 0; i < inventory.goodsCode.Length; i++)
            {
                ScriptableShipGoods aGoods = GameManager.instance.goods.ToList().Where(x => x.codeName.Equals(inventory.goodsCode[i])).FirstOrDefault();
                if (aGoods != null)
                {
                    GameObject item = Instantiate(prefabItem, panelInventory.transform, false);
                    item.GetComponent<Image>().sprite = aGoods.image;
                    item.GetComponentInChildren<Text>().text = inventory.quantity[i].ToString();
                    Debug.Log("Item found " + inventory.goodsCode[i]);
                    item.SetActive(true);
                    slots.Add(item);
                }
                else
                {
                    Debug.Log("Item " + inventory.goodsCode[i]);
                }
            }
            */
        }

        if (otherShip != null)
        {
            panelInventoryOther.SetActive(true);
            AddShipIndex(otherShip, 1);
            ShowInventoryDetails(
                panelInventoryOther,
                AddInventoryIndex(otherShip.Inventory, 1)
            );
            /*
            for (int i = slotsOther.Count; i > 0; i--)
            {
                Destroy(slotsOther[i - 1]);
            }
            slotsOther.Clear();
            ShipInventory inventory = otherShip.inventory;
            for (int i = 0; i < inventory.goodsCode.Length; i++)
            {
                ScriptableShipGoods aGoods = GameManager.instance.goods.ToList().Where(x => x.codeName.Equals(inventory.goodsCode[i])).FirstOrDefault();
                if (aGoods != null)
                {
                    GameObject item = Instantiate(prefabItem, panelInventoryOther.transform, false);
                    item.GetComponent<Image>().sprite = aGoods.image;
                    item.GetComponentInChildren<Text>().text = inventory.quantity[i].ToString();
                    Debug.Log("Item found " + inventory.goodsCode[i]);
                    item.SetActive(true);
                    slotsOther.Add(item);
                }
                else
                {
                    Debug.Log("Item " + inventory.goodsCode[i]);
                }
            }
            */
        }
    }

    private void ClearAllChilds(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }

    private void ShowInventoryDetails(GameObject panel, int indexInv)
    {
        ClearAllChilds(panel);
        ShipInventory inventory = inventories[indexInv];
        Debug.Log("Inventory should show " + inventory.goodsCode.Length);
        for (int i = 0; i < inventory.goodsCode.Length; i++)
        {
            ScriptableShipGoods aGoods = Goods.Where(x => x.codeName.Equals(inventory.goodsCode[i])).FirstOrDefault();
            if (aGoods != null)
            {
                GameObject item = Instantiate(prefabItem, panel.transform, false);
                item.GetComponent<Image>().sprite = aGoods.image;
                item.GetComponentInChildren<Text>().text = inventory.quantity[i].ToString();
                Debug.Log("Item found " + inventory.goodsCode[i]);
                item.SetActive(true);
                int indexPass = i;
                Debug.Log("Register index inv " + indexInv);

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

    private int indexTransfer;
    private void ShowTransferItem(int indexInv, int index)
    {
        indexTransfer = index;
        ShipInventory inventory = inventories[indexInv];
        if (transferPanel != null)
        {
            transferPanel.ShowTransferItem(
                Goods.Where(x => x.codeName == inventory.goodsCode[index]).FirstOrDefault(),
                inventory.quantity[index],
                indexInv,
                ships[indexInv],
                ships[indexInv == 0 ? 1 : 0]
            );
        }
    }

    public void HideInventory()
    {
        panelInventoryOther.SetActive(false);
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGamePlay();
    }
}
