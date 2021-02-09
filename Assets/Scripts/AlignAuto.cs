using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignAuto : MonoBehaviour
{
    public enum AlignType
    {
        Left, Center, Right
    }

    public AlignType type;
    public float amount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector2(
          amount - transform.localScale.x / 2,
            transform.localPosition.y
        );
    }
}
