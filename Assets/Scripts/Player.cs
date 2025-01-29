using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	// Constants
	[SerializeField] public float MoveSpeed { get; private set; } = 10f;
	[SerializeField] public float SprintMultiplier { get; private set; } = 2f;
	[SerializeField] public float ModelRotationSpeed { get; private set; } = 5f;
	[SerializeField] public float JumpStrength { get; private set; } = 10f;
	private float groundedThreshold;
	public float Height { get; private set; }
	public float Radius { get; private set; }

	// Components
	[SerializeField] private LayerMask collisionLayer;
	[SerializeField] private PlayerVisuals playerVisuals;

	// Events
	public UnityEvent PlayerDiedEvent = new();
	public UnityEvent PlayerCrouchEvent = new();
	public UnityEvent PlayerReviveEvent = new();

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
		Radius = GetComponent<CapsuleCollider>().radius;
		groundedThreshold = Height / 2;
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
		// Entering state will automatically call animation
		IsCrouching = crouching;
	}

	public void Sprint(bool sprinting)
	{
		// Entering state will automatically call animation
		IsSprinting = sprinting;
	}

	public void Interact()
	{
		playerVisuals.PlayAnimation(PlayerVisuals.HIT);

		// Check collision in front
		RaycastHit collidedObject;
		bool collided = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * Height, 
			Radius, transform.forward, out collidedObject, 2f);

		// Early exit if no collision
		if (!collided) return;

		// Try to invoke kill on collided object
		Player player = collidedObject.collider.GetComponent<Player>();
		if (player) player.KillPlayer();
	}

	public bool IsGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, groundedThreshold, collisionLayer);
	}
}