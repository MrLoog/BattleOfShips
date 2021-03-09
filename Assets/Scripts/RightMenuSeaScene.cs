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
        if (SeaBattleManager.Instance.IsBattle)
        {
            GameManager.Instance.ToastService.ShowMessage(GameText.GetText(GameText.TOAST_CANNOT_RETURN_TOWN), 5f);
        }
        else
        {
            GameManager.Instance.PauseGamePlay();
            GameManager.Instance.PopupCtrl.ShowDialog(
                title: GameText.GetText(GameText.CONFIRM_RETURN_TOWN_TITLE),
                content: GameText.GetText(GameText.CONFIRM_RETURN_TOWN_CONTENT),
                okText: GameText.GetText(GameText.CONFIRM_COMMON_OK),
                cancelText: GameText.GetText(GameText.CONFIRM_COMMON_NO),
                onResult: (i) =>
                {
                    GameManager.Instance.ResumeGamePlay();
                    if (i == ModalPopupCtrl.RESULT_POSITIVE)
                    {

                        SeaBattleManager.Instance.UpdateGameData();
                        SeaBattleManager.Instance.seaBattleData = null;
                        GameManager.Instance.ChangeScene(GameManager.Instance.townSceneName);
                        Debug.Log("Press Return Town");
                    }
                }
            );
        }
        // UnityEngine.SceneManagement.SceneManager.LoadScene("TownScene");
    }
}
