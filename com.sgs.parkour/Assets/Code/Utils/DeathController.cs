using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeathController
{
    public event EventHandler OnClickRespawnCallback;

    [SerializeField] Animator displayAnimator;
    [SerializeField] Button respawnButton;

    public DeathController(DeathController deathController)
    {
        displayAnimator = deathController.displayAnimator;
        respawnButton = deathController.respawnButton;
        
        respawnButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        OnClickRespawnCallback?.Invoke(this, EventArgs.Empty);
    }

    public void Show()
    {
        displayAnimator.SetTrigger("Show");
    }

    public void Hide()
    {
        displayAnimator.SetTrigger("Hide");
    }
}