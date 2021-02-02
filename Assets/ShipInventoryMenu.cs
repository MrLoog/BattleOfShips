using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShipInventoryMenu : MonoBehaviour
{
    public GameObject panelInventory;
    public GameObject prefabItem;
    public bool isShow = false;

    List<GameObject> slots = new List<GameObject>();
    void Awake()
    {
    }
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

    public void ShowInventory()
    {
        gameObject.SetActive(true);
        if (GameManager.instance.playerShip != null)
        {
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
        }
    }

    public void HideInventory()
    {
        gameObject.SetActive(false);
    }
}
