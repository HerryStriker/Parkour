using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Locomotion : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider capsuleCollider;
    
    const float CHARACTER_SPEED = 4f;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        capsuleCollider = GetComponent<CapsuleCollider>();

        capsuleCollider.center += new Vector3(0, 1, 0);
        capsuleCollider.height = 2;
    }
    private void Start() {
        InputManager.Instance.EnableMove();
    }
    private void Update() 
    {
        var moveDirection = InputManager.Instance.Move;
        Move(moveDirection);
    }

    void Move(Vector2 direction)
    {
        //var moveDirection = new Vector3(direction.x, 0, direction.y);
        var directionalMovement = transform.forward * direction.y + transform.right * direction.x;

        rb.linearVelocity = directionalMovement.normalized * CHARACTER_SPEED;
    }
}
 
