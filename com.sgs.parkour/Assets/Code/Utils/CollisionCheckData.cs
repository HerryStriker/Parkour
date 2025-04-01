using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CollisionCheckData<T> : CollisionCheck 
{
    [SerializeField] bool useRaycastAll = false;
    [SerializeField] T data;

    public CollisionCheckData(CollisionCheckData<T> collisionCheckData, CollisionCheck collisionCheck, Transform transform) : base(collisionCheck, transform)
    {
        this.transform = transform;
        useRaycastAll = collisionCheckData.useRaycastAll;
    }

    public override IEnumerator UpdateCheck()
    {
        do
        {
            Collider[] colliders = default;

            switch (type)
            {
                case CheckType.BOX:
                colliders = Physics.OverlapBox(Position(transform), Vector3.one / 2 * radius, default, mask);
                break;

                case CheckType.CAPSULE:
                colliders = Physics.OverlapCapsule(transform.position + Vector3.up * range, transform.position - Vector3.up * range , radius , mask);
                break;

                case CheckType.SPHERE:
                colliders = Physics.OverlapSphere(Position(transform) , radius, mask);
                break;
            }

            bool isColliding = colliders.Length > 0; 

            data = colliders.Length > 0 && colliders[0].TryGetComponent(out T controlPoint) ? controlPoint : default;
                
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

            yield return new WaitForSecondsRealtime(0);
        } while (useRaycastAll);
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
    }

    public T GetData()
    {
        return data;
    }
}