using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnShipMenu : MonoBehaviour
{
    public InputField whiteTeam;
    public InputField redTeam;
    public InputField blueTeam;
    public InputField greenTeam;
    public InputField yellowTeam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSpawnPress()
    {
        if (whiteTeam.text.Length == 0) whiteTeam.text = "0";
        if (redTeam.text.Length == 0) redTeam.text = "0";
        if (blueTeam.text.Length == 0) blueTeam.text = "0";
        if (greenTeam.text.Length == 0) greenTeam.text = "0";
        if (yellowTeam.text.Length == 0) yellowTeam.text = "0";
        int[] enemys = new int[]{
            int.Parse(whiteTeam.text),
            int.Parse(redTeam.text),
            int.Parse(blueTeam.text),
            int.Parse(greenTeam.text),
            int.Parse(yellowTeam.text)
        };
        SeaBattleManager.Instance.SpawnNewShips(enemys);
    }

    public void OnCancelPress()
    {
        SeaBattleManager.Instance.CloseDialogSpawnShipMenu();
    }
}
