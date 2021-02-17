using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : BaseSceneManager
{
    public static TownManager Instance;
    public GameManager gameManager;

    public TownData townData;
    public TownData TownData
    {
        set
        {
            townData = value;
        }
        get
        {
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
    }

    // Update is called once per frame
    void Update()
    {

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
    public void SetSail()
    {
        SaveGame();
        GameManager.Instance.ChangeScene(GameManager.Instance.battleSceneName);
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
