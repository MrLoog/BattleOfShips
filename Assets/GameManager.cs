using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
    public GameObject playerShip;
    public GameObject enemyShip;

    public Vector2 windForce;
    public float intervalWindChange = 30f;
    public float accumWindChangeInterval = 0f;

    public UnityEvent OnWindChange;

    List<ScriptableCannonBall> ScriptableCannonBalls;

    private List<Ship> ships;

    void Awake()
    {
        instance = this;
        Resources.LoadAll<ScriptableCannonBall>("ScriptableObjects");
        ScriptableCannonBalls = Resources.FindObjectsOfTypeAll<ScriptableCannonBall>().Cast<ScriptableCannonBall>().ToList();

        ships = new List<Ship>();
        ships.Add(playerShip.GetComponent<Ship>());
        RandomWindForce();
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject newCannon = Instantiate(prefabCannon);
        newCannon.SetActive(false);
        newCannon.GetComponent<CannonShot>().Data = ScriptableCannonBalls.Where(x => x.name == "RoundShot").First();
        // CannonPool cannon = prefabCannon.Find
        WrapPool cannon = ScriptableObject.CreateInstance<WrapPool>();
        cannon.cannonBall = newCannon;
        poolCannon = new PoolManager<WrapPool>(cannon);
        poolCannon.OnCreateNew = delegate ()
        {
            WrapPool newInstance = poolCannon.pooledObjects.Last();
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
        return playerShip.GetComponent<Ship>().GetFrontDirection();
    }

    public void RandomWindForce()
    {
        float force = (float)Math.Round(Random.Range(0.1f, 1f), 1);
        float direction = Random.Range(-180, 180);

        windForce = VectorUtils.Rotate(Vector2.up, direction, true).normalized * force;
        OnWindChange.Invoke();
        foreach (Ship s in ships)
        {
            s.ApplyWindForce(windForce);
        }
    }
}
