using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShieldSkill : ShipSkill
{
    public float maxDamage = 100f;
    private float damageBlocked = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ActiveSkill()
    {
        base.ActiveSkill();
        gameObject.SetActive(true);
        damageBlocked = 0f;
    }

    public override void DeActiveSkill()
    {
        base.DeActiveSkill();
        gameObject.SetActive(false);
    }


    public override void ToggleSkill()
    {
        base.ToggleSkill();
        if (isActiveSkill)
        {
            gameObject.SetActive(true);
            damageBlocked = 0f;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == GameSettings.TAG_CANNON)
        {
            CannonShot cannon = other.gameObject.GetComponent<CannonShot>();
            if (cannon.owner.shipId != ship.shipId)
            {
                cannon.PerformExplosion();
                damageBlocked += cannon.GetDamage();
                cannon.EndTravel(false);

                if (damageBlocked >= maxDamage)
                {
                    DeActiveSkill();
                }
            }
        }
    }

}
