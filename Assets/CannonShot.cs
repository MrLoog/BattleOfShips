using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public Transform model;
    public GameObject Explosion;
    internal float speed;
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
    public Action OnImpactTarget { get; internal set; }

    private Rigidbody2D rigidbody2D;
    internal GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Vector2.SqrMagnitude((Vector2)transform.position - Target) > 0.1f)
        if (Vector2.Distance(Target, (Vector2)transform.position) > 0)
        {
            rigidbody2D.velocity = (Target - (Vector2)transform.position).normalized * speed;
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;

        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (owner != null && col.tag == GameSettings.TAG_SHIP && !owner.Equals(col.gameObject))
        {
            Instantiate(Explosion, transform.position, Quaternion.Euler(0, 0, 0));
            Ship ship = col.gameObject.GetComponent<Ship>();
            ship.ApplyDamage(5f);
            StartCoroutine(ShotDone(trailTime));
            Debug.Log("OnTriggerEnter2D" + col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        }
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
