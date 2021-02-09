using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HumanFly : MonoBehaviour
{
    public float time = 1f;
    private float process = 0f;
    public bool isAnimate = false;
    public Vector2 Target;
    public Sprite[] sprites;
    public UnityAction OnAnimateDone;
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
            transform.localPosition = Vector2.Lerp(Vector2.zero, Target, process / time);
            if (process >= time)
            {
                isAnimate = false;
                // gameObject.SetActive(false);
                if (OnAnimateDone != null)
                {
                    OnAnimateDone.Invoke();
                }
                else
                {
                    Destroy(gameObject.transform.parent.gameObject);
                }

            }
        }
    }

    public void StartAnimate(Vector2 to)
    {
        Target = to;
        RandomSprite();
        process = 0;
        isAnimate = true;
        // gameObject.SetActive(true);
    }

    public void RandomSprite()
    {
        int pick = Random.Range(0, sprites.Length - 1);
        Debug.Log("pick " + pick);
        GetComponent<SpriteRenderer>().sprite = sprites[pick];
    }
}
