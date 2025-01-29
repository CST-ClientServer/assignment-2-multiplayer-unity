using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDriver : MonoBehaviour
{
	// Components
	private InputManager inputManager;

	// Running variables
	public bool IsPlaying { get; private set; } = false;
	private static readonly float timeLimitSeconds = 60;
	private static readonly float startDelaySeconds = 5;
	private float timer = 0;
	private float currentLimit;

	// Start is called before the first frame update
	void Start()
	{
		currentLimit = startDelaySeconds;
		inputManager = InputManager.Instance;
		inputManager.Disabled = true;
	}

	// Update is called once per frame
	void Update()
	{
		RunPregame();
		RunGame();
		RunTimer();
	}

	public float GetTime()
	{
		return timer;
	}

	private void RunPregame()
	{
		if (IsPlaying) return;
		Debug.Log("Game Not Started Yet");
	}

	private void RunGame()
	{
		if (!IsPlaying) return;

		Debug.Log("Game Playing");
	}

	private void RunTimer()
	{
		if (timer > currentLimit)
		{
			currentLimit = currentLimit == startDelaySeconds ? timeLimitSeconds : startDelaySeconds;
			IsPlaying = currentLimit == timeLimitSeconds;
			inputManager.Disabled = !IsPlaying;
			timer = 0;
		}
		else timer += Time.deltaTime;
	}
}