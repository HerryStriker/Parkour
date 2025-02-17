using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LoaderController
{
    public event EventHandler OnValueChangedCallback;
    public event EventHandler OnLoadedCallback;
    public event EventHandler OnUnloadedCallback;

    [SerializeField] protected GameObject gameObject;
    [SerializeField] protected Image value;

    public bool IsEnabled { get; private set; }

    public LoaderController(LoaderController loaderController)
    {
        gameObject = loaderController.gameObject;
        value  = loaderController.value;
        Disable();

        OnLoadedCallback += OnLoaded;
        OnUnloadedCallback += OnUnloaded;
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        Fire();
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        Fire();
    }

    public void Enable()
    {
        IsEnabled = true;
        OnLoadedCallback?.Invoke(this, EventArgs.Empty);
        // Debug.Log("Enable");
    }

    public void Disable()
    {
        IsEnabled = false;
        OnUnloadedCallback?.Invoke(this, EventArgs.Empty);
        // Debug.Log("Disable");
    }

    void Fire()
    {
        OnValueChangedCallback?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(IsEnabled);
    }

    public void Update(float normalizedTime)
    {
        if (IsEnabled)
        {
            value.fillAmount = normalizedTime;
        }
    }
}