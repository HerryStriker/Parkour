using TMPro;
using UnityEngine;

[System.Serializable]
public class DoubleJumpCounter
{
    [SerializeField] protected GameObject gameObject;
    [SerializeField] protected TextMeshProUGUI value;

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