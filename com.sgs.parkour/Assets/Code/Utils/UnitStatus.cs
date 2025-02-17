using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStatus {

    public event EventHandler OnValueChangedCallback;
    public event EventHandler OnValueEqualToZero;
    public event EventHandler OnValueEqualToMax;
    public event EventHandler OnValueEqualToHalf;


    [field: SerializeField] public float CurrentValue { get; private set; }
    [field: SerializeField] public float MaxValue { get; private set; } = 100f;

    public float NormalizedValue {
        get {
            return CurrentValue / MaxValue;
        }
    }

    public bool IsZero {
        get { return CurrentValue == 0; }
    }

    public UnitStatus(float maxValue) {
        MaxValue = maxValue;
        CurrentValue = maxValue;
    }
    public UnitStatus(UnitStatus unitStatus) {
        MaxValue = unitStatus.MaxValue;
        CurrentValue = unitStatus.MaxValue;

        if (MaxValue <= 0 || unitStatus.MaxValue <= 0) {
            Debug.LogWarning("Unit life value is not a valid value, make sure you change before run it.");
        }
    }
    public UnitStatus(UnitStatus unitStatus, float maxValue) {
        unitStatus.MaxValue = maxValue;
        MaxValue = unitStatus.MaxValue;
        CurrentValue = unitStatus.MaxValue;
    }

    public void ChangeValue(float value) {
        var half = MaxValue / 2;

        CurrentValue = Mathf.Clamp(CurrentValue + value, 0, MaxValue);
        if (CurrentValue > 0 || CurrentValue < MaxValue) {
            OnValueChangedCallback?.Invoke(value, new EventArgs());
            //Debug.Log("ChangeValue " + CurrentValue + " ," + value);
        }

        if (CurrentValue < half) {
            OnValueEqualToHalf?.Invoke(value, new EventArgs());
        }

        if (CurrentValue == MaxValue) {
            OnValueEqualToMax?.Invoke(value, new EventArgs());
        }

        if (CurrentValue == 0) {
            OnValueEqualToZero?.Invoke(value, new EventArgs());
        }
    }

    public void ClearValue() {
        ChangeValue(-CurrentValue);
        OnValueEqualToZero?.Invoke(this, new EventArgs());
    }

    public void ResetValue() {
        if (CurrentValue < MaxValue) {
            ChangeValue(MaxValue);
        }
    }
}
