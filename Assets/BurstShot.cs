using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstShot : MonoBehaviour
{
    public bool isShot = false;
    public float shotSpeed = 3f;

    public Vector2 shotDirection;
    public float accumTravel = 0f;
    public float range = 10f;

    [Tooltip("Minimum second charge for minimum damage")]
    public float minCharge = 5f;

    [Tooltip("Damage increase per second after charge minimun")]
    public float damageStep = 50f;
    public float minDamage = 50f;
    public float maxDamage = 200f;

    public float damage = 0f;
    public float charge = 0f;

    [Tooltip("Damage Decrease per second")]
    public float damageDecrease = 10f;

    public bool isCharge = false;
    public GameObject ExplosionEffect;
    public GameObject CannonBall;
    public GameObject trail;

    public float delayDestroy = 1f;
    public bool isEndTravel = false;

    public Ship owner;

    private void Awake()
    {
        // StartCoroutine(ResetCooldown());
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (isEndTravel) return;
        if (isShot)
        {
            accumTravel += Time.deltaTime * shotSpeed;
            // transform.position += (Vector3)shotDirection * Time.deltaTime * shotSpeed;
            // transform.position += (Vector3)shotDirection * Time.deltaTime * shotSpeed * (minDamage / damage);
            transform.position += (Vector3)shotDirection * Time.deltaTime * shotSpeed;

            transform.localScale = new Vector3(damage / minDamage, damage / minDamage, 0);
            damage -= Time.deltaTime * damageDecrease;
            // if (accumTravel > range)
            if ((damage <= 0) || (damageDecrease <= 0 && accumTravel >= range))
            {
                EndTravel();
            }
        }
        else if (isCharge)
        {
            charge += Time.deltaTime;
            if (charge >= minCharge && damage == 0)
            {
                damage = minDamage;
            }
            else if (charge >= minCharge && damage > 0)
            {
                damage = minDamage + (charge - minCharge) * damageStep;
                if (damage > maxDamage) damage = maxDamage;
                transform.localScale = new Vector3(damage / minDamage, damage / minDamage, 0);
            }

        }
    }

    public void StartCharge()
    {
        isCharge = true;
        CannonBall.SetActive(true);
        trail.SetActive(false);
        damage = 0f;
        charge = 0f;
        transform.localScale = Vector3.one;
    }

    public bool Shot(Ship owner)
    {
        if (damage <= 0) return false;
        isShot = true;
        accumTravel = 0f;
        this.owner = owner;
        shotDirection = owner.ShipVelocity.normalized;
        transform.SetParent(null);
        trail.SetActive(true);
        return true;
    }

    // public override void DeActiveSkill()
    // {
    //     base.DeActiveSkill();
    //     isShot = false;
    //     CannonBall.SetActive(false);
    //     trail.GetComponent<TrailRenderer>().Clear();
    //     trail.SetActive(false);
    //     transform.SetParent(ship.transform);
    //     transform.localPosition = originPos;
    // }

    // public override void ToggleSkill()
    // {
    //     if (!isActiveSkill)
    //     {
    //         StartCharge();
    //     }
    //     else
    //     {
    //         if (!isShot)
    //         {
    //             Shot();
    //         }
    //         else
    //         {
    //             DeActiveSkill();
    //         }
    //     }
    // }

    public void EndTravel()
    {
        CannonBall.SetActive(false);
        isEndTravel = true;
        Destroy(gameObject, delayDestroy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isShot && other.tag == GameSettings.TAG_SHIP && !owner.IsSameShip(other.GetComponent<Ship>()))
        {
            Instantiate(ExplosionEffect, transform.position, Quaternion.Euler(0, 0, 0));
            EndTravel();
        }
    }

}
