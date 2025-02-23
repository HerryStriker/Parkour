using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class LifeController
{
    [SerializeField] GameObject gameObject;
    [SerializeField] Image value;
    [SerializeField] Image value_remain;
    [SerializeField] Gradient gradient;

    public LifeController(LifeController lifeController)
    {
        gameObject = lifeController.gameObject;
        gradient = lifeController.gradient;
        value = lifeController.value;
        value_remain = lifeController.value_remain;

        Debug.Log( gradient.colorKeys.Length);
    }


    float time;
    [SerializeField] float delayTime = 2f;

    public void Update(float normalizedValue)
    {
        var colorNormalized = Mathf.Lerp(gradient.colorKeys[0].time, gradient.colorKeys[^1].time, normalizedValue);

        // APPLY TO A MAIN SLIDE COLOR
        value.color = gradient.Evaluate(colorNormalized);
        value.fillAmount = normalizedValue;
        
        if(value.fillAmount != value_remain.fillAmount)
        {
            time += Time.deltaTime;
            float t = time / delayTime;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            value_remain.fillAmount = Mathf.Lerp(value_remain.fillAmount, value.fillAmount, t);
        }
        else
        {
            time = 0f;
        }

    }
}