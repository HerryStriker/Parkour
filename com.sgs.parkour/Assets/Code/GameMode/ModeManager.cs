using Parkour.State;
using Unity.Mathematics;
using UnityEngine;

public class ModeManager : MonoBehaviour
{   
    public delegate void ModeHandler(ModeType type);
    public ModeHandler OnModeStartsCallback;
    public ModeHandler OnModeEndsCallback;
    public static ModeManager Instance { get; private set; }
    public static MatchScoreManager MatchScoreInstance { get; private set; }
    
    
    [SerializeField] StateObject<ModeType> _mode;
    [SerializeField] TimeCounter timeCounter;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _mode = new StateObject<ModeType>(ModeType.NONE);
        timeCounter = new TimeCounter(this);

        StartGame(30);
    }

    public void StartGame()
    {
        timeCounter.Start(180);
        OnModeStartsCallback?.Invoke(_mode.State);
    }

    public void StartGame(float t)
    {
        timeCounter.Start(t);    
        OnModeStartsCallback?.Invoke(_mode.State);
    }
    
    public StateObject<ModeType> Mode => _mode;
}
public enum ModeType
{
    NONE,
    PLATFORM2D,
    RUSH,
    INTOTOP
}