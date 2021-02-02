using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBurstShotSkill : ShipSkill
{
    public float coolTime = 20f;
    public float accumCooltime = 20f;


    public bool IsCooldown => accumCooltime < coolTime;

    public GameObject CannonBallShot;
    public GameObject prefabCannonBall;

    public bool IsCharge => CannonBallShot != null && CannonBallShot.activeInHierarchy;

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
        if (IsCooldown) accumCooltime += Time.deltaTime;
    }
    void FixedUpdate()
    {
    }
    public override void ActiveSkill()
    {
        if (!isActiveSkill)
        {
            StartCharge();
        }
        else
        {
            Shot();
        }

    }

    public void InitShot()
    {
        if (CannonBallShot == null)
        {
            CannonBallShot = Instantiate(prefabCannonBall, transform, false);
        }
        else
        {
            CannonBallShot.SetActive(true);
        }
    }

    public void StartCharge()
    {
        if (!IsCooldown)
        {
            isActiveSkill = true;
            InitShot();
            CannonBallShot.GetComponent<BurstShot>().StartCharge();
        }
    }

    public void Shot()
    {
        if (CannonBallShot.GetComponent<BurstShot>().Shot(ship))
        {
            CannonBallShot = null;
            accumCooltime = 0f;
        }
    }

    public override void DeActiveSkill()
    {
        base.DeActiveSkill();
        if (CannonBallShot != null)
        {
            CannonBallShot.SetActive(false);
        }
    }

    public override void ToggleSkill()
    {
        if (!isActiveSkill)
        {
            StartCharge();
        }
        else
        {
            if (IsCharge)
            {
                Shot();
            }
            else
            {
                DeActiveSkill();
            }
        }
    }


    public override bool RegisterShip(Ship ship)
    {
        accumCooltime = coolTime;
        // transform.localPosition = new Vector3(transform.localPosition.x, -ship.model.transform.localScale.y / 2);
        return base.RegisterShip(ship);
    }
}
