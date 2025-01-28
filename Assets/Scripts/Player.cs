using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Constants
    [SerializeField] public float MoveSpeed { get; private set; } = 10f;
    [SerializeField] public float SprintMultiplier { get; private set; } = 2f;
    [SerializeField] public float ModelRotationSpeed { get; private set; } = 5f;
    [SerializeField] public float JumpStrength { get; private set; } = 10f;
    private float groundedThreshold;
    public float Height { get; private set; }

    // Components
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private PlayerVisuals playerVisuals;

    // Running variables
    public bool IsChasing { get; set; } = false;
    public bool IsDead { get; private set; } = false;
    public bool IsWalking { get; private set; } = false;
    public bool IsSprinting { get; private set; } = false;
    public bool IsCrouching { get; private set; } = false;
    private float verticalSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        Height = GetComponent<CapsuleCollider>().height;
        groundedThreshold = Height / 2 + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate gravity
        if (IsGrounded() && verticalSpeed <= 0) verticalSpeed = 0;
        else verticalSpeed += Physics.gravity.y * Time.deltaTime;

        // Play fall animation
        if (verticalSpeed <= 0) playerVisuals.PlayAnimation(PlayerVisuals.FALL);

        transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
    }

    public void KillPlayer()
    {
        IsDead = true;
        playerVisuals.PlayAnimation(PlayerVisuals.DYING);
    }

    public void RevivePlayer()
    {
        IsDead = false;
    }

    public void SetPosition(Vector3 position)
    {
        if (transform.position == position) IsWalking = false;
        else IsWalking = true;
        transform.position = position;
    }

    public void SetForward(Vector3 rotation)
    {
        transform.forward = rotation;
    }

    public void Jump(float amount)
    {
        verticalSpeed = amount;
        playerVisuals.PlayAnimation(PlayerVisuals.JUMP);
	}

    public void Crouch(bool crouching)
    {
        IsCrouching = crouching;
		// TODO: Implement crouch animation (crouching won't actually do anything but the crouch animation)
	}

    public void Sprint(bool sprinting)
    {
        IsSprinting = sprinting;
	}

    public void Interact(bool interacting)
    {
        // TODO: Spawn hurtbox
        playerVisuals.PlayAnimation(PlayerVisuals.HIT);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundedThreshold, collisionLayer);
    }
}
