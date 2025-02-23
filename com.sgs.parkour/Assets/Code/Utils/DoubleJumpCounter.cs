using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DoubleJumpCounter
{
    [SerializeField] protected GameObject gameObject;
    [SerializeField] protected Text value;

    public DoubleJumpCounter(DoubleJumpCounter doubleJumpCounter, int initialValue)
    {
        gameObject = doubleJumpCounter.gameObject;
        value = doubleJumpCounter.value;

        ChangeValue(initialValue);
    }

    public void ChangeValue(string strValue)
    {
        value.text = strValue;
    }

    public void ChangeValue(int intValue)
    {
        value.text = intValue.ToString();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}