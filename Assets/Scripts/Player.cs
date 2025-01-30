using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	// Constants
	public float MoveSpeed { get; private set; } = 10f;
	public float SprintMultiplier { get; private set; } = 2f;
	public float ModelRotationSpeed { get; private set; } = 5f;
	public float JumpStrength { get; private set; } = 10f;
	public float Height { get; private set; }
	public float Radius { get; private set; }
	private float groundedThreshold;

	// Components
	[SerializeField] private LayerMask collisionLayer;
	[SerializeField] private PlayerVisuals playerVisuals;

	// Needed since transform_get can't be called on side thread
	[SerializeField] public Vector3 Position;
	[SerializeField] public Vector3 Forward;

	// Events
	public UnityEvent PlayerDiedEvent = new();
	public UnityEvent PlayerReviveEvent = new();	
	public UnityEvent PlayerCrouchEvent = new();
	public UnityEvent PlayerSprintEvent = new();

	// Running variables
	public bool IsChasing { get; set; } = false;
	public bool IsDead { get; private set; } = false;
	public bool IsWalking { get; private set; } = false;
	public bool IsSprinting { get; private set; } = false;
	public bool IsCrouching { get; private set; } = false;	
	private float verticalSpeed = 0;
	private Queue<Action> interactQueue = new();

	void Start()
	{
		transform.position = Position;
		transform.forward = Forward;

		Height = GetComponent<CapsuleCollider>().height;
		Radius = GetComponent<CapsuleCollider>().radius;
		groundedThreshold = Height / 2;
	}

	void Update()
	{
		// Calculate gravity
		if (IsGrounded() && verticalSpeed <= 0) verticalSpeed = 0;
		else verticalSpeed += Physics.gravity.y * Time.deltaTime;

		// Play fall animation
		if (verticalSpeed <= 0) playerVisuals.PlayAnimation(PlayerVisuals.FALL);

		Position += Vector3.up * verticalSpeed * Time.deltaTime;
		transform.position = Position;
		transform.forward = Forward;

		// Process interact queue from separate thread
		while (interactQueue.Count > 0) interactQueue.Dequeue().Invoke();
	}

	public void KillPlayer()
	{
		IsDead = true;
		playerVisuals.PlayAnimation(PlayerVisuals.DYING);
		PlayerDiedEvent.Invoke();
	}

	public void RevivePlayer()
	{
		IsDead = false;
		playerVisuals.PlayAnimation(PlayerVisuals.REVIVE);
		PlayerReviveEvent.Invoke();
	}

	public void SetPosition(Vector3 position)
	{
		if (Position == position) IsWalking = false;
		else IsWalking = true;
		Position = position;
	}

	public void SetForward(Vector3 rotation)
	{
		Forward = rotation;
	}

	public void Jump(float amount)
	{
		verticalSpeed = amount;
		playerVisuals.PlayAnimation(PlayerVisuals.JUMP);
	}

	public void Crouch(bool crouching)
	{
		// Entering state will automatically call animation
		IsCrouching = crouching;
		PlayerCrouchEvent.Invoke();
	}

	public void Sprint(bool sprinting)
	{
		// Entering state will automatically call animation
		IsSprinting = sprinting;
		PlayerSprintEvent.Invoke();
	}

	public void Interact()
	{
		if (!IsChasing) return; // Don't do anything if player isnt it
		playerVisuals.PlayAnimation(PlayerVisuals.HIT);

		// Need to queue it since unity doesnt let capsulecast in separate thread
		interactQueue.Enqueue(new Action(() => {

			// Check collision in front
			RaycastHit collidedObject;
			bool collided = Physics.CapsuleCast(Position, Position + Vector3.up * Height, Radius,
					Forward, out collidedObject, 2f);

			// Early exit if no collision
			if (!collided) return;

			// Try to invoke kill on collided object
			Player player = collidedObject.collider.GetComponent<Player>();
			if (player) player.KillPlayer();
		}));
	}

	public bool IsGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, groundedThreshold, collisionLayer);
	}
}