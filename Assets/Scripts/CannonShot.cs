using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public Transform model;
    public GameObject Explosion;
    public GameObject DripWater;
    internal float speed;
    private float maxTime;
    private float travelTime;
    internal bool enableTravel = false;

    private float trailTime;

    private ScriptableCannonBall data;
    public ScriptableCannonBall Data
    {
        set
        {
            foreach (Transform child in model)
            {
                GameObject.Destroy(child.gameObject);
            }
            data = value;
            InitModel();
        }
        get
        {
            return data;
        }
    }

    public void InitModel()
    {
        Instantiate(data.prefab, model, false);
        trailTime = GetComponentInChildren<TrailRenderer>().time;
    }

    public Vector2 Target { get; internal set; }
    public Vector2 LastPos;
    public Action OnImpactTarget { get; internal set; }

    private Rigidbody2D _rigidbody2D;
    internal Ship owner;
    internal Vector2 fireDirection;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        LastPos = (Vector2)transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableTravel)
        {
            if (Vector2.Distance(Target, (Vector2)transform.position) > 0)
            {
                _rigidbody2D.velocity = fireDirection.normalized * speed;
                travelTime += Time.deltaTime;
                if (travelTime >= maxTime)
                {
                    EndTravel();
                }
            }
        }
        // if (Vector2.SqrMagnitude((Vector2)transform.position - Target) > 0.1f)

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (owner != null && col.tag == GameSettings.TAG_SHIP)
        {
            Ship ship = col.gameObject.GetComponent<Ship>();
            if (owner.shipId != ship.shipId)
            {
                PerformExplosion();
                ship.TakeDamage(GetShipDamage(), gameObject);
                EndTravel(false);
            }
            // Debug.Log("OnTriggerEnter2D" + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        }
    }

    public void PerformExplosion()
    {
        Instantiate(Explosion, transform.position, Quaternion.Euler(0, 0, 0));
    }
    public float GetDamage()
    {
        return data.damage;
    }

    public DamageDealShip GetShipDamage()
    {
        return new DamageDealShip()
        {
            hullDamage = data.hullDamage,
            sailDamage = data.sailDamage,
            crewDamage = data.crewDamage
        };
    }

    public void EndTravel(bool onSea = true)
    {

        _rigidbody2D.velocity = Vector2.zero;
        enableTravel = false;
        if (onSea)
        {
            GetComponentInChildren<BaseShot>().ShotImg.SetActive(false);
            GameObject dripwater = Instantiate(DripWater, transform.position, Quaternion.Euler(0, 0, 0));
        }

        StartCoroutine(ShotDone(trailTime));
    }

    public void StartTravel()
    {
        LastPos = (Vector2)transform.position;
        Target = fireDirection.normalized * data.range + LastPos;
        speed = data.speed;
        maxTime = data.range / speed;
        travelTime = 0;
        enableTravel = true;
        // Debug.Log("Fire " + speed + "/" + maxTime);
        GetComponentInChildren<BaseShot>().ShotImg.SetActive(true);
    }

    IEnumerator ShotDone(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        OnImpactTarget.Invoke();
        yield return null;
    }

    public void ResetTravel()
    {
        GetComponentInChildren<TrailRenderer>().Clear();
    }
}
