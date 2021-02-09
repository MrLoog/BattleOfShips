using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightMenuSeaScene : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressReturnTown()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TownScene");
        Debug.Log("Press Return Town");
    }
}
