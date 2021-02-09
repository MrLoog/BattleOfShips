using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedCtrl : MonoBehaviour
{
    public Sprite pauseState;
    public Sprite playState;
    public Sprite speed2State;
    public Sprite speed3State;

    public Image btnSpeed;

    public int state = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSpeed()
    {
        state = (state == 3 ? 0 : (state + 1));
        Time.timeScale = state;
        switch (state)
        {
            case 0:
                btnSpeed.GetComponent<Image>().sprite = pauseState;
                break;
            case 1:
                btnSpeed.GetComponent<Image>().sprite = playState;
                break;
            case 2:
                btnSpeed.GetComponent<Image>().sprite = speed2State;
                break;
            case 3:
                btnSpeed.GetComponent<Image>().sprite = speed3State;
                break;
            default:
                break;
        }
    }
}
