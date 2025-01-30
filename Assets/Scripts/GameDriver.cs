using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameDriver : MonoBehaviour
{
	public static GameDriver Instance { get; private set; }

	// State enum
	public enum GameState
	{
		PRE_GAME,
		IN_GAME,
		POST_GAME
	}

	// Components
	[SerializeField] private Player localPlayer;
	[SerializeField] private Player remotePlayer;
	private InputManager inputManager;

	// Running variables
	public GameState State { get; private set; } = GameState.PRE_GAME;
	public static readonly float TimeLimitSeconds = 60;
	public static readonly float StartDelaySeconds = 10;
	public static readonly float PostGameDelaySeconds = 5;
	private float timer = 0;
	private float currentLimit;

	// Events
	public UnityEvent GameStartEvent = new();
	public UnityEvent GameEndEvent = new();
	public UnityEvent GameRestartEvent = new();

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
		currentLimit = StartDelaySeconds;
		inputManager = InputManager.Instance;
		inputManager.Disabled = true;

		// Set up first round
		localPlayer.IsChasing = true;
		remotePlayer.IsChasing = false;

		// Listen to player death
		localPlayer.PlayerDiedEvent.AddListener(() => EndGame());
		remotePlayer.PlayerDiedEvent.AddListener(() => EndGame());
	}

	// Update is called once per frame
	void Update()
	{
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

	public bool IsPlayerDead(bool local)
	{
		if (local) return localPlayer.IsDead;
		else return remotePlayer.IsDead;
	}

	public bool IsPlayerIt(bool local)
	{
		if (local) return localPlayer.IsChasing;
		else return remotePlayer.IsChasing;		
	}

	private void RunTimer()
	{
		// Trigger next state after time expires
		if (timer > currentLimit)
		{
			// Set next game state
			if (currentLimit == StartDelaySeconds)
			{
				currentLimit = TimeLimitSeconds;
				StartGame();
			}
			else if (currentLimit == TimeLimitSeconds)
			{
				currentLimit = PostGameDelaySeconds;
				EndGame();
			}
			else if (currentLimit == PostGameDelaySeconds)
			{
				currentLimit = StartDelaySeconds;
				RestartGame();
			}
		}
		else timer += Time.deltaTime;
	}

	private void StartGame()
	{
		// Set properties
		inputManager.Disabled = false;
		State = GameState.IN_GAME;
		timer = 0;

		// Respawn players to spawn positions
		if (localPlayer.IsDead) localPlayer.RevivePlayer();
		else if (remotePlayer.IsDead) remotePlayer.RevivePlayer();
		localPlayer.transform.position = new Vector3(0, 0.5f, -5);
		localPlayer.transform.forward = new Vector3(0, 0, 1);
		remotePlayer.transform.position = new Vector3(0, 0.5f, 5);
		remotePlayer.transform.forward = new Vector3(0, 0, -1);

		// Alternate between whos it
		localPlayer.IsChasing = !localPlayer.IsChasing;
		remotePlayer.IsChasing = !remotePlayer.IsChasing;

		GameStartEvent.Invoke();
	}

	private void EndGame()
	{
		inputManager.Disabled = true;
		State = GameState.POST_GAME;
		timer = 0;

		GameEndEvent.Invoke();
	}

	private void RestartGame()
	{
		State = GameState.PRE_GAME;
		timer = 0;

		GameRestartEvent.Invoke();
	}
}