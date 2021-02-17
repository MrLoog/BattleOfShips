using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameManager : BaseSceneManager
{
    public static GameManager Instance;

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

        database = StoreDataFactory.GetDatabase();
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
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
        ChangeScene(gameData.SceneCurrentName);
    }

    public void StartNewGame()
    {
        GameData.playerShip = playerStarterShip;
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
}
