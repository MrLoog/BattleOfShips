using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanFly : MonoBehaviour
{
    public float time = 1f;
    private float process = 0f;
    public bool isAnimate = false;
    public Vector2 Target;
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimate)
        {
            process += Time.deltaTime;
            transform.position = Vector2.Lerp(Vector2.zero, Target, process / time);
            if (process >= time)
            {
                isAnimate = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void StartAnimate(Vector2 to)
    {
        isAnimate = true;
        gameObject.SetActive(true);
    }
}
