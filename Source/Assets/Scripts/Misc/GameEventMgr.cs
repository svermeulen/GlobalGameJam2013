using UnityEngine;
using System.Collections;

public enum GameEvents
{
    PlayerFinishedLevel,
    PlayerDied,
    PlayerHitMaxFear,
}

public class GameEventMgr : EventMgr<GameEvents>
{
    private static GameEventMgr _instance;

    public static GameEventMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventMgr();
            }

            return _instance;
        }
    }

    public GameEventMgr()
    {
        RegisterEvent<PlayerType>(GameEvents.PlayerFinishedLevel);
        RegisterEvent<PlayerType>(GameEvents.PlayerDied);
        RegisterEvent(GameEvents.PlayerHitMaxFear);
    }
}
