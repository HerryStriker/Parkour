using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

public class CheckPoint : MonoBehaviour, IEquatable<CheckPoint>
{
    BoxCollider _boxCollider;


    [SerializeField] string nam;
    [TextArea, SerializeField] string description;

    [field: SerializeField] public int FlagID { get; private set; }
    [field: SerializeField] public CheckPointType t_CheckPoint { get; private set; }
    
    [Tooltip("Constrains that controls the object collision range.")]
    [Header("Collision")]
    [SerializeField, Range(1,10)] float c_radius = 1f;
    
    [Tooltip("Constrains that controls the object spawn visibility range.")]
    [Header("Spawn")]

    /// <summary>
    /// Spawn radius
    /// </summary>
    [SerializeField, Range(1,10)] float s_radius = 1f;

    /// <summary>
    /// Spawn height
    /// </summary>
    [SerializeField, Range(0,5)] float s_heigth = 0;

    [Header("Effect")]
    [SerializeField] ParticleSystem idle;
    [SerializeField] ParticleSystem pickup;

    void Awake()
    {
        if(idle != null && pickup != null)
        {
            idle.Play();
            pickup.Stop();
        }
    }

    private void OnValidate()
    {   
        if(t_CheckPoint != CheckPointType.CHECK) return;

        if(_boxCollider == null)
        {
            _boxCollider = GetComponent<BoxCollider>();
        }
        else
        {
            Vector3 size = new Vector3(1,_boxCollider.size.y,1);
            size.x = c_radius;
            size.z = c_radius;
            _boxCollider.size = size;
        }
    }

    public void PlayPickupEffect()
    {
        if(pickup != null && !pickup.isPlaying)
        {
            pickup.Play();
            idle.Stop();
        }
    }

    public Vector3 GetRangeSpawn()
    {
        var range = transform.position + UnityEngine.Random.insideUnitSphere * s_radius;
        range.y = transform.position.y + Vector3.up.y * s_heigth;
        return range;
    }

    void OnDrawGizmos()
    {
        Color g_color = Color.red;
        g_color.a = .5f;
        Gizmos.color = g_color;
        Gizmos.DrawSphere(transform.position + Vector3.up * s_heigth, s_radius);
    }
    #region Utils
    public bool Equals(CheckPoint other)
    {
        return FlagID == other.FlagID;
    }

    public void SetFlagID(int id, CheckPointType type)
    {
        FlagID = id;
        t_CheckPoint = type;
    }

#endregion
    public enum CheckPointType
    {
        START,
        CHECK,
        END,
    }
}
