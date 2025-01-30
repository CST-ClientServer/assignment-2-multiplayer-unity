using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Transform NetworkAPI;

	// Components
	private Player player;
	private InputManager inputManager;
	private INetwork network;

	void Start()
	{
		inputManager = InputManager.Instance;
		player = GetComponent<Player>();
		network = NetworkAPI.GetComponent<INetwork>();

		// Add listeners
		player.PlayerDiedEvent.AddListener(() => {
			inputManager.Disabled = true;
			network.SendMessage(ByteTag.DEAD_BOOL, true);
		});
		player.PlayerReviveEvent.AddListener(() => {
			inputManager.Disabled = false;
			network.SendMessage(ByteTag.DEAD_BOOL, false);
		});
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
		Vector3 directionVector = inputManager.GetInputVector().normalized;
		float speedFactor = player.IsSprinting ? player.MoveSpeed * player.SprintMultiplier : player.MoveSpeed;
		network.SendMessage(ByteTag.SPRINTING_BOOL, player.IsSprinting); // Send sprint state to network

		// Check collision and calculate slide angle if possible
		Vector3 moveVector;
		if (!CanMove(directionVector, speedFactor))
		{
			// Attempt movement on X only
			Vector3 directionVectorX = new Vector3(directionVector.x, 0, 0).normalized;
			if (CanMove(directionVectorX, speedFactor)) moveVector = directionVectorX * speedFactor;
			else
			{
				// Attempt movement on Z only
				Vector3 directionVectorZ = new Vector3(0, 0, directionVector.z).normalized;
				if (CanMove(directionVectorZ, speedFactor)) moveVector = directionVectorZ * speedFactor;
				else moveVector = Vector3.zero; // Both directions cannot move, so set to 0
			}
		}
		else moveVector = directionVector * speedFactor;

		// Set new position
		Vector3 newPosition = player.transform.position + moveVector * Time.deltaTime;
		player.SetPosition(newPosition);
		network.SendMessage(ByteTag.POSITION_VECTOR, newPosition);

		if (inputManager.AttemptedJump() && player.IsGrounded())
		{
			player.Jump(player.JumpStrength);
			network.SendMessage(ByteTag.JUMP_TRIGGER, true);
		}

		// Rotate based on direction
		Vector3 newForward = Vector3.Slerp(transform.forward, directionVector, Time.deltaTime * player.ModelRotationSpeed);
		player.SetForward(newForward);
		network.SendMessage(ByteTag.FORWARD_VECTOR, newForward);
	}

	private void HandleInteract()
	{
		if (!inputManager.AttemptedInteract()) return;
		player.Interact();
		network.SendMessage(ByteTag.INTERACT_TRIGGER, true);
	}

	private void HandleCrouch()
	{
		if (!inputManager.AttemptedCrouch()) return;
		player.Crouch(!player.IsCrouching);
		network.SendMessage(ByteTag.CROUCHING_BOOL, player.IsCrouching); // Don't flip since previous line does it already
	}

	private bool CanMove(Vector3 direction, float speed)
	{
		return !Physics.CapsuleCast(player.transform.position, player.transform.position + Vector3.up * player.Height,
				player.Radius, direction, speed * Time.deltaTime);
	}
}