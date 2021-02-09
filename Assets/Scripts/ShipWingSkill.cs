using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWingSkill : ShipSkill
{
    public float speed = 1f;
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
        GetComponent<Animator>().Play("Wing");
    }

    public override void DeActiveSkill()
    {
        base.DeActiveSkill();
        GetComponent<Animator>().Play("WingReverse");
        // gameObject.SetActive(false);
    }

    public void OnAnimationCloseDone()
    {
        if (!isActiveSkill)
        {
            gameObject.SetActive(false);
            ship.fixedSpeed = 0f;
            ship.RevalidMovement();
        }
    }

    public void OnAnimationOpenDone()
    {
        if (isActiveSkill)
        {
            ship.fixedSpeed = speed;
            ship.RevalidMovement();
        }
    }


    public override void ToggleSkill()
    {
        base.ToggleSkill();
        if (isActiveSkill)
        {
            gameObject.SetActive(true);
            GetComponent<Animator>().Play("Wing");
        }
        else
        {
            GetComponent<Animator>().Play("WingReverse");
            // gameObject.SetActive(false);
        }
    }
}
