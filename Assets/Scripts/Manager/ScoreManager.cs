using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{
    private List<string> playerPhotonViewID = new List<string>();
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();
    private Dictionary<string, int> playerDeaths = new Dictionary<string, int>();
    private Dictionary<string, int> playerKills = new Dictionary<string, int>();
    private Dictionary<string, string> playerNames = new Dictionary<string, string>();

    [SerializeField]
    private ScrollRect playerScoreView;

    [SerializeField]
    private GameObject playerScoreViewGameObject;

    [SerializeField]
    private ScrollRect playerScoreViewSmall;

    [SerializeField]
    private GameObject playerScoreViewSmallGameObject;

    [SerializeField]
    private Text timer;

    [SerializeField]
    private GameObject ScoreViewPrefab;
    [SerializeField]
    private GameObject ScoreViewSmallPrefab;

    private List<RectTransform> playerScoreViews = new List<RectTransform>();
    private List<RectTransform> playerScoreViewSmalls = new List<RectTransform>();

    private const float space = 15f;
    private const float spaceSmall = 10f;

    private bool isEnd = false;

    [SerializeField]
    private GameObject TextUI_timer;

    private void Start()
    {
        Public.score = 0;
        Public.death = 0;
        Public.kill = 0;
        isEnd = false;
    }

    void Update()
    {
        TextUI_timer.SetActive(Public.gameMode.isTime);
        if (Public.gameMode.isTime)
        {
            float second = Public.gameMode.targetTime - Public.currentTime;
            int minute = (int)second / 60;
            second = second % 60;
            int hour = minute / 60;
            minute = minute % 60;

            timer.text = getStringTime(hour) + ":" + getStringTime(minute) + ":" + getStringTime((int)second);
        }
        if (Public.isGameStarted)
        {
            if (!isEnd)
            {
                if (Public.gameMode.isScore)
                {
                    for (int i = 0; i < playerPhotonViewID.Count; i++)
                    {
                        if (playerScores[playerPhotonViewID[i]] >= Public.gameMode.targetScore)
                        {
                            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().winner(playerNames[playerPhotonViewID[i]]);
                            isEnd = true;
                            return;
                        }
                    }
                }
                if (Public.gameMode.isKill)
                {
                    for (int i = 0; i < playerPhotonViewID.Count; i++)
                    {
                        if (playerKills[playerPhotonViewID[i]] >= Public.gameMode.targetKill)
                        {
                            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().winner(playerNames[playerPhotonViewID[i]]);
                            isEnd = true;
                            return;
                        }
                    }
                }
                if (Public.gameMode.isTime)
                {
                    if (Public.gameMode.targetTime - Public.currentTime < 0)
                    {
                        if (Public.gameMode.isKill)
                        {
                            string winnerName = "None";
                            int winnerKill = 0;
                            for (int i = 0; i < playerPhotonViewID.Count; i++)
                            {
                                if (playerKills[playerPhotonViewID[i]] > winnerKill)
                                {
                                    winnerName = playerNames[playerPhotonViewID[i]];
                                    winnerKill = playerKills[playerPhotonViewID[i]];
                                }
                            }
                            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().winner(winnerName);
                            isEnd = true;
                            return;
                        }
                        if (Public.gameMode.isScore)
                        {
                            string winnerName = "None";
                            int winnerScore = 0;
                            for (int i = 0; i < playerPhotonViewID.Count; i++)
                            {
                                if (playerScores[playerPhotonViewID[i]] > winnerScore)
                                {
                                    winnerName = playerNames[playerPhotonViewID[i]];
                                    winnerScore = playerScores[playerPhotonViewID[i]];
                                }
                            }
                            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().winner(winnerName);
                            isEnd = true;
                            return;
                        }
                    }
                }
            }
        }
        if (Input.GetKey(Key.ScoreTab))
        {
            playerScoreViewSmallGameObject.SetActive(false);
            playerScoreViewGameObject.SetActive(true);
        }
        else
        {
            playerScoreViewSmallGameObject.SetActive(true);
            playerScoreViewGameObject.SetActive(false);
        }
    }

    private string getStringTime(int time)
    {
        if(time > 0)
        {
            if(time >= 10)
            {

                return time.ToString();
            }
            else
            {
                return "0" + time.ToString();
            }
        }
        else{
            return "00";
        }
    }

    public void addPlayer(string id, string name, int score, int death, int kill)
    {
        playerPhotonViewID.Add(id);
        playerScores.Add(id, score);
        playerKills.Add(id, kill);
        playerDeaths.Add(id, death);
        playerNames.Add(id, name);
        RectTransform newPlayerScoreView = Instantiate(ScoreViewPrefab, playerScoreView.content).GetComponent<RectTransform>();
        RectTransform newPlayerSocreViewSmall = Instantiate(ScoreViewSmallPrefab, playerScoreViewSmall.content).GetComponent<RectTransform>();
        playerScoreViews.Add(newPlayerScoreView);
        playerScoreViewSmalls.Add(newPlayerSocreViewSmall);

        float y = 0f;
        for(int i = 0; i < playerScoreViews.Count; i++)
        {
            playerScoreViews[i].anchoredPosition = new Vector2(0f, -y);
            y += playerScoreViews[i].sizeDelta.y + space;
        }
        playerScoreView.content.sizeDelta = new Vector2(playerScoreView.content.sizeDelta.x, y);

        y = 0f;
        for (int i = 0; i < playerScoreViewSmalls.Count; i++)
        {
            playerScoreViewSmalls[i].anchoredPosition = new Vector2(0f, -y);
            y += playerScoreViewSmalls[i].sizeDelta.y + spaceSmall;
        }
        playerScoreViewSmall.content.sizeDelta = new Vector2(playerScoreViewSmall.content.sizeDelta.x, y);
    }

    public void removePlayer(string id)
    {
        try
        {
            Destroy(playerScoreViews[playerPhotonViewID.IndexOf(id)].gameObject);
            Destroy(playerScoreViewSmalls[playerPhotonViewID.IndexOf(id)].gameObject);
            playerScoreViews.Remove(playerScoreViews[playerPhotonViewID.IndexOf(id)]);
            playerScoreViewSmalls.Remove(playerScoreViews[playerPhotonViewID.IndexOf(id)]);

            float y = 0f;
            for (int i = 0; i < playerScoreViews.Count; i++)
            {
                playerScoreViews[i].anchoredPosition = new Vector2(0f, -y);
                y += playerScoreViews[i].sizeDelta.y + space;
            }
            playerScoreView.content.sizeDelta = new Vector2(playerScoreView.content.sizeDelta.x, y);

            y = 0f;
            for (int i = 0; i < playerScoreViewSmalls.Count; i++)
            {
                playerScoreViewSmalls[i].anchoredPosition = new Vector2(0f, -y);
                y += playerScoreViewSmalls[i].sizeDelta.y + spaceSmall;
            }
            playerScoreViewSmall.content.sizeDelta = new Vector2(playerScoreViewSmall.content.sizeDelta.x, y);

            playerScores.Remove(id);
            playerNames.Remove(id);
            playerKills.Remove(id);
            playerDeaths.Remove(id);
            playerPhotonViewID.RemoveAt(playerPhotonViewID.IndexOf(id));
        }
        catch { return; }
    }

    public void setPlayerScore(string id, int score, int death, int kill)
    {
        playerScores[id] = score;
        playerKills[id] = kill;
        playerDeaths[id] = death;
        playerScoreViews[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[0].text = playerNames[id];
        playerScoreViews[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[1].text = score.ToString();
        playerScoreViews[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[2].text = kill.ToString();
        playerScoreViews[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[3].text = death.ToString();


        playerScoreViewSmalls[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[0].text = playerNames[id];
        if (Public.gameMode.isScore)
        {
            playerScoreViewSmalls[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[1].text = score.ToString();
        }
        else
        {
            playerScoreViewSmalls[playerPhotonViewID.IndexOf(id)].gameObject.GetComponentsInChildren<Text>()[1].text = kill.ToString();
        }
    }
}
