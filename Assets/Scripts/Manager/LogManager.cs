using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [SerializeField]
    private GameObject logPrefab;

    [HideInInspector]
    public Dictionary<int, RectTransform> logs = new Dictionary<int, RectTransform>();
    [HideInInspector]
    public List<int> ids = new List<int>();

    [SerializeField]
    private ScrollRect ScrollView;

    private const float space = 5f;

    int count = 0;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Public.logManager = this;
    }

    public void AddLog(string message)
    {        
        RectTransform newLog = Instantiate(logPrefab, ScrollView.content).GetComponent<RectTransform>();
        logs.Add(count, newLog);
        ids.Add(count);
        newLog.GetComponent<Log>().logManager = this;
        newLog.GetComponent<Text>().text = message;
        newLog.GetComponent<Log>().id = count;
        count++;

        float y = 0;
        for (int i = 0; i < logs.Count; i++)
        {
            logs[ids[i]].anchoredPosition = new Vector2(0f, y);
            y += logs[ids[i]].sizeDelta.y + space;
        }
        ScrollView.content.sizeDelta = new Vector2(ScrollView.content.sizeDelta.x, y);
    }

    public void RemoveLog(int id)
    {
        logs.Remove(id);
        ids.Remove(id);

        float y = 0f;
        for (int i = 0; i < logs.Count; i++)
        {
            logs[ids[i]].anchoredPosition = new Vector2(0f, y);
            y += logs[ids[i]].sizeDelta.y + space;
        }
        ScrollView.content.sizeDelta = new Vector2(ScrollView.content.sizeDelta.x, y);
    }
}
