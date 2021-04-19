using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SeaBattleManager : BaseSceneManager
{
    [System.Flags]
    public enum CannonDirection
    {
        None = 0, Front = 1, Right = 2, Left = 4, Back = 8
    }
    public static SeaBattleManager Instance;

    public GameManager gameManager;

    public SeaBattleData seaBattleData;
    public SeaBattleData SeaBattleData
    {
        set
        {
            seaBattleData = value;
        }
        get
        {
            return (SeaBattleData)seaBattleData;
        }
    }

    public PoolManager<WrapPool> poolCannon;
    public PoolManager<WrapPool> poolHumanFly;
    public GameObject prefabCannon;

    public ScriptableCannonBall[] avaiableCannonBall;

    public Dictionary<string, PoolManager<WrapPool>> dictCannons = new Dictionary<string, PoolManager<WrapPool>>();
    public GameObject prefabHumanFly;
    public GameObject prefabShip;
    public GameObject playerShip;
    public GameObject enemyShip;

    public GameObject InventoryMenu;





    public Vector2 windForce;


    public float[] windChangeRandomTime;
    public float intervalWindChange = 30f;


    public float accumWindChangeInterval = 0f;

    public UnityEvent OnWindChange;

    public Text WindTimeRemain;

    List<ScriptableCannonBall> ScriptableCannonBalls;


    public ScriptableShip[] scriptableShips;
    public ScriptableShipSkill[] scriptableShipSkills;

    public ScriptableShipGoods[] goods;
    public ScriptableShip playerStartShip;
    public ScriptableShipCustom playerStartShipCustom;

    private List<Ship> ships;

    public List<Ship> AllShip
    {
        get
        {
            return ships;
        }
    }
    public GameObject ShipManager;

    public Tilemap battleFields;
    public GameObject cameraFollow;

    public WeatherWindRate[] weatherWindRates;

    public Canvas Modal;
    public GameObject DialogInputSpawnShip;

    public ScriptableStateShip[] ImgShipGroups;


    public ModalPopupCtrl PopupCtrl => gameManager.PopupCtrl;

    public bool IsBattle
    {
        get
        {
            return seaBattleData.IsBattle;
        }
        set
        {
            seaBattleData.IsBattle = value;
        }
    }

    public float rateCrewConduct = 0.5f;
    public float rangeCloseConduct = 1f;

    public const string INTENT_RESUME = "RESUME";
    public const string INTENT_BATTLE_LEVEL = "BATTLE_LEVEL";
    public const string INTENT_FIRST_BATTLE = "FIRST_BATTLE";
    public const string PLAYER_SHIP_BATTLE_ID = "0";

    public override void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        // DontDestroyOnLoad(gameObject);
        gameManager = GameManager.Instance;

        Resources.LoadAll<ScriptableCannonBall>("ScriptableObjects");
        scriptableShips = Resources.LoadAll<ScriptableShip>("ScriptableObjects");
        goods = Resources.LoadAll<ScriptableShipGoods>("ScriptableObjects");
        scriptableShipSkills = Resources.LoadAll<ScriptableShipSkill>("ScriptableObjects");
        ScriptableCannonBalls = Resources.FindObjectsOfTypeAll<ScriptableCannonBall>().Cast<ScriptableCannonBall>().ToList();


        ships = new List<Ship>();
        // for (int i = 0; i < ShipManager.transform.childCount; i++)
        // {
        //     ships.Add(ShipManager.transform.GetChild(i).GetComponent<Ship>());
        // }
        base.Awake();
        // ships.Add(playerShip.GetComponent<Ship>());
        // ships.Add(enemyShip.GetComponent<Ship>());
        // ships.Add(enemyShip.GetComponent<Ship>());
    }
    // Start is called before the first frame update
    void Start()
    {
        StartBattle();

        for (int i = 0; i < avaiableCannonBall.Length; i++)
        {
            ScriptableCannonBall cannonBallData = avaiableCannonBall[i].Clone<ScriptableCannonBall>();

            GameObject newCannon = Instantiate(cannonBallData.prefab);
            newCannon.SetActive(false);

            newCannon.GetComponent<BaseShot>().Data = cannonBallData;
            // CannonPool cannon = prefabCannon.Find
            WrapPool poolItemSeed = ScriptableObject.CreateInstance<WrapPool>();
            poolItemSeed.poolObj = newCannon;
            PoolManager<WrapPool> pool = new PoolManager<WrapPool>(poolItemSeed);
            pool.OnCreateNew = delegate ()
            {
                // WrapPool newInstance = poolCannon.pooledObjects.Last();
                WrapPool newInstance = pool.newInstance;
                BaseShot cs = newInstance.poolObj.GetComponent<BaseShot>();
                newInstance.poolObj = Instantiate(cannonBallData.prefab);
                // newInstance.poolObj.GetComponent<CannonShot>().Data = cs.Data.Clone<ScriptableCannonBall>();
                newInstance.poolObj.GetComponent<BaseShot>().Data = cs.Data;
                // newInstance.SetActive(false);
            };
            dictCannons.Add(cannonBallData.codeName, pool);
        }

        /*
                GameObject newCannon = Instantiate(prefabCannon);
                newCannon.SetActive(false);
                newCannon.GetComponent<CannonShot>().Data = ScriptableCannonBalls.Where(x => x.name == "RoundShot").First();
                // CannonPool cannon = prefabCannon.Find
                WrapPool cannon = ScriptableObject.CreateInstance<WrapPool>();
                cannon.poolObj = newCannon;
                poolCannon = new PoolManager<WrapPool>(cannon);
                poolCannon.OnCreateNew = delegate ()
                {
                    // WrapPool newInstance = poolCannon.pooledObjects.Last();
                    WrapPool newInstance = poolCannon.newInstance;
                    CannonShot cs = newInstance.poolObj.GetComponent<CannonShot>();
                    newInstance.poolObj = Instantiate(prefabCannon);
                    // newInstance.poolObj.GetComponent<CannonShot>().Data = cs.Data.Clone<ScriptableCannonBall>();
                    newInstance.poolObj.GetComponent<CannonShot>().Data = cs.Data;
                    // newInstance.SetActive(false);
                };
                */

        GameObject seedHUmanFly = Instantiate(prefabHumanFly);
        seedHUmanFly.SetActive(false);
        // CannonPool cannon = prefabCannon.Find
        WrapPool wrapHuman = ScriptableObject.CreateInstance<WrapPool>();
        wrapHuman.poolObj = seedHUmanFly;
        poolHumanFly = new PoolManager<WrapPool>(wrapHuman);
        poolHumanFly.OnCreateNew = delegate ()
        {
            // WrapPool newInstance = poolCannon.pooledObjects.Last();
            WrapPool newInstance = poolHumanFly.newInstance;
            newInstance.poolObj = Instantiate(prefabHumanFly);
            // newInstance.SetActive(false);
        };
    }

    internal Ship FindShipByBattleId(string battleId)
    {
        return AllShip.FirstOrDefault(x => x.BattleId == battleId);
    }

    public void StartBattle()
    {
        RandomWindForce();
        if (INTENT_RESUME.Equals(gameManager.MessageIntent))
        {
            LoadGame();
            SeaBattleData.activeFlow.ActiveFlow();
        }
        else
        {
            SeaBattleData = new SeaBattleData();
            SeaBattleData.activeFlow = gameManager.focusBattleFlow;
            if (SeaBattleData.activeFlow.type.Equals(ScriptableBattleFlow.FlowType.GameLevel) && GameManager.Instance.PlayLevel != null)
            {
                SeaBattleData.levelShipDatas = GameManager.Instance.PlayLevel.GetAllShipData();

            }
            else if (SeaBattleData.activeFlow.type.Equals(ScriptableBattleFlow.FlowType.Self))
            {
                SeaBattleData.levelShipDatas = ScriptableGameLevel.GetAllShipDataFromGroup(SeaBattleData.activeFlow.groupShips);
            }
            SeaBattleData.IsLevelShipSpawn = Enumerable.Repeat(false, SeaBattleData.levelShipDatas.Length).ToArray();
            SeaBattleData.IsRewardShipTake = Enumerable.Repeat(false, SeaBattleData.levelShipDatas.Length).ToArray();
            RandomTeleport(SpawnPlayerShip(gameManager.GameData.playerShip.Restore<ScriptableShipCustom>()));

            SeaBattleData.activeFlow.ActiveFlow();
            GEventManager.Instance.InvokeEvent(GEventManager.EVENT_START_BATTLE);
            if (!INTENT_FIRST_BATTLE.Equals(gameManager.MessageIntent))
                gameManager.GameData.process = GameData.PROCESS_FIRST_TIME_BATTLE;
        }
        IsBattle = true;
    }

    public void RestartBattle()
    {
        ClearAllShip();
        StartBattle();
    }

    public void EndBattle(bool win = true)
    {
        IsBattle = false;
        if (win)
        {
            GameManager.Instance.ToastService.ShowMessage(GameText.GetText(GameText.TOAST_DECLARE_WIN), 5f);
        }
        else
        {
            StartCoroutine(InformLoseGame());
        }
    }

    // Update is called once per frame
    void Update()
    {
        accumWindChangeInterval += Time.deltaTime;
        if (accumWindChangeInterval >= intervalWindChange)
        {
            RandomWindForce();
        }
        //display wind reset time
        TimeSpan time = TimeSpan.FromSeconds(intervalWindChange - accumWindChangeInterval);
        WindTimeRemain.text = time.ToString(@"mm\:ss");


    }

    internal void PlayerFireCannon(CannonDirection direction)
    {
        playerShip.GetComponent<Ship>().FireCannon(direction);
    }

    public Vector2 GetPlayerShipFrontDirection()
    {
        return playerShip.GetComponent<Ship>().ShipDirection;
    }

    public void RandomWindForce()
    {
        // float force = (float)Math.Round(Random.Range(0.1f, 3f), 1);
        float force = RandomWindByConfig();
        Debug.Log("force " + force);
        float direction = Random.Range(-180, 180);

        windForce = VectorUtils.Rotate(Vector2.up, direction, true).normalized * force;
        OnWindChange.Invoke();
        foreach (Ship s in ships)
        {
            s.ApplyWindForce(windForce);
        }

        if (!CommonUtils.IsArrayNullEmpty(windChangeRandomTime) && windChangeRandomTime.Length == 2)
        {
            intervalWindChange = Random.Range(windChangeRandomTime[0], windChangeRandomTime[1]);
        }
        accumWindChangeInterval = 0f;
    }

    public float RandomWindByConfig()
    {
        Debug.Log("random wind");
        if (weatherWindRates == null || weatherWindRates.Length == 0)
        {
            Debug.Log("random wind random");
            return (float)Math.Round(Random.Range(0.1f, 3f), 1);
        }
        else
        {
            // float maxRange = 0f;
            // float[,] level = new float[weatherWindRates.Length, 2];
            // for (int i = 0; i < weatherWindRates.Length; i++)
            // {
            //     level[i, 0] = maxRange;
            //     maxRange += weatherWindRates[i].percent;
            //     level[i, 1] = maxRange - 1;
            // }
            // float selected = Random.Range(0f, maxRange - 1);
            // Debug.Log("random wind selected " + selected);

            // for (int i = 0; i < weatherWindRates.Length; i++)
            // {
            //     Debug.Log("random wind check " + level[i, 0] + " - " + level[i, 1]);
            //     if (level[i, 0] <= selected && level[i, 1] >= selected)
            //     {
            //         Debug.Log("random wind level " + i + " " + weatherWindRates[i].min + "-" + weatherWindRates[i].max);
            //         return (float)Math.Round(Random.Range(weatherWindRates[i].min, weatherWindRates[i].max), 1);
            //     }
            // }
            int choiceWindLevel = CommonUtils.RandomByRate(weatherWindRates.Select(x => x.weightProbability).ToArray());
            return (float)Math.Round(Random.Range(weatherWindRates[choiceWindLevel].min, weatherWindRates[choiceWindLevel].max), 1);
        }
    }

    public void RandomTeleport(GameObject gameObject)
    {
        int randX = Random.Range(battleFields.cellBounds.xMin, battleFields.cellBounds.xMax);
        int randY = Random.Range(battleFields.cellBounds.yMin, battleFields.cellBounds.yMax);
        Vector3 newPos = battleFields.GetCellCenterWorld(new Vector3Int(randX, randY, 0));
        Debug.Log(string.Format("Teleport {0} / {1}-{2}:{3} / {4}-{5}:{6}",
        newPos,
        battleFields.cellBounds.xMin,
        battleFields.cellBounds.xMax,
        randX,
        battleFields.cellBounds.yMin,
        battleFields.cellBounds.yMax,
        randY));
        Debug.Log("Teleport 1" + battleFields.GetCellCenterWorld(new Vector3Int(battleFields.cellBounds.xMin, battleFields.cellBounds.yMin, 0)));
        Debug.Log("Teleport 2" + battleFields.GetCellCenterWorld(new Vector3Int(battleFields.cellBounds.xMax - 1, battleFields.cellBounds.yMin, 0)));
        Debug.Log("Teleport 3" + battleFields.GetCellCenterWorld(new Vector3Int(battleFields.cellBounds.xMin, battleFields.cellBounds.yMax - 1, 0)));
        Debug.Log("Teleport 4" + battleFields.GetCellCenterWorld(new Vector3Int(battleFields.cellBounds.xMax - 1, battleFields.cellBounds.yMax - 1, 0)));
        Debug.Log("Teleport 5" + battleFields.GetCellCenterWorld(new Vector3Int(0, 0, 0)));
        gameObject.transform.position = (Vector2)newPos;
    }

    public void RestartGame()
    {
        Modal.gameObject.SetActive(true);
        DialogInputSpawnShip.SetActive(true);
        Time.timeScale = 0f;

        // foreach (Ship s in ships)
        // {
        //     s.curShipData.hullHealth = s.startShipData.hullHealth;
        //     s.RestoreState();
        //     RandomTeleport(s.gameObject);
        //     s.RevalidMovement();
        // }
    }

    private void ClearAllShip()
    {
        foreach (Ship s in ships)
        {
            Destroy(s.gameObject);
        }
        ships.Clear();
    }
    internal void SpawnNewShips(int[] enemys)
    {
        ClearAllShip();
        SpawnPlayerShip(gameManager.GameData.playerShip.Restore<ScriptableShipCustom>());
        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemys[i] > 0)
            {
                for (int j = 0; j < enemys[i]; j++)
                {
                    SpawnShip(i);
                }
            }
        }
        foreach (Ship s in ships)
        {
            RandomTeleport(s.gameObject);
            if (s.Group != 0)
            {
                s.RestoreState();
            }
            else
            {
                s.ApplyWindForce(windForce);
            }
            // s.ApplyWindForce(windForce);
            // s.RevalidMovement();
        }
        CloseDialogSpawnShipMenu();
    }

    private GameObject SpawnPlayerShip(ScriptableShipCustom customData = null)
    {
        GameObject newShip = Instantiate(prefabShip, ShipManager.transform, false);
        playerShip = newShip;

        Ship scriptShip = newShip.GetComponent<Ship>();
        customData.unions = new ScriptableShipCustom.Union[1] { ScriptableShipCustom.Union.Pirate };
        customData.battleId = PLAYER_SHIP_BATTLE_ID;
        scriptShip.InitFromCustomData(customData);

        scriptShip.Group = 0;
        scriptShip.shipId = 0;


        ships.Add(scriptShip);
        cameraFollow.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerShip.transform;

        scriptShip.EnableCannonSight = true;
        // scriptShip.Events.RegisterListener(Ship.EVENT_TACKED_OTHER_SHIP).AddListener(TackedOtherShip);
        scriptShip.Events.RegisterListener(Ship.EVENT_SHIP_DEFEATED).AddListener(delegate ()
        {
            GEventManager.Instance.InvokeEvent(GEventManager.EVENT_PLAYER_DEFEATED);
        });
        scriptShip.ApplyWindForce(windForce);


        PlayerSailCtrl.Instance.StartSync();
        PlayerCannonCtrl.Instance.StartSync();
        PlayerWheelCtrl.Instance.StartSync();
        ShipSkillCtrl.Instance.StartSync();
        return newShip;
    }
    private void TackedOtherShip()
    {
        Ship mainShip = playerShip.GetComponent<Ship>();
        Ship target = mainShip.LastCollision2D.gameObject.GetComponent<Ship>();
        CommandTackedOtherShip(target);

        // Time.timeScale = 0f;
        // InventoryMenu.GetComponent<ShipInventoryMenu>().ShowInventory(playerShip.GetComponent<Ship>().LastCollision2D.gameObject.GetComponent<Ship>());
    }

    public void CommandTackedOtherShip(Ship target)
    {
        Ship mainShip = playerShip.GetComponent<Ship>();
        if (target.IsSameGroup(mainShip))
        {
            TransferCargo(mainShip.CustomData, target.CustomData);
            return;
        }
        Debug.Log("Check Conduct " + (target.CurShipData.maxCrew / target.ShipData.maxCrew) + "/" + rateCrewConduct);
        // if (!target.IsDefeated && rateCrewConduct < ((float)target.CurShipData.maxCrew / target.ShipData.maxCrew))
        // {
        //     GameManager.Instance.ToastService.ShowMessage(
        //         GameText.GetText(GameText.TOAST_CANNOT_CLOSE_COMBAT)
        //         , 1f
        //     );
        //     return;
        // }
        GameManager.Instance.PauseGamePlay();

        PopupCtrl.ShowDialog(
            title: GameText.GetText(GameText.CONFIRM_CLOSE_COMBAT_TITLE),
            content: GameText.GetText(GameText.CONFIRM_CLOSE_COMBAT_CONTENT),
            okText: GameText.GetText(GameText.CONFIRM_COMMON_YES),
            cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
            onResult: (i) =>
            {
                if (i == 1)
                {
                    ShipTackedBattle result = CalculateTacked2Ship(playerShip.GetComponent<Ship>(), target);
                    if (result.Result == 1) //win
                    {
                        PromptResultWin(result);
                    }
                    else if (result.Result == 0)
                    {
                        //draw
                        PromptResultDraw(result);
                    }
                    else if (result.Result == 2)
                    {
                        //lose
                        GameManager.Instance.ResumeGamePlay();
                    }

                }
                else
                {
                    GameManager.Instance.ResumeGamePlay();
                }
            }
        );
        // Time.timeScale = 0f;
        // InventoryMenu.GetComponent<ShipInventoryMenu>().ShowInventory(playerShip.GetComponent<Ship>().LastCollision2D.gameObject.GetComponent<Ship>());
    }

    private void PromptResultDraw(ShipTackedBattle result)
    {
        PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.DIALOG_RESULT_COMBAT_TITLE),
                    content: GameText.GetText(GameText.DIALOG_RESULT_COMBAT_DRAW_CONTENT, result.Ship1Damaged, result.Ship2Damaged),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                    cancelText: null,
                    onResult: (i) =>
                    {
                        GameManager.Instance.ResumeGamePlay();
                    }
                );
    }

    public void GetRewardFromShip(ScriptableShipCustom target)
    {
        if (target.reward != null)
        {
            if (!CommonUtils.IsArrayNullEmpty(target.reward.gold))
            {
                int gold = target.reward.Gold.Sum();
                GameManager.Instance.AddGold(gold);
                ToastService.Instance.ShowMessage(
                    GameText.GetText(GameText.TOAST_LOOT_GOLD, gold)
                    , 3f
                );

            }
        }
    }
    private void PromptResultWin(ShipTackedBattle result)
    {
        PopupCtrl.ShowDialog(
                    title: GameText.GetText(GameText.DIALOG_RESULT_COMBAT_TITLE),
                    content: GameText.GetText(GameText.DIALOG_RESULT_COMBAT_WIN_CONTENT, result.Ship1Damaged),
                    okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                    cancelText: null,
                    onResult: (i) =>
                    {
                        ScriptableShipCustom targetCustom = result.ship2.CustomData;
                        GetRewardFromShip(targetCustom);
                        if (targetCustom.curShipData.hullHealth > 0)
                        {
                            seaBattleData.Reward.ships = CommonUtils.AddElemToArray(
                                seaBattleData.Reward.ships,
                                targetCustom
                            );
                            GameManager.Instance.ToastService.ShowMessage(
                                GameText.GetText(GameText.TOAST_INFORM_AUTO_TAKE_SHIP),
                                3f
                            );
                        }
                        else
                        {
                            GameManager.Instance.ToastService.ShowMessage(
                                GameText.GetText(GameText.TOAST_INFORM_LOOT_DEATH_SHIP),
                                3f
                            );
                        }
                        RemoveShip(result.ship2.shipId);
                        TransferCargo(result.ship1.CustomData, targetCustom);
                        // InventoryMenu.GetComponent<ShipInventoryMenu>().ShowInventory(result.ship2);
                        // List<ScriptableShipCustom> shipCustoms = new List<ScriptableShipCustom>();
                        // shipCustoms.Add(result.ship1.CustomData);
                        // shipCustoms.Add(result.ship2.CustomData);
                        // ShipInventoryCtrl transferCtrl = GameManager.Instance.ShipInventoryCtrl;
                        // transferCtrl.RegisterAvaiableShip(shipCustoms.ToArray(), 0);
                        // transferCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Transfer);
                    }
                );
    }

    private void TransferCargo(ScriptableShipCustom data1, ScriptableShipCustom data2)
    {
        GameManager.Instance.PauseGamePlay();
        List<ScriptableShipCustom> shipCustoms = new List<ScriptableShipCustom>();
        shipCustoms.Add(data1);
        shipCustoms.Add(data2);
        ShipInventoryCtrl transferCtrl = GameManager.Instance.ShipInventoryCtrl;
        transferCtrl.RegisterAvaiableShip(shipCustoms.ToArray(), 0);
        transferCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.Transfer);
    }

    public void ToggleInventory()
    {
        ShipInventoryCtrl transferCtrl = GameManager.Instance.ShipInventoryCtrl;
        if (transferCtrl.isShow)
        {
            transferCtrl.HideInventory();
        }
        else
        {
            List<ScriptableShipCustom> shipCustoms = new List<ScriptableShipCustom>();
            shipCustoms.Add(playerShip.GetComponent<Ship>().CustomData);
            transferCtrl.RegisterAvaiableShip(shipCustoms.ToArray(), 0);
            transferCtrl.ShowInventory(ShipInventoryCtrl.InventoryMode.View);
        }
    }

    private ShipTackedBattle CalculateTacked2Ship(Ship ship1, Ship ship2)
    {
        ShipTackedBattle battle = new ShipTackedBattle(ship1, ship2);
        battle.CalculateResult();
        return battle;
    }

    public void RemoveShip(int shipId)
    {
        Ship s = AllShip.FirstOrDefault(x => x.shipId.Equals(shipId));
        if (s != null)
        {
            AllShip.Remove(s);
            Destroy(s.gameObject);
        }
    }

    private GameObject SpawnShip(int group)
    {
        GameObject newShip = Instantiate(prefabShip, ShipManager.transform, false);
        Ship scriptShip = newShip.GetComponent<Ship>();
        if (scriptableShips.Length > 0)
        {
            int choose = Random.Range(0, scriptableShips.Length);
            scriptShip.ShipData = scriptableShips[choose].Clone<ScriptableShip>();
        }
        else
        {
            scriptShip.ShipData = playerStartShip.Clone<ScriptableShip>();
        }
        scriptShip.Group = group + 1;
        scriptShip.shipId = ships.Count + 1;
        ships.Add(scriptShip);
        return newShip;
    }

    public GameObject SpawnShipFromData(ScriptableShipCustom customData)
    {
        Debug.Log("Spawn Ship");
        GameObject newShip = Instantiate(prefabShip, ShipManager.transform, false);
        Ship scriptShip = newShip.GetComponent<Ship>();
        scriptShip.InitFromCustomData(customData);

        scriptShip.shipId = ships.Count + 1;
        scriptShip.Events.RegisterListener(Ship.EVENT_SHIP_DEFEATED).AddListener(delegate ()
        {
            GEventManager.Instance.InvokeEvent(GEventManager.EVENT_SHIP_DEFEAT);
        });
        scriptShip.ApplyWindForce(windForce);
        ships.Add(scriptShip);
        return newShip;
    }
    public void ReturnTown(bool isRun = false)
    {
        if (!isRun) TakeReward();
        UpdateGameData();
        seaBattleData = null;
        GameManager.Instance.ChangeScene(GameManager.Instance.townSceneName);
    }

    public IEnumerator InformLoseGame()
    {
        float delayTime = 3f;
        GameManager.Instance.ToastService.ShowMessage(
            GameText.GetText(GameText.TOAST_YOU_LOSE_GAME),
            delayTime
        );
        yield return new WaitForSeconds(delayTime);
        GameManager.Instance.ToastService.ShowMessage(
            GameText.GetText(GameText.TOAST_INFORM_LOSE_NEW_SHIP),
            delayTime
        );
        yield return new WaitForSeconds(delayTime);
        //give new ship for player
#if UNITY_EDITOR
        GameManager.Instance.GameData.playerShip = GameManager.Instance.playerStarterShipTest.Clone<ScriptableShipCustom>();
#else
        GameManager.Instance.GameData.playerShip = GameManager.Instance.playerStarterShip.Clone<ScriptableShipCustom>();
#endif
        seaBattleData = null;
        //set intent for town manager
        GameManager.Instance.ChangeScene(GameManager.Instance.townSceneName, TownManager.INTENT_LOSE_GAME_RETURN);
    }

    public ScriptableStateShip GetImgStateShip(ScriptableShipCustom.Union union)
    {
        switch (union)
        {
            case ScriptableShipCustom.Union.Pirate:
                return ImgShipGroups[0];
            case ScriptableShipCustom.Union.Marine:
                return ImgShipGroups[1];
            case ScriptableShipCustom.Union.Red:
                return ImgShipGroups[2];
            case ScriptableShipCustom.Union.Blue:
                return ImgShipGroups[3];
            case ScriptableShipCustom.Union.Green:
                return ImgShipGroups[4];
            case ScriptableShipCustom.Union.Yellow:
                return ImgShipGroups[5];
            default:
                return ImgShipGroups[1];
        }
    }

    internal void CloseDialogSpawnShipMenu()
    {
        DialogInputSpawnShip.SetActive(false);
        Modal.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnApplicationQuit()
    {
        //collect seaBattleData
        // CollectSceneData();
        //update gameData
        // UpdateGameData();
        //save
        // SaveGame();
    }

    public override BaseDataEntity GetDataForSave()
    {
        CollectSceneData();
        UpdateGameData();
        return base.GetDataForSave();
    }

    public void CollectSceneData()
    {
        if (SeaBattleData == null)
            SeaBattleData = new SeaBattleData();
        SeaBattleData.SetShipData(ships.Select(x => x.GetComponent<Ship>()).ToArray());
        SeaBattleData.SetWindData(windForce, accumWindChangeInterval);
    }

    public void UpdateGameData()
    {
        if (playerShip != null)
        {
            gameManager.GameData.playerShip = playerShip.GetComponent<Ship>().CustomData;
        }
    }

    public void TakeReward()
    {
        if (SeaBattleData.Reward != null)
        {
            RewardBattle reward = SeaBattleData.Reward;
            if (!CommonUtils.IsArrayNullEmpty(reward.gold))
            {
                GameManager.Instance.AddGold(reward.gold.Sum());
            }
            if (!CommonUtils.IsArrayNullEmpty(reward.gem))
            {
                GameManager.Instance.AddGem(reward.gem.Sum());
            }
            if (!CommonUtils.IsArrayNullEmpty(reward.ships))
            {
                GameManager.Instance.GameData.otherShips = CommonUtils.AddElemToArray(GameManager.Instance.GameData.otherShips, reward.ships);
            }
        }
    }
    public void SaveGame()
    {
        gameManager.SaveGame();
    }

    public void LoadGame()
    {
        if (SeaBattleData != null)
        {
            windForce = JsonUtility.FromJson<Vector2>(SeaBattleData.windDataJson);
            OnWindChange.Invoke();
            accumWindChangeInterval = SeaBattleData.windAccumTime;

            bool donePlayerShip = false;
            int i = SeaBattleData.playerShipIndex;
            for (i = 0; i < SeaBattleData.shipDatas.Length; i++)
            {
                if (donePlayerShip && i == SeaBattleData.playerShipIndex)
                {
                    //ignore if player ship because already init
                    i++;
                    if (i >= SeaBattleData.shipDatas.Length) break;
                }
                // Debug.Log("Init custom0 " + JsonUtility.ToJson(SeaBattleData.shipDatas[i]));
                ScriptableShipCustom customData = SeaBattleData.shipDatas[i].Restore<ScriptableShipCustom>();
                // Debug.Log("Init custom1 " + JsonUtility.ToJson(customData));
                // Debug.Log("Init custom2 " + MyJsonUtil.SerializeScriptableObject(customData));
                GameObject newShip = null;
                if (i != SeaBattleData.playerShipIndex)
                {
                    newShip = SpawnShipFromData(customData);
                }
                else
                {
                    newShip = SpawnPlayerShip(customData);
                }
                newShip.transform.localPosition = new Vector3(
                    SeaBattleData.transPosJsons[2 * i],
                    SeaBattleData.transPosJsons[2 * i + 1],
                    0
                );
                newShip.transform.localRotation = Quaternion.Euler(0,
                    0,
                    SeaBattleData.transRotJsons[i]
                );
                newShip.GetComponent<Ship>().ApplyWindForce(windForce);
                if (!donePlayerShip)
                {
                    //init player ship first
                    donePlayerShip = true;
                    i = 0;
                }
            }

        }
    }

    internal PoolManager<WrapPool> GetPoolCannon(string cannonCodeName)
    {
        return dictCannons[cannonCodeName];
    }
}
