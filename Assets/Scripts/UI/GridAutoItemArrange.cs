using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridAutoItemArrange : MonoBehaviour
{
    public GridLayoutGroup layout;

    public GameObject backgroundItemPrefab;
    public GameObject panelBounds;

    public float minWidthCell = 60f;
    public List<GameObject> items = new List<GameObject>();
    private int minNumRows = 0;
    private int cellsInRow = 0;
    private float cellWidth = 0f;
    private bool isInitBackground = false;

    private float lastWidth = -1f;
    private GameObject tempHolder;

    private void Awake()
    {
        Prepare();
    }

    private void Prepare()
    {
        layout = transform.GetComponent<GridLayoutGroup>();
        DisplayBackgroundCell();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (panelBounds.GetComponent<RectTransform>().rect.width != lastWidth)
        {
            CalculateBackgroundRequired();
            AdjustMatchItems();
        }
    }

    public void DisplayBackgroundCell()
    {
        if (layout != null)
        {
            CalculateBackgroundRequired();

            layout.cellSize = new Vector2(cellWidth, cellWidth);

            for (int r = 0; r < minNumRows; r++)
            {
                for (int i = 0; i < cellsInRow; i++)
                {
                    GameObject newCell = Instantiate(backgroundItemPrefab, transform, false);
                }
            }
            AdjustMatchItems();
            if (tempHolder != null)
            {
                tempHolder.transform.SetAsLastSibling();
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].transform.SetParent(GetHolderItem(i, true), false);
                }
                Debug.Log("Destroy tempHolder");
                Destroy(tempHolder);
                tempHolder = null;
            }
            isInitBackground = true;
        }
    }


    private void OnEnable()
    {

        if (!isInitBackground || lastWidth == -1)
        {
            Prepare();
        }


    }

    private void OnWillRenderObject()
    {

    }

    private void CalculateBackgroundRequired()
    {
        lastWidth = panelBounds.GetComponent<RectTransform>().rect.width;

        float width = panelBounds.GetComponent<RectTransform>().rect.width - layout.padding.left - layout.padding.right;
        float height = panelBounds.GetComponent<RectTransform>().rect.height - layout.padding.top - layout.padding.bottom;
        int numberItemRow = (int)(width / minWidthCell);
        if (numberItemRow <= 0) return;
        float expectWidth = width / numberItemRow;
        int numberRow = Mathf.CeilToInt(height / expectWidth);
        Debug.Log("calculate grid layout " + width + "/" + minWidthCell + "/" + numberItemRow + "/" + numberRow);
        numberRow++;
        //save calculate
        cellWidth = expectWidth;
        minNumRows = numberRow;
        cellsInRow = numberItemRow;
    }

    private int GetCurrentTotalChild()
    {
        if (tempHolder != null) return transform.childCount - 1;
        else return transform.childCount;
    }

    public void AdjustMatchItems()
    {
        Debug.Log("AdjustMatchItems " + cellsInRow);
        int rowForItem = Mathf.CeilToInt(items.Count / (float)cellsInRow);
        Debug.Log("AdjustMatchItems rowForItem " + rowForItem + "/" + items.Count);
        int totalRows = Mathf.CeilToInt(GetCurrentTotalChild() / (float)cellsInRow);
        rowForItem = rowForItem > minNumRows ? rowForItem : minNumRows;

        int changeRow = rowForItem - totalRows;
        int changeCell = rowForItem * cellsInRow - GetCurrentTotalChild();
        Debug.Log("AdjustMatchItems changeRow " + changeRow + "/" + GetCurrentTotalChild() + "/" + transform.childCount);

        if (changeCell > 0)
        {
            for (int r = 0; r < changeCell; r++)
            {
                GameObject newCell = Instantiate(backgroundItemPrefab, transform, false);
            }
        }
        else if (changeCell < 0)
        {

            Debug.Log("destroy row");
            for (int i = GetCurrentTotalChild(); i > (rowForItem * cellsInRow); i--)
            {
                DestroyImmediate(transform.GetChild(i - 1).gameObject);
            }
        }
    }

    private void DisplayItems()
    {
        if (items.Count == 0) return;
        for (int i = 0; i < items.Count; i++)
        {
            transform.SetParent(transform.GetChild(i).transform, false);
        }
    }

    private void ClearAll()
    {
        MyGameObjectUtils.ClearAllChilds(gameObject);
    }

    public void OnRectTransformDimensionsChange()
    {
    }

    public int AddItem(GameObject item)
    {
        items.Add(item);
        int index = items.Count - 1;
        item.transform.SetParent(GetHolderItem(index), false);
        return index;
    }

    public void AddItems(GameObject[] addItems)
    {
        int beforeAddCount = items.Count;
        items.AddRange(addItems);
        for (int i = 0; i < addItems.Length; i++)
        {
            addItems[i].transform.SetParent(GetHolderItem(beforeAddCount + i), false);
        }
    }

    public void RemoveItem(int index)
    {
        items.RemoveAt(index);
        MyGameObjectUtils.ClearAllChilds(GetHolderItem(index));
        for (int i = index; i < items.Count; i++)
        {
            items[i].transform.SetParent(GetHolderItem(index), false);
        }
    }

    internal GameObject AddItemPrefab(GameObject prefabItem)
    {
        items.Add(prefabItem);
        int index = items.Count - 1;
        GameObject newItem = Instantiate(prefabItem, GetHolderItem(index), false);
        items[index] = newItem;
        return newItem;
    }

    private Transform GetHolderItem(int index, bool forceNonTemp = false)
    {
        if (isInitBackground || forceNonTemp)
        {
            AdjustMatchItems();
            Debug.Log("GetHolderItem " + GetCurrentTotalChild() + "/" + index);
            return transform.GetChild(index).transform;
        }
        else
        {
            if (tempHolder == null)
            {
                tempHolder = new GameObject();
                tempHolder.SetActive(false);
                tempHolder.transform.SetParent(transform, false);
            }
            if (index >= tempHolder.transform.childCount)
            {
                for (int i = tempHolder.transform.childCount; i <= index; i++)
                {
                    Instantiate(backgroundItemPrefab, tempHolder.transform, false);
                }
            }
            return tempHolder.transform.GetChild(index);
        }
    }

    internal void ClearItems()
    {
        Debug.Log("Clear Items");
        for (int i = items.Count; i > 0; i--)
        {
            Destroy(items[i - 1].gameObject);
        }
        items.Clear();
    }
}
