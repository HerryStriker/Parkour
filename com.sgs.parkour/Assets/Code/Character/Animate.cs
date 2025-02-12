using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class Animate : MonoBehaviour
{
    
    Holder holder;
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        holder = GetComponent<Holder>();
    }

    void Update()
    {
        animator.SetFloat("VELOCITY", holder.NormalizedVelocity);
        animator.SetBool("IS_MOVING", InputManager.Instance.IsMoving);

    }
    
}
