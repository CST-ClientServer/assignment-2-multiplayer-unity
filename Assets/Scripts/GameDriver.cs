using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameDriver : MonoBehaviour
{
	public static GameDriver Instance { get; private set; }

	// Components
	[SerializeField] private Player localPlayer;
	[SerializeField] private Player remotePlayer;
	private InputManager inputManager;

	// Running variables
	public bool IsPlaying { get; private set; } = false;
	private static readonly float timeLimitSeconds = 60;
	private static readonly float startDelaySeconds = 10;
	private float timer = 0;
	private float currentLimit;

	// Events
	public UnityEvent GameStartEvent = new();
	public UnityEvent GameEndEvent = new();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

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
		if (IsPlaying) RunGame();
		else RunPregame();
		RunTimer();
	}

	public float GetTime()
	{
		return timer;
	}

	public void SyncTimer(float time)
	{
		// Assumes person who joined late needs to sync with first person in
		if (time < timer) timer = time;
	}

	public bool IsLocalPlayerDead()
	{
		return localPlayer.IsDead;
	}

	private void RunPregame()
	{
		
	}

	private void RunGame()
	{
		
	}

	private void RunTimer()
	{
		if (timer > currentLimit)
		{
			currentLimit = currentLimit == startDelaySeconds ? timeLimitSeconds : startDelaySeconds;
			IsPlaying = currentLimit == timeLimitSeconds;

			if (IsPlaying) StartGame();
			else EndGame();			
		}
		else timer += Time.deltaTime;
	}

	private void StartGame()
	{
		// Set properties
		inputManager.Disabled = false;
		timer = 0;
		GameStartEvent.Invoke();

		// Respawn players to spawn positions
		localPlayer.transform.position = new Vector3(0, 0.5f, -5);
		localPlayer.transform.forward = new Vector3(0, 0, 1);
		remotePlayer.transform.position = new Vector3(0, 0.5f, 5);
		remotePlayer.transform.forward = new Vector3(0, 0, -1);
	}

	private void EndGame()
	{
		inputManager.Disabled = true;
		timer = 0;
		GameEndEvent.Invoke();
	}
}