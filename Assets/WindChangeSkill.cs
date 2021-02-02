using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChangeSkill : ShipSkill
{
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
        GameManager.instance.RandomWindForce();
    }

    public override void ToggleSkill()
    {
        // base.ToggleSkill();
        ActiveSkill();
    }
}
