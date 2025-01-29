using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Components
	private Player player;
	private InputManager inputManager;

	void Start()
	{
		inputManager = InputManager.Instance;
		player = GetComponent<Player>();

		// Add listeners
		player.PlayerDiedEvent.AddListener(() => inputManager.Disabled = true);
		player.PlayerReviveEvent.AddListener(() => inputManager.Disabled = false);
	}

	void Update()
	{
		HandleMovement();
		HandleInteract();
		HandleCrouch();
	}

	private void HandleMovement()
	{
		// Toggle sprint
		player.Sprint(inputManager.AttemptedSprint());

		// Calculate movement
		Vector3 directionVector = inputManager.GetInputVector();
		Vector3 moveVector = directionVector * player.MoveSpeed;
		if (player.IsSprinting) moveVector *= player.SprintMultiplier;

		player.SetPosition(player.transform.position + moveVector * Time.deltaTime);

		if (inputManager.AttemptedJump() && player.IsGrounded()) player.Jump(player.JumpStrength);

		// Rotate based on direction
		player.SetForward(Vector3.Slerp(transform.forward, directionVector, Time.deltaTime * player.ModelRotationSpeed));
	}

	private void HandleInteract()
	{
		if (!inputManager.AttemptedInteract()) return;
		player.Interact();
	}

	private void HandleCrouch()
	{
		if (!inputManager.AttemptedCrouch()) return;
		player.Crouch(!player.IsCrouching);
	}
}