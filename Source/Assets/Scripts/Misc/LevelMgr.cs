using UnityEngine;
using System.Collections;

public enum PlayerType
{
    Hansel,
    Gretel
}

public class LevelMgr : MonoBehaviour
{
    public float advanceLevelDelayWin;
    public float advanceLevelDelayLevelDeath;
    public float advanceLevelDelayFearDeath;

    private static LevelMgr _instance;
    private bool[] _playerDone = new bool[2];

    public static LevelMgr Instance
    {
        get
        {
            Util.Assert(_instance != null);
            return _instance;
        }
    }

    private int _currentLevel = -1;

    private float _advanceCountdown = 0;
    private string _nextLevelName;

    private string[] _levelSequence = new[]
    {
        "TutorialRoom_Cross",
        "StephenLevel2",
        "Room_DontLeave",
        "StephenLevel1",
		"DrawbridgeBoxes",
		"RyansHellLevel",
    };

    public void LoadLevel(int levelIndex)
    {
        _currentLevel = levelIndex;
        CameraFade.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel(_levelSequence[levelIndex]); });
    }

    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (Application.loadedLevelName != "MainMenu")
            {
                CameraFade.StartAlphaFade(Color.black, false, 2f, 0f, () => { Application.LoadLevel("MainMenu"); });
            }
        }
    }

	void Start()
	{
	    CameraFade.StartAlphaFade(Color.black, true, 3f, 0f);
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        for (var i = 0; i < _levelSequence.Length; i++)
        {
            if (_levelSequence[i] == Application.loadedLevelName)
            {
                _currentLevel = i;
            }
        }

	    _instance = this;

        Object.DontDestroyOnLoad(this);

        GameEventMgr.Instance.AddListener<PlayerType>(GameEvents.PlayerFinishedLevel, OnFinishedLevel);
        GameEventMgr.Instance.AddListener<PlayerType>(GameEvents.PlayerDied, OnPlayerDied);
        GameEventMgr.Instance.AddListener(GameEvents.PlayerHitMaxFear, OnPlayerHitMaxFear);
	}

    private void OnPlayerHitMaxFear()
    {
        _advanceCountdown = advanceLevelDelayFearDeath;
        _nextLevelName = _levelSequence[_currentLevel];
        CameraFade.StartAlphaFade(Color.black, false, _advanceCountdown, 2f, () => { Application.LoadLevel(_nextLevelName); });
    }

    private void OnPlayerDied(PlayerType param)
    {
        _advanceCountdown = advanceLevelDelayLevelDeath;
        _nextLevelName = _levelSequence[_currentLevel];

        CameraFade.StartAlphaFade(Color.black, false, _advanceCountdown, 2f, () => { Application.LoadLevel(_nextLevelName); });
    }

    void OnFinishedLevel(PlayerType playerType)
    {
        _playerDone[(int)playerType] = true;

        if (_playerDone[0] && _playerDone[1])
        {
            _advanceCountdown = advanceLevelDelayWin;
            _currentLevel = _currentLevel + 1;

            if (_currentLevel == _levelSequence.Length)
            {
                _nextLevelName = "Credits";
            }
            else
            {
                _nextLevelName = _levelSequence[_currentLevel];

                _playerDone[0] = false;
                _playerDone[1] = false;
            }

            CameraFade.StartAlphaFade(Color.black, false, _advanceCountdown, 0f, () => { Application.LoadLevel(_nextLevelName); });
        }
    }
}
