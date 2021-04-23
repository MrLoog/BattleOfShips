using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameLevelInfo : MonoBehaviour
{
    public ScriptableGameLevel level;
    public Text content;
    public Text title;

    public UnityAction OnSelect;
    public UnityAction OnCancel;

    private const string TEMPLATE_LEVEL_INFO = "{0}";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressSelect()
    {
        gameObject.SetActive(false);
        OnSelect?.Invoke();
    }
    public void PressClose()
    {
        gameObject.SetActive(false);
        OnCancel?.Invoke();
    }

    internal void ShowPreview(ScriptableGameLevel level)
    {
        this.level = level;
        ShowLevelInfo();
        gameObject.SetActive(true);
    }

    private void ShowLevelInfo()
    {
        title.text = level.levelName;
        content.text = string.Format(TEMPLATE_LEVEL_INFO
        , level.description
        );
    }
}
