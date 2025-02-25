using Parkour.State;
using Unity.Mathematics;
using UnityEngine;

public class ModeManager : MonoBehaviour
{   
    public delegate void ModeHandler(ModeType type);
    public ModeHandler OnModeStartsCallback;
    public static ModeManager Instance;

    [SerializeField] StateObject<ModeType> _mode;
    [SerializeField] TimeCounter timeCounter;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _mode = new StateObject<ModeType>(ModeType.NONE);
        timeCounter = new TimeCounter();
    }

    public void StartGame()
    {
        timeCounter.Start(this);
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