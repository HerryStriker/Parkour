using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class TimeCounter
{
    [SerializeField] bool isCounting = false;
    [SerializeField] float h,m,s;
    [SerializeField] float incrementTime = 1f;
    const float MAX_TIME_UNITY = 59;
    public IEnumerator IncrementTimer()
    {
        do {
            s += incrementTime;

            if(s > MAX_TIME_UNITY)
            {
                s = 0;
                m += 1;

                if(m > MAX_TIME_UNITY)
                {
                    m = 0;
                    h += 1;

                    if(h > MAX_TIME_UNITY)
                    {
                        s = 0;
                        m = 0;
                        h = 0;
                        isCounting = false;
                    }
                }
            }
            yield return new WaitForSecondsRealtime(1f);
        }
        while (isCounting);
    }

    public void Start(MonoBehaviour monoBehaviour)
    {
        if(isCounting) return;

        isCounting = true;        
        monoBehaviour.StartCoroutine(IncrementTimer());
    }

    public void Reset(MonoBehaviour monoBehaviour)
    {
        Stop();
        Start(monoBehaviour);
    }

    public void Stop()
    {
        s = 0;
        m = 0;
        h = 0;
        isCounting = false;
    }
}
