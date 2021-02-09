using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalPopupCtrl : MonoBehaviour
{
    public Text title;
    public Text content;
    public Button btnDone;
    public Button btnCancel;
    public Button btnClose;

    public UnityAction<int> OnSelectDone;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDialog(string title = "No Title"
    , string content = "Are you sure?"
    , string okText = "Done"
    , string cancelText = "Cancel",
    UnityAction<int> onResult = null
    )
    {
        gameObject.SetActive(true);
        this.title.text = title;
        this.content.text = content;
        btnDone.GetComponentInChildren<Text>().text = okText;
        if (cancelText == null)
        {
            btnCancel.gameObject.SetActive(false);
        }
        else
        {
            btnCancel.gameObject.SetActive(true);
            btnCancel.GetComponentInChildren<Text>().text = cancelText;
        }
        OnSelectDone = onResult;
    }

    public void SelectDone()
    {
        gameObject.SetActive(false);
        OnSelectDone?.Invoke(1);
    }

    public void SelectCancel()
    {
        gameObject.SetActive(false);
        OnSelectDone?.Invoke(0);
    }

    public void SelectClose()
    {
        gameObject.SetActive(false);
        OnSelectDone?.Invoke(2);
    }
}
