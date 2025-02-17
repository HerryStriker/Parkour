using UnityEngine;


[System.Serializable]
public struct EffectClamp
{
    [Range(0,1)] public float Min;
    [Range(0,1)] public float Max;
    public float Value;

    public float Normalized {
        get {
            return Mathf.Lerp(Min , Max, Value);
        }
    }


}