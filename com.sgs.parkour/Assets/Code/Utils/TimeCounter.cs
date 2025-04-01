using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class TimeCounter
{
    public delegate void TimerDelegate(TimerState state);
    public event TimerDelegate OnTimerStateChanged;
    public event TimerDelegate OnTimerStateStart;
    public event TimerDelegate OnTimerStateStopped;
    public event TimerDelegate OnTimerStatePaused;
    public event TimerDelegate OnTimerStateEnd;
    public event TimerDelegate OnTimerStateHalf;

    [SerializeField] TimerState state;
    [SerializeField] float t;
    float maxTime = 0;
    readonly MonoBehaviour behaviour;
    public TimeCounter(MonoBehaviour monoBehaviour)
    {
        behaviour = monoBehaviour;
    }

    public void Start(float t)
    {
        maxTime = t;
        ChangeState(TimerState.RUNNING);

        behaviour.StartCoroutine(StartCoroutine());
        OnTimerStateStart?.Invoke(TimerState.RUNNING);
    }

    IEnumerator StartCoroutine()
    {
        do
        {
            t += 1;

            if(t == maxTime)
            {
                ChangeState(TimerState.STOPPED);
                OnTimerStateEnd?.Invoke(TimerState.STOPPED);
            }
            
            if(t == maxTime / 2)
            {
                OnTimerStateHalf?.Invoke(TimerState.STOPPED);
            }

            yield return new WaitForSecondsRealtime(1f);
        } while (state == TimerState.RUNNING);
    }

    public void Pause()
    {
        ChangeState(Switch(state));
        OnTimerStatePaused?.Invoke(state);

        if(state == TimerState.RUNNING)
        {
            behaviour.StartCoroutine(StartCoroutine());
        }
    }

    public void Stop()
    {
        ChangeState(TimerState.STOPPED);
        OnTimerStateStopped?.Invoke(state);
    }
    
    TimerState Switch( TimerState state)
    {
        switch (state)
        {
            default:
            case TimerState.PAUSED: return TimerState.RUNNING;
            case TimerState.RUNNING: return TimerState.PAUSED;
        }
    }

    public void ChangeState(TimerState state)
    {
        this.state = state;
        OnTimerStateChanged?.Invoke(state);
    }
}
    public enum TimerState
    {
        RUNNING,
        PAUSED,
        STOPPED,
    }
