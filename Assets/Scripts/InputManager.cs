using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour
{
	public static InputManager Instance {  get; private set; }
	public static bool IsLockedCursor { get; private set; } = false;
	public bool Disabled = false;
	public static float mouseSensitivity = 40f;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		LockMouse(true);	
	}
	private void Update()
	{
		ReadPause();
	}

	private bool ReadPause()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) LockMouse(!IsLockedCursor);
		// Returns if game is paused or not (if cursor locked -> not paused)
		return !IsLockedCursor; 
	}

	public float GetMouseInput(string axis)
	{
		if (Disabled) return 0;
		if (!IsLockedCursor) return 0;
		return Input.GetAxis(axis) * mouseSensitivity * Time.deltaTime;
	}

	public Vector3 GetInputVector()
	{
		if (Disabled) return Vector3.zero;

		// Get camera directions
		Vector3 forwardVector = Camera.main.transform.forward;
		forwardVector.y = 0; // Ignore y component to stop moving downwards
		forwardVector.Normalize();
		Vector3 rightVector = Camera.main.transform.right;
		rightVector.y = 0;
		rightVector.Normalize();

		// Calculate direction Vector
		Vector3 directionVector = new();
		if (Input.GetKey(KeyCode.W)) directionVector += forwardVector;
		if (Input.GetKey(KeyCode.S)) directionVector -= forwardVector;
		if (Input.GetKey(KeyCode.A)) directionVector -= rightVector;
		if (Input.GetKey(KeyCode.D)) directionVector += rightVector;

		return directionVector;
	}

	public bool AttemptedJump()
	{
		if (Disabled) return false;
		return Input.GetKey(KeyCode.Space);
	}

	public bool AttemptedInteract()
	{
		if (Disabled) return false;
		return Input.GetKey(KeyCode.E) || Input.GetMouseButton(0);
	}

	public bool AttemptedCrouch()
	{
		if (Disabled) return false;
		return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
	}

	public bool AttemptedSprint()
	{
		if (Disabled) return false;
		return Input.GetKey(KeyCode.LeftShift);
	}

	private void LockMouse(bool locked)
	{
		if (locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			IsLockedCursor = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			IsLockedCursor = false;
		}
	}
}
