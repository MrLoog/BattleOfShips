using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    public bool isShow = false;

    List<GameObject> slots = new List<GameObject>();
    List<GameObject> slotsOther = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
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
        if (GameManager.instance.playerShip != null)
        {

            ShowInventoryDetails(
                slots,
                panelInventory,
                GameManager.instance.playerShip.GetComponent<Ship>().inventory
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
            ShowInventoryDetails(
                slotsOther,
                panelInventoryOther,
                otherShip.inventory
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

    private void ShowInventoryDetails(List<GameObject> lst, GameObject panel, ShipInventory inventory)
    {
        for (int i = lst.Count; i > 0; i--)
        {
            Destroy(lst[i - 1]);
        }
        slots.Clear();
        for (int i = 0; i < inventory.goodsCode.Length; i++)
        {
            ScriptableShipGoods aGoods = GameManager.instance.goods.ToList().Where(x => x.codeName.Equals(inventory.goodsCode[i])).FirstOrDefault();
            if (aGoods != null)
            {
                GameObject item = Instantiate(prefabItem, panel.transform, false);
                item.GetComponent<Image>().sprite = aGoods.image;
                item.GetComponentInChildren<Text>().text = inventory.quantity[i].ToString();
                Debug.Log("Item found " + inventory.goodsCode[i]);
                item.SetActive(true);
                lst.Add(item);
            }
            else
            {
                Debug.Log("Item " + inventory.goodsCode[i]);
            }
        }
    }

    public void HideInventory()
    {
        panelInventoryOther.SetActive(false);
        gameObject.SetActive(false);
        GameManager.instance.ResumeGame();
    }
}
