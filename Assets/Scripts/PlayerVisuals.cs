using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    // Animation state names
    public static readonly string IS_WALKING = "IsWalking";
    public static readonly string IS_SPRINTING = "IsSprinting";
    public static readonly string IS_GROUNDED = "IsGrounded";
    public static readonly string IS_CROUCHING = "IsCrouching";
    public static readonly string REVIVE = "Reviving";
    public static readonly string JUMP = "Jumping";
    public static readonly string FALL = "Falling";
    public static readonly string DYING = "Dying";
    public static readonly string HIT = "Hitting";

    [SerializeField] private Player player;
    private Queue<Action> triggerQueue = new();
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking);
        animator.SetBool(IS_SPRINTING, player.IsSprinting);
        animator.SetBool(IS_CROUCHING, player.IsCrouching);
        animator.SetBool(IS_GROUNDED, player.IsGrounded());
        while (triggerQueue.Count > 0) triggerQueue.Dequeue().Invoke();
    }

    public void PlayAnimation(string animationName)
    {
        // SetTrigger not allowed to be called on main thread, need to queue it instead for update to call it
        triggerQueue.Enqueue(new Action(() => animator.SetTrigger(animationName)));
    }
}
