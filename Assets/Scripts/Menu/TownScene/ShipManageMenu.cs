using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipManageMenu : MonoBehaviour
{
    public static ShipManageMenu Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }
    public GameObject panel;
    public GameObject scrollView;
    public GameObject shipListContent;
    public GameObject prefabShipInfo;
    public GameObject prefabInfoRow;

    public GameObject miniMenu;

    public ShipManageMiniMenu MiniMenuCtrl => miniMenu.GetComponent<ShipManageMiniMenu>();


    public ScriptableShipCustom[] shipCustoms;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowManage()
    {

        for (int i = shipListContent.transform.childCount; i > 0; i--)
        {
            Destroy(shipListContent.transform.GetChild(i - 1).gameObject);
        }

        shipCustoms = new ScriptableShipCustom[GameManager.Instance.GameData.otherShips.Length + 1];
        ShowShip(GameManager.Instance.GameData.playerShip, true);
        shipCustoms[0] = GameManager.Instance.GameData.playerShip;

        if (GameManager.Instance.GameData.otherShips != null)
        {
            int i = 1;
            foreach (var item in GameManager.Instance.GameData.otherShips)
            {
                ShowShip(item);
                shipCustoms[i++] = item;
            }
        }
        // for (int i = 0; i < GameManager.Instance.gameData.otherShips.Length; i++)
        // {

        //     GameObject newShipInfo = Instantiate(prefabShipInfo, shipListContent.transform, false);
        //     ShipManageInfo scriptInfo = newShipInfo.GetComponent<ShipManageInfo>();
        //     if (i == 0) scriptInfo.isMainShip = true;
        //     scriptInfo.SetShipData(shipCustoms[i].Clone<ScriptableShipCustom>(), prefabInfoRow);
        //     newShipInfo.SetActive(true);
        // }
        panel.SetActive(true);
    }

    private void ShowShip(ScriptableShipCustom data, bool isMain = false)
    {
        GameObject newShipInfo = Instantiate(prefabShipInfo, shipListContent.transform, false);
        ShipManageInfo scriptInfo = newShipInfo.GetComponent<ShipManageInfo>();
        if (isMain) scriptInfo.isMainShip = true;
        scriptInfo.SetShipData(data, prefabInfoRow);
        scriptInfo.OnFuncBtnClick += delegate ()
        {
            ShowMiniMenu(scriptInfo);
        };
        newShipInfo.SetActive(true);
    }

    public void HideManage()
    {
        panel.SetActive(false);
    }
    private ShipManageInfo focusShip;
    public void ShowMiniMenu(ShipManageInfo shipManageInfo)
    {
        MiniMenuCtrl.ShowMiniMenu(shipManageInfo);
        if (shipManageInfo == null || shipManageInfo.Equals(focusShip))
        {
            focusShip = null;
        }
        else
        {
            focusShip = shipManageInfo;
        }
    }


    public void PerformRepair()
    {
        if (focusShip != null)
        {
            int cost = ShipHelper.CalculateRepairCost(focusShip.data);
            if (GameManager.Instance.IsEnoughGold(cost))
            {
                GameManager.Instance.PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.CONFIRM_REPAIR_TITLE),
                    content: GameText.GetText(GameText.CONFIRM_REPAIR_CONTENT, cost),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_YES),
                    cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                    onResult: (i) =>
                {
                    if (i == ModalPopupCtrl.RESULT_POSITIVE)
                    {
                        if (GameManager.Instance.DeductGold(cost) > -1)
                        {
                            ShipHelper.PerformFullRepair(focusShip.data);
                            focusShip.ShowData();
                            MiniMenuCtrl.ValidFeatureAvaiable();
                        }
                    }
                }
                );
            }
            else
            {
                GameManager.Instance.PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.CONFIRM_GOLD_NOT_ENOUGH_TITLE),
                    content: GameText.GetText(GameText.CONFIRM_GOLD_NOT_ENOUGH_CONTENT, cost),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                    cancelText: null,
                    onResult: null
                );
            }
        }
    }

    public void PerformSellShip()
    {
        if (focusShip != null)
        {
            int cargoValue = ShipHelper.CalculateAllCargoValue(focusShip.data);
            int price = ShipHelper.CalculateSellPrice(focusShip.data);
            string extraContent = "";
            if (focusShip.data.curShipData.maxCrew > 0)
            {
                extraContent = GameText.GetText(GameText.CONFIRM_SELL_CONTENT_EXTRA, focusShip.data.curShipData.maxCrew);
            }
            GameManager.Instance.PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.CONFIRM_SELL_TITLE),
                    content: GameText.GetText(GameText.CONFIRM_SELL_CONTENT, price, cargoValue, extraContent),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_YES),
                    cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                    onResult: (i) =>
                {
                    if (i == ModalPopupCtrl.RESULT_POSITIVE)
                    {
                        ShipManageInfo info = focusShip;
                        //hide mini menu
                        ShowMiniMenu(null);
                        //destroy ship
                        GameManager.Instance.DeleteShip(info.data);
                        //remove manage
                        info.SelfDestroy();
                        GameManager.Instance.AddGold(price + cargoValue);
                    }
                }
                );
        }
    }

    public void PerformTransferCargo()
    {
        int indexShip = shipCustoms.ToList().IndexOf(focusShip.data);
        GameManager.Instance.ShipInventoryCtrl.RegisterAvaiableShip(shipCustoms, indexShip);
        GameManager.Instance.ShipInventoryCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Transfer);
    }

    public void PerformShop()
    {
        int indexShip = shipCustoms.ToList().IndexOf(focusShip.data);
        GameManager.Instance.ShipInventoryCtrl.RegisterAvaiableShip(shipCustoms, indexShip);
        GameManager.Instance.ShipInventoryCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Shop, indexShip);
    }
}
