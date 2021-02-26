﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Workshop GetWorkshopData()
    {
        if (TownData.workshop == null || TownData.workshop.timeRefresh == null || TownData.workshop.timeRefresh.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
        {
            TownData.workshop = RefreshWorkshop();
        }
        return TownData.workshop;
    }

    public Workshop RefreshWorkshop()
    {
        ScriptableShipFactory factory = MyResourceUtils.ResourcesLoad<ScriptableShipFactory>(MyResourceUtils.RESOURCES_PATH_SCRIPTABLE_WORKSHOP);
        Debug.Assert(factory != null, "factory should found");
        Workshop result = TownData.workshop ?? (new Workshop());
        result.timeRefresh = DateTime.Now;
        int quantity = result.slot;
        result.workshopShips = factory.GetRandomShip(quantity);
        result.soldStatus = Enumerable.Repeat(false, quantity).ToArray();
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
