using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CollisionCheck
{
    public event EventHandler OnEnter;
    public event EventHandler OnExit;

    public bool IsColliding { get; private set; } = false;

    [SerializeField] bool inversedDirection = false;
    [SerializeField] CheckDirection direction = CheckDirection.CENTER;
    [SerializeField] CheckType type = CheckType.BOX;
    [SerializeField] float range = 1f;
    [SerializeField] float height = 1f;
    [SerializeField] float radius = 0.1f;
    [SerializeField] LayerMask mask;

    readonly Transform transform;
    public CollisionCheck(CollisionCheck collisionCheck, Transform transform)
    {
        this.transform = transform;

        this.inversedDirection = collisionCheck.inversedDirection;
        this.direction = collisionCheck.direction;
        this.type = collisionCheck.type;
        this.range = collisionCheck.range;
        this.radius = collisionCheck.radius;
        this.mask = collisionCheck.mask;
        this.height = collisionCheck.height;
        this.enableGizmos = collisionCheck.enableGizmos;
        
        if(transform.TryGetComponent(out MonoBehaviour monoBehaviour))
        {
            monoBehaviour.StartCoroutine(UpdateCheck());
        }
    }
    
    IEnumerator UpdateCheck()
    {
        while(true)
        {
            bool isColliding = false;
            switch (type)
            {
                case CheckType.BOX:
                isColliding = Physics.CheckBox(Position(transform), Vector3.one / 2 * radius, Quaternion.identity , mask);
                break;
                case CheckType.CAPSULE:
                isColliding = Physics.CheckCapsule(transform.position + Vector3.up * range, transform.position - Vector3.up * range , radius , mask); 
                break;
                case CheckType.SPHERE:
                isColliding = Physics.CheckSphere(Position(transform) , radius, mask);
                break;
            }

            if (isColliding != IsColliding)
            {
                if(isColliding)
                {
                    OnEnter?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    OnExit?.Invoke(this, EventArgs.Empty);
                }
                IsColliding = isColliding;
            }
            yield return new WaitForSecondsRealtime(0f);
        }
    }

    Vector3 Position(Transform transform)
    {
        var positionHeight = transform.position + Vector3.up * height;

        switch (direction)
        {
            default:
            case CheckDirection.CENTER: return positionHeight;
            case CheckDirection.FORWARD: return positionHeight + Inverse(transform.forward);
            case CheckDirection.UP: return positionHeight + Inverse(transform.up);
            case CheckDirection.RIGHT: return positionHeight + Inverse(transform.right);
        }

        Vector3 Inverse(Vector3 direction)
        {
            Vector3 dir = inversedDirection ? -direction : direction;
            return dir * range;
        }
    }
    
    [Space(10)]
    [SerializeField] bool enableGizmos = true;    
    public void DrawGizmos()
    {
        if(!enableGizmos || transform == null) return;

        var color = IsColliding ? Color.green : Color.red;
        Gizmos.color = color;

        switch (type)
        {
                case CheckType.BOX:
                Gizmos.DrawCube(Position(transform), Vector3.one * radius);
                break;
                case CheckType.CAPSULE:
                //Physics.CheckCapsule(transform.position + Vector3.up * range, transform.position - Vector3.up * range , radius , mask); 
                break;
                case CheckType.SPHERE:
                Gizmos.DrawSphere(Position(transform) , radius);
                break;
        }
    }

    public enum CheckDirection
    {
        CENTER,
        FORWARD,
        RIGHT,
        UP,
    }

    public enum CheckType 
    {
        BOX,
        CAPSULE,
        SPHERE,
    }
}

