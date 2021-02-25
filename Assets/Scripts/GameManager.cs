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

    public ModalPopupCtrl PopupCtrl => ModalPopupUtil.GetComponent<ModalPopupCtrl>();
    public GameObject ShipInventoryCanvas;

    public ShipInventoryCtrl ShipInventoryCtrl => ShipInventoryCanvas.GetComponentInChildren<ShipInventoryCtrl>();

    public UnityAction<int> OnGoldAccountChanged;
    public IStoreData database;

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

    public ScriptableShipCustom playerStarterShip;
    public ScriptableShipCustom[] otherShips;

    public string mainSceneName = "GameScene";
    public string battleSceneName = "SeaBattleScene";
    public string townSceneName = "TownScene";


    public override void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ShipInventoryCanvas);
        DontDestroyOnLoad(PopupCtrl.gameObject); //modal util

        database = StoreDataFactory.GetDatabase();
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        // gameData.playerShip.test = new Test() { prop = "Test" };
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void StartBattle()
    {
        ChangeScene(battleSceneName);
    }

    public void ResumeGame()
    {
        ChangeScene(GameData.SceneCurrentName);
    }

    public void StartNewGame()
    {
        GameData.playerShip = playerStarterShip;
        List<ScriptableShipCustom> lst = new List<ScriptableShipCustom>();
        if (otherShips != null)
        {
            foreach (var item in otherShips)
            {
                lst.Add(item.Clone<ScriptableShipCustom>());
            }
        }
        GameData.otherShips = lst.ToArray();
        GameData.gold = 5000;
        GameData.process = GameData.PROCESS_INIT_FIRST_SHIP;
        ChangeScene(battleSceneName);
    }

    public bool ChangeScene(string sceneName)
    {
        Debug.Log("Change Scene " + sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        if (sceneName != mainSceneName)
        {
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
    public bool IsEnoughGold(int amount)
    {
        return GameData.gold >= amount;
    }

    public int DeductGold(int amount)
    {
        if (!IsEnoughGold(amount)) return -1;
        GameData.gold -= amount;
        OnGoldAccountChanged?.Invoke(-amount);
        return GameData.gold;
    }

    public int AddGold(int amount)
    {
        GameData.gold += amount;
        OnGoldAccountChanged?.Invoke(amount);
        return GameData.gold;
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
        prevSpeed = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void ResumeGamePlay()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = prevSpeed > 0f ? prevSpeed : 1;
        }
    }

    public MarketStateToday GetMarketStateToday()
    {
        if (GameData.market == null || GameData.market.time == null || GameData.market.time.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
        {
            GameData.market = MarketStateFactory.BuildMarketStateToday(MyResourceUtils.ResourcesLoadAll<ScriptableShipGoods>(
                MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS_GOODS),
                MarketStateFactory.RandomStateByProbability(MyResourceUtils.ResourcesLoadAll<ScriptableMarketState>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_OBJECTS))
                );
        }
        return GameData.market;
    }
}
