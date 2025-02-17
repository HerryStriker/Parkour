using System.Collections;
using UnityEngine;

public class WorldSpaceParticleManager : MonoBehaviour 
{
    ParticleSystem particle;
    const float byPassTime = .5f;
    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        StartCoroutine(SelfDestroy());
    }
    
    IEnumerator SelfDestroy()
    {
        yield return new WaitWhile(() => particle != null && particle.isPlaying);
        yield return new WaitForSecondsRealtime(byPassTime);
        Destroy(gameObject);
    }
}