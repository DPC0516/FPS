using UnityEngine;
using UnityEngine.UI;
using Varriables;
using UnityEngine.SceneManagement;
using Varriables.Setting;

public class JoinOrMakeServerPopup : MonoBehaviour
{
    [SerializeField]
    private InputField _roomName;
    [SerializeField]
    private InputField _name;
    [SerializeField]
    private Toggle isScore;
    [SerializeField]
    private Toggle isKill;
    [SerializeField]
    private Toggle isTime;
    [SerializeField]
    private InputField score;
    [SerializeField]
    private InputField kill;
    [SerializeField]
    private InputField time;
    [SerializeField]
    private InputField map;
    [SerializeField]
    private Toggle isDayNightSystem;
    [SerializeField]
    private Toggle isDay;
    [SerializeField]
    private InputField dayNightTime;

    [SerializeField]
    private Toggle isJoin;
    [SerializeField]
    private GameObject create;
    [SerializeField]
    private GameObject joinButton;
    [SerializeField]
    private GameObject createButton;

    void Start()
    {
        _roomName.text = Public.roomName;
        _name.text = Public.userName;
        kill.text = Public.gameMode.targetKill.ToString();
        score.text = Public.gameMode.targetScore.ToString();
        time.text = Public.gameMode.targetTime.ToString();
        isScore.isOn = Public.gameMode.isScore;
        isKill.isOn = Public.gameMode.isKill;
        isTime.isOn = Public.gameMode.isTime;
        isJoin.isOn = Public.isJoin;
        map.text = Public.gameMode.mapName;
        Public.DebugLog("start", Public.gameMode.isDayNightSystem.ToString(), null);
        Public.DebugLog("start", Public.gameMode.isDay.ToString(), null);
        Public.DebugLog("start", Public.gameMode.dayNightTime.ToString(), null);
        isDayNightSystem.isOn = Public.gameMode.isDayNightSystem;
        isDay.isOn = Public.gameMode.isDay;
        dayNightTime.text = Public.gameMode.dayNightTime.ToString();
    }

    private void Update()
    {
        create.SetActive(!isJoin.isOn);
        createButton.SetActive(!isJoin.isOn);
        joinButton.SetActive(isJoin.isOn);
    }

    public void OnJoinClick()
    {
        Public.isJoin = true;
        Public.roomName = _roomName.text;
        Public.userName = _name.text;
        Public.gameMode.targetKill = int.Parse(kill.text);
        Public.gameMode.targetScore = int.Parse(score.text);
        Public.gameMode.targetTime = int.Parse(time.text);
        Public.gameMode.isScore = isScore.isOn;
        Public.gameMode.isTime = isTime.isOn;
        Public.gameMode.isKill = isKill.isOn;
        Public.gameMode.mapName = map.text;
        Public.gameMode.dayNightTime = int.Parse(dayNightTime.text);
        Public.gameMode.isDayNightSystem = isDayNightSystem.isOn;
        Public.gameMode.isDay = isDay.isOn;
        FileLoader.SaveGameModeSetting(new GameModeSetting(Public.gameMode));
        LoadingSceneManager.LoadScene("Game");
    }

    public void OnCreateClick()
    {
        Public.isJoin = false;
        Public.roomName = _roomName.text;
        Public.userName = _name.text;
        Public.gameMode.targetKill = int.Parse(kill.text);
        Public.gameMode.targetScore = int.Parse(score.text);
        Public.gameMode.targetTime = int.Parse(time.text);
        Public.gameMode.isScore = isScore.isOn;
        Public.gameMode.isTime = isTime.isOn;
        Public.gameMode.isKill = isKill.isOn;
        Public.gameMode.mapName = map.text;
        Public.gameMode.dayNightTime = int.Parse(dayNightTime.text);
        Public.gameMode.isDayNightSystem = isDayNightSystem.isOn;
        Public.gameMode.isDay = isDay.isOn;
        FileLoader.SaveGameModeSetting(new GameModeSetting(Public.gameMode));
        LoadingSceneManager.LoadScene("Game");
    }

    public void OnCanceClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnValueChange(string changed)
    {
        if(changed == "Score")
        {
            isKill.isOn = !isScore.isOn;
        }
        else
        {
            isScore.isOn = !isKill.isOn;
        }
    }
}
