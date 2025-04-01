using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EffectMap 
{
    public MovementEffectType movementEffectType;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] EffectClamp effectClamp;

    Transform ParticleGroupTransform {
        get {
            return particleSystem != null ? particleSystem.transform : null;
        }
    }

    ParticleSystem CreateOnWorldSpace(Vector3 position)
    {
        var particle = UnityEngine.Object.Instantiate(particleSystem, position, Quaternion.identity);
        particle.AddComponent<WorldSpaceParticleManager>();
        return particle;
    }

    [SerializeField] bool modifyScale = false;
    [SerializeField] bool createOnActivate = false;
    public float Normalized {get; set;}
    
    /// <summary>
    /// World space position where the object will be created. 
    /// </summary>
    /// <param name="position"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Start(Vector3 position = default)
    {
        if(!particleSystem.isPlaying)
        {
            var scale = !modifyScale ? Vector3.one : Vector3.one * effectClamp.Max;
            if(createOnActivate)
            {
                if(position == default)
                {
                    Debug.LogError("You must usa a world space position if {createOnActivate} is true.");
                    throw new NotImplementedException();
                }

                var particle = CreateOnWorldSpace(position);
                particle.transform.localScale = scale;
                particle.Play();
            }
            else
            {
                particleSystem.Play();
                ParticleGroupTransform.localScale = scale;
            }
        }
    }

    public void Update(float normalized)
    {
        effectClamp.Value = normalized;
        ParticleGroupTransform.localScale = !modifyScale ? Vector3.one : Vector3.one * effectClamp.Normalized;
    }

    public void Stop()
    {
        particleSystem.Stop();
    }

    public void Reset()
    {
        if(particleSystem.isPlaying)
        {
            particleSystem.Stop();
            Start();
            ParticleGroupTransform.localScale = !modifyScale ? Vector3.one : Vector3.one * effectClamp.Max;
        }
    }
}
public enum MovementEffectType
{
    NONE,
    LANDING,
    JUMPING,
    LOADING,
    FULL_LOADED
}