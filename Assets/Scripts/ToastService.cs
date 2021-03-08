using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastService : MonoBehaviour
{
    public static ToastService Instance;

    public Text txtMessage;
    public float defaultDelayTime = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowMessage(string message, float existTime = -1f)
    {
        if (existTime < 0)
        {
            existTime = defaultDelayTime;
        }
        txtMessage.text = message;
        gameObject.SetActive(true);
        StartCoroutine(HideMessage(existTime));
    }

    public IEnumerator HideMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        txtMessage.text = "";
        gameObject.SetActive(false);
    }
}
