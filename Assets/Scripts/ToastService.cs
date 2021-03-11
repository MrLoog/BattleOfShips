using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastService : MonoBehaviour
{
    public static ToastService Instance;

    public Text txtMessage;
    public float defaultDelayTime = 3f;
    public bool IsShowing = false;
    public Queue<string> queueMes = new Queue<string>();
    public Queue<float> queueTime = new Queue<float>();

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

    public void DequeueMessage()
    {
        string next = queueMes.Dequeue();
        float time = queueTime.Dequeue();
        ShowMessage(next, time, true);
    }

    public void ShowMessage(string message, float existTime = -1f, bool queue = false)
    {
        if (existTime < 0)
        {
            existTime = defaultDelayTime;
        }
        if (queue || (queueMes.Count == 0 && !IsShowing))
        {
            IsShowing = true;
            txtMessage.text = message;
            gameObject.SetActive(true);
            StartCoroutine(HideMessage(existTime));
        }
        else
        {
            queueMes.Enqueue(message);
            queueTime.Enqueue(existTime);
        }
    }

    public IEnumerator HideMessage(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (queueMes.Count > 0)
        {
            DequeueMessage();
        }
        else
        {
            txtMessage.text = "";
            gameObject.SetActive(false);
            IsShowing = false;
        }
    }
}
