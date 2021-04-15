using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TownManager : BaseSceneManager
{
    public static TownManager Instance;
    public GameManager gameManager;

    public GameObject panelLevelSelect;

    public const string INTENT_LOSE_GAME_RETURN = "LOSE_GAME_NEW_SHIP";

    public Text GoldText;
    public Text GemText;

    public TownData townData;
    public TownData TownData
    {
        set
        {
            townData = value;
        }
        get
        {
            if (townData == null) townData = new TownData();
            return (TownData)townData;
        }
    }

    public override void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        // DontDestroyOnLoad(gameObject);
        gameManager = GameManager.Instance;
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadGame();
        RegisterGoldGemInfo();
    }

    private void RegisterGoldGemInfo()
    {
        GameManager.Instance.OnGoldAccountChanged += DisplayGoldInfo;
        GameManager.Instance.OnGemAccountChanged += DisplayGemInfo;
        DisplayGoldInfo(0);
        DisplayGemInfo(0);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGoldAccountChanged -= DisplayGoldInfo;
        GameManager.Instance.OnGemAccountChanged -= DisplayGemInfo;
    }

    private void DisplayGoldInfo(long amout)
    {
        GoldText.text = GetDisplayGoldGem(GameManager.Instance.GameData.gold);
    }

    private void DisplayGemInfo(long amout)
    {
        GemText.text = GetDisplayGoldGem(GameManager.Instance.GameData.gem);

    }

    private string GetDisplayGoldGem(long amount)
    {
        string surfix = "";
        string displayAmount = amount.ToString();
        int length = displayAmount.Length;
        if (length > 12)
        {
            displayAmount = displayAmount.Substring(0, displayAmount.Length - 9);
            surfix = "b";
        }
        else if (length > 9)
        {
            displayAmount = displayAmount.Substring(0, displayAmount.Length - 6);
            surfix = "m";
        }
        else if (length > 6)
        {
            displayAmount = displayAmount.Substring(0, displayAmount.Length - 3);
            surfix = "k";
        }
        return string.Format(CultureInfo.CurrentCulture, "{0:N0}" + surfix, Int64.Parse(displayAmount));
    }

    // Update is called once per frame
    void Update()
    {

    }


    public Workshop GetWorkshopData()
    {
        if (TownData.workshop == null || TownData.workshop.timeRefresh == null || TownData.workshop.forceReload || TownData.workshop.timeRefresh.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
        {
            RefreshWorkshop();
        }
        return TownData.workshop;
    }

    public Workshop RefreshWorkshop()
    {
        ScriptableShipFactory factory = GameManager.Instance.gameData.shipShopFactory;
        Debug.Assert(factory != null, "factory should found");
        Workshop result = TownData.workshop ?? (new Workshop());
        result.timeRefresh = DateTime.Now;
        int quantity = result.slot;
        result.workshopShips = factory.GetRandomShip(quantity, true);
        result.soldStatus = Enumerable.Repeat(false, quantity).ToArray();
        result.forceReload = false;
        TownData.workshop = result;
        return result;
    }

    public override BaseDataEntity GetDataForSave()
    {
        CollectSceneData();
        UpdateGameData();
        return base.GetDataForSave();
    }

    public void CollectSceneData()
    {
    }

    public void UpdateGameData()
    {

    }
    public void SaveGame()
    {
        //auto collect & update game data then save
        gameManager.SaveGame();
    }

    public void LoadGame()
    {
        if (townData != null)
        {

        }
    }

    #region sea battle prepare
    public void ToggleSelectLevel()
    {
        if (panelLevelSelect.activeSelf)
        {
            CloseLevelSelect();
        }
        else
        {
            SetSail();
        }
    }
    public void SetSail()
    {
        SaveGame();
        panelLevelSelect?.SetActive(true);
        // GameManager.Instance.ChangeScene(GameManager.Instance.battleSceneName);
    }

    public void PlayLevel(ScriptableGameLevel level)
    {
        GameManager.Instance.PlayLevel = level;
        GameManager.Instance.focusBattleFlow = level.battleFlow.Clone<ScriptableBattleFlow>();
        GameManager.Instance.ChangeScene(GameManager.Instance.battleSceneName, SeaBattleManager.INTENT_BATTLE_LEVEL);
    }

    public void CloseLevelSelect()
    {
        panelLevelSelect?.GetComponent<GameLevelDisplay>()?.CloseSelectPanel();
        // panelLevelSelect?.SetActive(false);
    }
    #endregion

    #region main menu
    public void ReturnMainScene()
    {
        SaveGame();
        GameManager.Instance.ChangeScene(GameManager.Instance.mainSceneName);
    }
    #endregion
}
