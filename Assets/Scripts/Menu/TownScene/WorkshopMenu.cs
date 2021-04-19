using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WorkshopMenu : MonoBehaviour
{
    public static WorkshopMenu Instance { get; private set; }
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

    public GameObject buttonRefresh;
    public Text PlayerGold;
    public Text CountdownRefresh;


    private int costRefresh = 3;
    private const string TEMPLATE_REFRESH_BTN = "Refresh({0:N0} gem)";
    private string template_refresh_btn = "";
    private const string TEMPLATE_PLAYER_GOLD = "<color=yellow>Gold: {0:N0}</color>";

    private Workshop workshop;

    private ScriptableShipCustom[] shipCustoms;

    public ScriptableShipCustom[] ShipCustomList
    {
        set
        {
            shipCustoms = value;
        }
        get
        {
            return shipCustoms;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // buttonRefresh.GetComponentInChildren<Text>().text = string.Format(TEMPLATE_REFRESH_BTN, costRefresh);
        if (template_refresh_btn == "")
        {
            template_refresh_btn = buttonRefresh.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        buttonRefresh.GetComponentInChildren<TextMeshProUGUI>().text =
        template_refresh_btn.Replace("{0}",
        string.Format("{0:N0}", costRefresh)
        );

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

    // Update is called once per frame
    void Update()
    {
        if (TownManager.Instance.TownData?.workshop?.timeRefresh != null)
        {
            TimeSpan timeLeft = DateTime.Now.Subtract(TownManager.Instance.TownData.workshop.timeRefresh);
            if (timeLeft.TotalHours < TownManager.Instance.hourRefreshShop)
            {
                CountdownRefresh.text = TimeSpan.FromHours(TownManager.Instance.hourRefreshShop - timeLeft.TotalHours).ToString(@"hh\:mm\:ss");
            }
            else
            {
                DoRefreshWorkshop();
            }
        }
    }

    private void DisplayPlayerGold(long gold = 0)
    {
        PlayerGold.text = string.Format(TEMPLATE_PLAYER_GOLD, GameManager.Instance.GameData.gold);
    }

    public void ToggleWorkshop()
    {
        if (panel.activeSelf)
        {
            HideWorkshop();
        }
        else
        {
            ShowAllShip();
        }
    }
    public void ShowAllShip()
    {

        for (int i = shipListContent.transform.childCount; i > 0; i--)
        {
            Destroy(shipListContent.transform.GetChild(i - 1).gameObject);
        }
        if (workshop == null)
        {
            RequestWorkshopInfo();
        }
        for (int i = 0; i < ShipCustomList.Length; i++)
        {
            ShowShip(ShipCustomList[i]);
        }
        panel.SetActive(true);
    }

    private void ShowShip(ScriptableShipCustom data)
    {
        GameObject newShipInfo = Instantiate(prefabShipInfo, shipListContent.transform, false);
        ShipManageInfo scriptInfo = newShipInfo.GetComponent<ShipManageInfo>();
        scriptInfo.WorkshopMode = true;
        scriptInfo.SetShipData(data, prefabInfoRow);
        if (workshop.IsSold(data))
        {
            scriptInfo.SetFuncText("Sold");
            scriptInfo.btnFunc.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            scriptInfo.SetFuncText(string.Format("{0:N0}", ShipHelper.CalculateShipPrice(scriptInfo.data)));
            scriptInfo.OnFuncBtnClick += delegate ()
            {
                PerfromBuyShip(scriptInfo);
            };
        }
        newShipInfo.SetActive(true);
    }

    public void HideWorkshop()
    {
        ShipInfoDetails.Instance.HidePanel();
        panel.SetActive(false);
    }
    private ShipManageInfo focusShip;

    private void RequestWorkshopInfo()
    {
        workshop = TownManager.Instance.GetWorkshopData();
        ShipCustomList = workshop.workshopShips;
    }
    private void PerfromBuyShip(ShipManageInfo shipManageInfo)
    {
        int price = ShipHelper.CalculateShipPrice(shipManageInfo.data);
        if (GameManager.Instance.IsEnoughGold(price))
        {
            GameManager.Instance.PopupCtrl.ShowDialog(
                title: GameText.GetText(GameText.CONFIRM_BUY_SHIP_TITLE),
                content: string.Format(GameText.GetText(GameText.CONFIRM_BUY_SHIP_CONTENT), price),
                okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                onResult: (choice) =>
                  {
                      if (choice == ModalPopupCtrl.RESULT_POSITIVE)
                      {
                          workshop.MarkShipSold(shipManageInfo.data);
                          shipManageInfo.SetFuncText("Sold");
                          shipManageInfo.btnFunc.GetComponent<UnityEngine.UI.Button>().interactable = false;
                          GameManager.Instance.GameData.AddSubShip(shipManageInfo.data.Clone<ScriptableShipCustom>());
                          GameManager.Instance.DeductGold(price);
                      }
                  }
            );
        }
    }

    public void DoRefreshWorkshop()
    {
        TownManager.Instance.RefreshWorkshop();
        RequestWorkshopInfo();
        ShowAllShip();
    }
    public void RefreshShop()
    {
        if (GameManager.Instance.IsEnoughGem(costRefresh))
        {
            GameManager.Instance.PopupCtrl.ShowDialog(
                title: GameText.GetText(GameText.CONFIRM_SHOP_REFRESH_TITLE),
                content: string.Format(GameText.GetText(GameText.CONFIRM_SHOP_REFRESH_CONTENT), costRefresh),
                okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                onResult: (choice) =>
                  {
                      if (choice == ModalPopupCtrl.RESULT_POSITIVE)
                      {
                          DoRefreshWorkshop();
                          GameManager.Instance.DeductGem(costRefresh);
                      }
                  }
            );
        }
        else
        {
            GameManager.Instance.ToastService.ShowMessage(GameText.GetText(GameText.TOAST_NOT_ENOUGH_GEM), 3f);
        }
    }
}
