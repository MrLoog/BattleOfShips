using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenRotate : MonoBehaviour
{
    public bool frozenX = false;
    public float frozenXValue = 0f;
    public bool frozenY = false;
    public float frozenYValue = 0f;
    public bool frozenZ = false;
    public float frozenZValue = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            frozenX ? frozenXValue : transform.rotation.eulerAngles.x,
            frozenY ? frozenYValue : transform.rotation.eulerAngles.y,
            frozenZ ? frozenZValue : transform.rotation.eulerAngles.z
        );
    }
}
