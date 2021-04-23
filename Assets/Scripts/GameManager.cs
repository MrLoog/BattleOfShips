using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : BaseSceneManager
{
    public static GameManager Instance;

    public GameObject ModalPopupUtil;

    public ToastService ToastService;

    public ModalPopupCtrl PopupCtrl => ModalPopupUtil.GetComponent<ModalPopupCtrl>();
    public GameObject ShipInventoryCanvas;

    public ShipInventoryCtrl ShipInventoryCtrl => ShipInventoryCanvas.GetComponentInChildren<ShipInventoryCtrl>();

    public UnityAction<long> OnGoldAccountChanged;
    public UnityAction<long> OnGemAccountChanged;
    public IStoreData database;

    public string MessageIntent;

    private ScriptableGameLevel playLevel;

    public ScriptableGameLevel PlayLevel
    {
        get
        {
            if (playLevel == null && gameData != null && gameData.playLevelName != null && gameData.playLevelName.Length > 0)
            {
                playLevel = MyResourceUtils.ResourcesLoadAll<ScriptableGameLevel>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS).First(x => x.name == gameData.playLevelName);
            }
            return playLevel;
        }
        set
        {
            playLevel = value;
            gameData.playLevelName = playLevel.name;
        }
    }
    public GameData gameData;
    public GameData GameData
    {
        set
        {
            gameData = value;
        }
        get
        {
            if (gameData == null)
            {
                gameData = new GameData();
                Debug.Log(JsonConvert.SerializeObject(gameData));
            }
            return (GameData)gameData;
        }
    }

    public int startGold = 10000;
    public int startGem = 10;
    public ScriptableShipCustom playerStarterShip;
    public ScriptableShipCustom playerStarterShipTest;

    public ScriptableShipCustom[] otherShips;

    public ScriptableShipFactory StartShopFactory;
    public ScriptableShipFactory FinalShopFactory;

    public ScriptableBattleFlow firstBattleFlow;
    public ScriptableBattleFlow focusBattleFlow;

    public string mainSceneName = "GameScene";
    public string battleSceneName = "SeaBattleScene";
    public string townSceneName = "TownScene";

    public float hourRefreshMarket = 1f;

    #region toggle for test

    public bool forceRefreshMarket = false;
    #endregion
    public override void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ShipInventoryCanvas);
        DontDestroyOnLoad(PopupCtrl.gameObject); //modal util
        DontDestroyOnLoad(ToastService.gameObject); //toast util

        database = StoreDataFactory.GetDatabase();
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        // gameData.playerShip.test = new Test() { prop = "Test" };
        // GEventManager.Instance.InvokeEvent(GEventManager.EVENT_CLEAR_LEVEL);
        // if (!CommonUtils.IsArrayNullEmpty(otherShips))
        // {
        //     otherShips.ToList().ForEach(
        //         x =>
        //         {
        //             GameData.otherShips = CommonUtils.AddElemToArray(GameData.otherShips, x.Clone<ScriptableShipCustom>());
        //         }
        //     );
        // }

    }


    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle()
    {
        ChangeScene(battleSceneName, SeaBattleManager.INTENT_RESUME);
    }

    public void ResumeGame()
    {
        ChangeScene(GameData.SceneCurrentName, SeaBattleManager.INTENT_RESUME);
    }

    public void StartNewGame()
    {
        GameData = new GameData();
#if UNITY_EDITOR
        // for test
        GameData.playerShip = playerStarterShipTest;
        // GameData.otherShips = lst.ToArray();
#else
        GameData.playerShip = playerStarterShip;
#endif
        GameData.shipShopFactory = StartShopFactory.Clone<ScriptableShipFactory>();
        // List<ScriptableShipCustom> lst = new List<ScriptableShipCustom>();
        // if (otherShips != null)
        // {
        //     foreach (var item in otherShips)
        //     {
        //         lst.Add(item.Clone<ScriptableShipCustom>());
        //     }
        // }

        GameData.gold = startGold;
        GameData.gem = startGem;
        GameData.process = GameData.PROCESS_INIT_FIRST_SHIP;
        focusBattleFlow = firstBattleFlow.Clone<ScriptableBattleFlow>();
        ChangeScene(battleSceneName, SeaBattleManager.INTENT_FIRST_BATTLE);
    }

    public bool ChangeScene(string sceneName, string intentMessage = "")
    {
        Debug.Log("Change Scene " + sceneName);
        MessageIntent = intentMessage;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        if (sceneName != mainSceneName)
        {
            GameData.ScenePrevName = GameData.SceneCurrentName;
            GameData.SceneCurrentName = sceneName;
        }
        return true;
    }

    #region store data
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        if (GameData.SceneCurrentName != null && GameData.SceneCurrentName != GetType().Name)
        {
            SaveDataScene(GameData.SceneCurrentName.Replace("Scene", "Manager"));
        }
        SaveData();
    }

    public void SaveData()
    {
        if (database == null) return;
        database.SaveData(this.name, GameData);
    }

    public void SaveDataScene(string sceneManagerName)
    {
        if (database == null) return;
        GameObject gameObjectManager = GameObject.Find(sceneManagerName);
        if (gameObjectManager != null)
        {
            BaseSceneManager sceneMnager = gameObjectManager.GetComponent<BaseSceneManager>();
            BaseDataEntity dataSave = sceneMnager.GetDataForSave();
            if (dataSave != null)
            {
                database.SaveData(sceneMnager.name, dataSave);
            }
        }
    }

    public void LoadData()
    {
        if (database == null) return;
        GameData = database.LoadData<GameData>(this.name);
    }

    public T LoadDataScene<T>(string sceneManagerName)
    {
        Debug.Log("Load Scene " + sceneManagerName);
        if (database == null) return default(T);
        T data = database.LoadData<T>(sceneManagerName);
        // Debug.Log("Load Scene " + data.GetType() + " " + JsonConvert.SerializeObject(data));
        return data;
    }

    #endregion store data

    #region gold account
    public bool IsEnoughGold(long amount)
    {
        return GameData.gold >= amount;
    }

    public long DeductGold(long amount)
    {
        if (!IsEnoughGold(amount)) return -1;
        GameData.gold -= amount;
        OnGoldAccountChanged?.Invoke(-amount);
        return GameData.gold;
    }

    public long AddGold(long amount)
    {
        GameData.gold += amount;
        OnGoldAccountChanged?.Invoke(amount);
        return GameData.gold;
    }

    public bool IsEnoughGem(long amount)
    {
        return GameData.gem >= amount;
    }

    public long DeductGem(long amount)
    {
        if (!IsEnoughGem(amount)) return -1;
        GameData.gem -= amount;
        OnGemAccountChanged?.Invoke(-amount);
        return GameData.gem;
    }

    public long AddGem(long amount)
    {
        GameData.gem += amount;
        OnGemAccountChanged?.Invoke(amount);
        return GameData.gem;
    }

    internal void DeleteShip(ScriptableShipCustom data)
    {
        if (GameData.otherShips == null) return;
        List<ScriptableShipCustom> datas = GameData.otherShips.ToList();
        datas.Remove(data);
        GameData.otherShips = datas.ToArray();
    }
    #endregion


    public void PressShowInventory()
    {
        if (GameData.otherShips.Length > 0)
        {
            List<ScriptableShipCustom> shipCustoms = new List<ScriptableShipCustom>();
            shipCustoms.Add(GameData.playerShip);
            shipCustoms.AddRange(GameData.otherShips);
            ShipInventoryCtrl.RegisterAvaiableShip(shipCustoms.ToArray(), 0);
            ShipInventoryCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Transfer, 0, 1);
        }
    }

    public void PressShowMarket()
    {
        if (GameData.otherShips.Length > 0)
        {
            List<ScriptableShipCustom> shipCustoms = new List<ScriptableShipCustom>();
            shipCustoms.Add(GameData.playerShip);
            shipCustoms.AddRange(GameData.otherShips);
            ShipInventoryCtrl.RegisterAvaiableShip(shipCustoms.ToArray(), 0);
            ShipInventoryCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Shop, 0);
        }
    }

    public float prevSpeed = 1f;

    public void PauseGamePlay()
    {
        if (Time.timeScale > 0f)
        {
            prevSpeed = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGamePlay()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = prevSpeed > 0f ? prevSpeed : 1;
        }
    }
    public MarketStateToday GetMarketStateToday(bool refresh = false)
    {

        if (GameData.market == null
        || GameData.market.time == null
        || DateTime.Now.Subtract(GameData.market.time).TotalHours > hourRefreshMarket
        // || GameData.market.time.ToString("yyyy-MM-dd hh") != DateTime.Now.ToString("yyyy-MM-dd hh")
        || forceRefreshMarket
        || refresh
        )
        {
            GameData.market = MarketStateFactory.BuildMarketStateToday(MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(
                MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS),
                MarketStateFactory.RandomStateByProbability(MyResourceUtils.ResourcesLoadAll<ScriptableMarketState>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS))
                );
            forceRefreshMarket = false;
        }
        return GameData.market;
    }

    public void ShowToastText(string message, float time)
    {
        ToastService.ShowMessage(message, time);
    }
}
