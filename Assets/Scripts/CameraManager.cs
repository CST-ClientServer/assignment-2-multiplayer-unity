using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Constants
    [SerializeField] private Transform target;
    [SerializeField] private float bottomClamp = -89f;
    [SerializeField] private float topClamp = 80f;

    // Components
    InputManager inputManager;

    // Running variables
    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    void Start()
    {        
        inputManager = InputManager.Instance;
    }

	private void LateUpdate()
	{
        HandleCamera();
	}

	private void HandleCamera()
    {
        float mouseX = inputManager.GetMouseInput("Mouse X");
        float mouseY = inputManager.GetMouseInput("Mouse Y");
        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, false);
        ApplyRotations(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotations(float pitch, float yaw)
    {        
        target.rotation = Quaternion.Euler(pitch, yaw, target.eulerAngles.z);
    }

    private float UpdateRotation(float currentRotation, float input, bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        currentRotation = isXAxis ? Mathf.Clamp(currentRotation, bottomClamp, topClamp) : Mathf.Clamp(currentRotation, float.MinValue, float.MaxValue);

		return currentRotation;
    }
}
