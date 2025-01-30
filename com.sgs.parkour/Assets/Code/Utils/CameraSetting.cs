using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class InputSetting
{
    public Vector3 Offset;
    public Bool3 InverterOffset;
    [SerializeField] Vector2 m_SmoothVelocity = Vector2.zero;
    [SerializeField, Range(0, 1)] float m_SmoothTime = 0.01f;

    public Vector3 GetSettings(Vector3 value)
    {
        return new Vector3(InvertFloat(value.x, InverterOffset.x), InvertFloat(value.y, InverterOffset.y), InvertFloat(value.z, InverterOffset.z));
    }
    public Vector2 GetSettings(Vector2 value)
    {
        return new Vector2(InvertFloat(value.x, InverterOffset.x), InvertFloat(value.y, InverterOffset.y));
    }
    public Vector2 GetSettings(float x, float y)
    {
        return new Vector2(InvertFloat(x, InverterOffset.x), InvertFloat(y, InverterOffset.y));
    }

    public Vector2 GetSmoothDampSettings(float x, float y, ref Vector2 valueRef)
    {
        return Vector2.SmoothDamp(valueRef, new Vector2(x, y), ref m_SmoothVelocity, m_SmoothTime);
    }
    public Vector2 GetSmoothDampSettings(Vector2 inputValue, ref Vector2 valueRef)
    {
        return Vector2.SmoothDamp(valueRef, inputValue, ref m_SmoothVelocity, m_SmoothTime);
    }

    private float InvertFloat(float value, bool isInverted)
    {
        return isInverted ? value : -value;
    }

    public void SetSmoothTime(float time)
    {
        m_SmoothTime = time;
    }
}

[System.Serializable]
public struct Bool3
{
    public bool x, y, z;

    public Bool3(bool x, bool y, bool z)
    {
        this.x = x; this.y = y; this.z = z;
    }

}
