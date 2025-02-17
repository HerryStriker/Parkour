 using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Effect : MonoBehaviour
{
    Holder holder;

    [SerializeField] List<EffectMap> effects;
    [SerializeField] EffectMap currentEffect;
    void Awake()
    {
        holder = GetComponent<Holder>();
    }

    void Start()
    {
        // ON LANDING ON GROUND
        holder.GroundCheck.OnEnter += (sender, args) =>
        {
            currentEffect = GetEffect(MovementEffectType.LANDING);
            currentEffect?.Start(transform.position);
        };


        //STARTING TO LOAD A JUMP
        InputManager.Instance.OnJumpStart += (sender, args) =>
        {
            currentEffect = GetEffect(MovementEffectType.LOADING);
            currentEffect?.Start();
        };

        // AFTER RELEASE THE JUMP BUTTON
        InputManager.Instance.OnJumpCanceled += (sender, args) =>
        {
            currentEffect?.Stop();

            currentEffect = GetEffect(MovementEffectType.JUMPING);
            currentEffect?.Start(transform.position);            
        };
    }

    void Update()
    {
        var current = GetCurrentEffect(MovementEffectType.LOADING);
        current?.Update(holder.NormalizedTime);
    }

    EffectMap GetCurrentEffect(MovementEffectType type)
    {
        if(currentEffect != null && currentEffect.movementEffectType == type)
        {
            return currentEffect;
        } 
        return null;
    }

    EffectMap GetEffect(MovementEffectType movementEffectType)
    {
        return effects.Find((e) => {
            return e.movementEffectType == movementEffectType;
            });
    }
}
