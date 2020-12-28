using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public Transform model;
    public GameObject Explosion;
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

    private Rigidbody2D rigidbody2D;
    internal GameObject owner;
    internal Vector2 fireDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        LastPos = (Vector2)transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableTravel)
        {
            if (Vector2.Distance(Target, (Vector2)transform.position) > 0)
            {
                rigidbody2D.velocity = fireDirection.normalized * speed;
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
        if (owner != null && col.tag == GameSettings.TAG_SHIP && !owner.Equals(col.gameObject))
        {
            Instantiate(Explosion, transform.position, Quaternion.Euler(0, 0, 0));
            Ship ship = col.gameObject.GetComponent<Ship>();
            ship.ApplyDamage(5f, gameObject);
            EndTravel();
            // Debug.Log("OnTriggerEnter2D" + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        }
    }

    void EndTravel()
    {
        rigidbody2D.velocity = Vector2.zero;
        enableTravel = false;
        GetComponentInChildren<BaseShot>().ShotImg.SetActive(false);
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
        Debug.Log("Fire " + speed + "/" + maxTime);
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
