using System;
using UnityEngine;

public class Animate : MonoBehaviour {

    Holder holder;
    Animator animator;
    Locomotion locomotion;
    Rigidbody rb;

    [Header("JUMP ANIMATE")]
    [SerializeField, Range(0,1)] float chargedJumpPercentage = .9f; 


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        holder = GetComponent<Holder>();
        locomotion = GetComponent<Locomotion>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        InputManager.Instance.OnJumpCanceled += OnJumpCanceled;
        InputManager.Instance.OnJumpStart += OnJumpStart;
        holder.GroundCheck.OnEnter += OnEnterGroundCheck;
    }


    [Header("Air Animation")]
    [SerializeField, Range(-1,0)] float fallingThreshold = 0f; 
    [SerializeField, Range(-10,0)] float minAirVelocity = -2f; 
    [SerializeField, Range(0,10)] float maxAirVelocity = 2f; 

    void Update()
    {
        animator.SetFloat("JUMP_NORMALIZED", holder.NormalizedTime);
        animator.SetBool("ISFULLCHARGED", holder.NormalizedTime > chargedJumpPercentage);
        animator.SetBool("FALLING", rb.linearVelocity.y < fallingThreshold);
        animator.SetBool("AIR", !holder.GroundCheck.IsColliding);

        float velocity = Mathf.Clamp(rb.linearVelocity.y , minAirVelocity, maxAirVelocity);
        animator.SetFloat("FALLING_SPEED", velocity);

        animator.SetFloat("VELOCITY", holder.NormalizedVelocity);
        animator.SetBool("IS_MOVING", InputManager.Instance.IsMoving);
    }


    private void OnEnterGroundCheck(object inputManager, EventArgs args)
    {
        animator.SetTrigger("LANDING");
        animator.ResetTrigger("JUMP");
        animator.ResetTrigger("JUMP_START");
    }


    private void OnJumpStart(object sender, EventArgs e)
    {
            animator.SetTrigger("JUMP_START");
    }

    void OnJumpCanceled(object inputManager, EventArgs args)
    {
        if(locomotion.IsJumping)
        {
            animator.SetTrigger("JUMP");
        }
    }

}
