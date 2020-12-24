using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PoolManager<WrapPool> poolCannon;
    public GameObject prefabCannon;
    public GameObject playerShip;
    public GameObject enemyShip;
    List<ScriptableCannonBall> ScriptableCannonBalls;

    void Awake()
    {
        instance = this;
        Resources.LoadAll<ScriptableCannonBall>("ScriptableObjects");
        ScriptableCannonBalls = Resources.FindObjectsOfTypeAll<ScriptableCannonBall>().Cast<ScriptableCannonBall>().ToList();

    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject newCannon = Instantiate(prefabCannon);
        newCannon.SetActive(false);
        newCannon.GetComponent<CannonShot>().Data = ScriptableCannonBalls.First();
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
            shot.enableTravel = true;
            shot.OnImpactTarget = delegate ()
            {
                poolCannon.RePooledObject(wrapPool);
            };
            shot.gameObject.SetActive(true);
        }
    }

    internal void PlayerFireCannon(PlayerCannonCtrl.CannonDirection direction)
    {
        FireCannon(playerShip.transform.position, enemyShip.transform.position, 5f);
    }
}
