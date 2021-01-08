using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [System.Flags]
    public enum CannonDirection
    {
        None = 0, Front = 1, Right = 2, Left = 4, Back = 8
    }
    public static GameManager instance;

    public PoolManager<WrapPool> poolCannon;
    public GameObject prefabCannon;
    public GameObject prefabShip;
    public GameObject playerShip;
    public GameObject enemyShip;



    public Vector2 windForce;
    public float intervalWindChange = 30f;


    public float accumWindChangeInterval = 0f;

    public UnityEvent OnWindChange;

    List<ScriptableCannonBall> ScriptableCannonBalls;
    public ScriptableShip[] scriptableShips;
    public ScriptableShip playerStartShip;

    private List<Ship> ships;
    public GameObject ShipManager;

    public Tilemap battleFields;
    public GameObject cameraFollow;

    public WeatherWindRate[] weatherWindRates;

    public Canvas Modal;
    public GameObject DialogInputSpawnShip;

    public ScriptableStateShip[] ImgShipGroups;

    public Sprite MinimapAllySprite;

    void Awake()
    {
        instance = this;
        Resources.LoadAll<ScriptableCannonBall>("ScriptableObjects");
        scriptableShips = Resources.LoadAll<ScriptableShip>("ScriptableObjects");
        ScriptableCannonBalls = Resources.FindObjectsOfTypeAll<ScriptableCannonBall>().Cast<ScriptableCannonBall>().ToList();


        ships = new List<Ship>();
        for (int i = 0; i < ShipManager.transform.childCount; i++)
        {
            ships.Add(ShipManager.transform.GetChild(i).GetComponent<Ship>());
        }
        // ships.Add(playerShip.GetComponent<Ship>());
        // ships.Add(enemyShip.GetComponent<Ship>());
        // ships.Add(enemyShip.GetComponent<Ship>());
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayerShip();
        RandomWindForce();
        GameObject newCannon = Instantiate(prefabCannon);
        newCannon.SetActive(false);
        newCannon.GetComponent<CannonShot>().Data = ScriptableCannonBalls.Where(x => x.name == "RoundShot").First();
        // CannonPool cannon = prefabCannon.Find
        WrapPool cannon = ScriptableObject.CreateInstance<WrapPool>();
        cannon.cannonBall = newCannon;
        poolCannon = new PoolManager<WrapPool>(cannon);
        poolCannon.OnCreateNew = delegate ()
        {
            // WrapPool newInstance = poolCannon.pooledObjects.Last();
            WrapPool newInstance = poolCannon.newInstance;
            CannonShot cs = newInstance.cannonBall.GetComponent<CannonShot>();
            newInstance.cannonBall = Instantiate(prefabCannon);
            newInstance.cannonBall.GetComponent<CannonShot>().Data = cs.Data.Clone<ScriptableCannonBall>();
            // newInstance.SetActive(false);
        };

    }

    // Update is called once per frame
    void Update()
    {
        accumWindChangeInterval += Time.deltaTime;
        if (accumWindChangeInterval >= intervalWindChange)
        {
            RandomWindForce();
            accumWindChangeInterval = 0f;
        }
    }

    public void FireCannon(Vector2 from, Vector2 to, float speed)
    {
        WrapPool wrapPool = poolCannon.GetPooledObject();
        if (wrapPool != null)
        {
            Debug.Log("fire");
            GameObject actualCannon = wrapPool.cannonBall;
            CannonShot shot = actualCannon.GetComponent<CannonShot>();
            shot.ResetTravel();
            actualCannon.transform.position = from;
            shot.owner = playerShip;
            shot.Target = to;
            shot.speed = speed;
            shot.OnImpactTarget = delegate ()
            {
                poolCannon.RePooledObject(wrapPool);
            };
            shot.gameObject.SetActive(true);
            shot.StartTravel();
        }
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
            float maxRange = 0f;
            float[,] level = new float[weatherWindRates.Length, 2];
            for (int i = 0; i < weatherWindRates.Length; i++)
            {
                level[i, 0] = maxRange;
                maxRange += weatherWindRates[i].percent;
                level[i, 1] = maxRange - 1;
            }
            float selected = Random.Range(0f, maxRange - 1);
            Debug.Log("random wind selected " + selected);

            for (int i = 0; i < weatherWindRates.Length; i++)
            {
                Debug.Log("random wind check " + level[i, 0] + " - " + level[i, 1]);
                if (level[i, 0] <= selected && level[i, 1] >= selected)
                {
                    Debug.Log("random wind level " + i + " " + weatherWindRates[i].min + "-" + weatherWindRates[i].max);
                    return (float)Math.Round(Random.Range(weatherWindRates[i].min, weatherWindRates[i].max), 1);
                }
            }
            return 0f;
        }
    }

    public void RandomTeleport(GameObject gameObject)
    {
        int randX = Random.Range(battleFields.cellBounds.xMin, battleFields.cellBounds.xMax);
        int randY = Random.Range(battleFields.cellBounds.yMin, battleFields.cellBounds.yMax);
        Vector3 newPos = battleFields.GetCellCenterWorld(new Vector3Int(randX, randY, 0));
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

    internal void SpawnNewShips(int[] enemys)
    {
        foreach (Ship s in ships)
        {
            Destroy(s.gameObject);
        }
        ships.Clear();
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
        SpawnPlayerShip();
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

    private void SpawnPlayerShip()
    {
        GameObject newShip = Instantiate(prefabShip, ShipManager.transform, false);
        foreach (Transform eachChild in newShip.transform)
        {
            if (eachChild.name == "MinimapInfo")
            {
                Debug.Log("Child found. Mame: " + eachChild.name);
                eachChild.GetComponent<SpriteRenderer>().sprite = MinimapAllySprite;
            }
        }
        Ship scriptShip = newShip.GetComponent<Ship>();
        scriptShip.ShipData = playerStartShip.Clone<ScriptableShip>();
        playerShip = newShip;
        scriptShip.Group = 0;
        scriptShip.ImgStateShip = ImgShipGroups[0];
        ships.Add(scriptShip);
        cameraFollow.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = playerShip.transform;

        PlayerSailCtrl.Instance.StartSync();
        PlayerCannonCtrl.Instance.StartSync();
        PlayerWheelCtrl.Instance.StartSync();
    }

    private GameObject SpawnShip(int group)
    {
        GameObject newShip = Instantiate(prefabShip, ShipManager.transform, false);
        Ship scriptShip = newShip.GetComponent<Ship>();
        if (scriptableShips.Length > 0)
        {
            int choose = Random.Range(0, scriptableShips.Length - 1);
            scriptShip.ShipData = scriptableShips[choose].Clone<ScriptableShip>();
        }
        else
        {
            scriptShip.ShipData = playerStartShip.Clone<ScriptableShip>();
        }
        scriptShip.Group = group + 1;
        scriptShip.ImgStateShip = ImgShipGroups[scriptShip.Group];
        ships.Add(scriptShip);
        return newShip;
    }

    internal void CloseDialogSpawnShipMenu()
    {
        DialogInputSpawnShip.SetActive(false);
        Modal.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}
