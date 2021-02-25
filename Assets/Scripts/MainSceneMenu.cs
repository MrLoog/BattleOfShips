using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneMenu : MonoBehaviour
{

    public GameObject ButtonResume;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.GameData.process > GameData.PROCESS_INIT_FIRST_SHIP)
        {
            ButtonResume.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressNewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void PressResume()
    {
        GameManager.Instance.ResumeGame();
    }

}
