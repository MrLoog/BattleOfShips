using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelDisplay : MonoBehaviour
{
    public GameObject templateRow;
    public GameObject templateCell;

    public GameObject content;

    private ScriptableGameLevel selectedLevel;

    public GameLevelInfo levelPreview;
    public Dictionary<string, ScriptableGameLevel> dictGameLevel = new Dictionary<string, ScriptableGameLevel>();
    public ScriptableGameLevel firstLevel;
    // Start is called before the first frame update
    void Start()
    {
        PrepareLevelShow();
        levelPreview.OnSelect = PlayLevelSelect;
        levelPreview.OnCancel = DeSelectLevel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayLevelSelect()
    {
        TownManager.Instance.PlayLevel(selectedLevel);
    }

    public void DeSelectLevel()
    {
        selectedLevel = null;
    }
    public void PrepareLevelShow()
    {
        MyGameObjectUtils.ClearAllChilds(content);
        dictGameLevel.Clear();
        ShowLevel(firstLevel);
    }

    public void ShowLevel(ScriptableGameLevel level, GameObject prevRow = null, ScriptableGameLevel prevLevel = null)
    {
        if (dictGameLevel.ContainsKey(level.name))
        {
            Debug.Assert(!dictGameLevel.ContainsKey(level.name), "Sth Wrong with sub level repeat level reference");
            return;
        }
        else
        {
            dictGameLevel.Add(level.name, level);
        }
        GameObject newRow = prevRow;
        if (level.isMainLevel)
        {
            newRow = Instantiate(templateRow, content.transform, false);

        }
        else if (newRow == null)
        {
            Debug.Assert(newRow != null, "Sth Wrong with sub level");
            return;
        }
        GameObject newCell = Instantiate(templateCell, newRow.transform, false);
        newCell.GetComponentInChildren<Text>().text = level.shortName;
        Button btn = newCell.GetComponentInChildren<Button>();
        btn.onClick.AddListener(() =>
        {
            SelectLevel(level);
        });
        if (!GameManager.Instance.GameData.IsLevelCleared(level.codeName)
        && prevLevel != null && !GameManager.Instance.GameData.IsLevelCleared(prevLevel.codeName))
        {
            btn.interactable = false;
        }
        if (level.nextLevel != null)
        {
            foreach (ScriptableGameLevel next in level.nextLevel)
            {
                if (next != null)
                {
                    ShowLevel(next, newRow, level);
                }
            }
        }
    }

    public void SelectLevel(ScriptableGameLevel level)
    {
        Debug.Log("Select Level " + level.shortName);
        selectedLevel = level;
        levelPreview.ShowPreview(level);
    }

    public void CloseSelectPanel()
    {
        levelPreview?.PressClose();
        gameObject.SetActive(false);
    }
}
