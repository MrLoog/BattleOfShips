using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSceneManager
{
    public static GameManager Instance;

    public IStoreData database;

    public GameData gameData;

    public string mainSceneName = "MainScene";
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

    public bool ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        gameData.SceneCurrentName = sceneName;
        return true;
    }

    #region store data
    private void OnApplicationQuit()
    {
        SaveData();
        SaveDataScene(gameData.SceneCurrentName);
    }

    public void SaveData()
    {
        if (database == null) return;
        database.SaveData(mainSceneName, gameData);
    }

    public void SaveDataScene(string sceneManagerName)
    {
        if (database == null) return;
        GameObject gameObjectManager = GameObject.Find(sceneManagerName);
        if (gameObjectManager != null)
        {
            BaseSceneManager sceneMnager = gameObjectManager.GetComponent<BaseSceneManager>();
            database.SaveData(sceneManagerName, sceneMnager.GetDataForSave());
        }
    }

    public void LoadData()
    {
        if (database == null) return;
        gameData = (GameData)database.LoadData(mainSceneName);
    }

    public BaseDataEntity LoadDataScene(string sceneManagerName)
    {
        if (database == null) return null;
        return database.LoadData(sceneManagerName);
    }

    #endregion store data
}
