using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    // Messages
    private static readonly string PREGAME_MESSAGE = "Game Starting In...";
    private static readonly string POSTGAME_MESSAGE = "Game Over";
    private static readonly string TIMER_MESSAGE_TEMPLATE = "Timer: ";

    // Components
    private GameDriver gameDriver;
    private TextMeshProUGUI textField;

    // Running variables
    private bool showTimer;

	void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        gameDriver = GameDriver.Instance;

        // Add event listeners
        gameDriver.GameStartEvent.AddListener(() => showTimer = true);
        gameDriver.GameEndEvent.AddListener(() =>
        {
            showTimer = false;
            textField.text = POSTGAME_MESSAGE;
        });
        gameDriver.GameRestartEvent.AddListener(() =>
        {
            showTimer = false;
            textField.text = PREGAME_MESSAGE;
        });
    }

	private void Update()
	{
        // Round to 1 decimal place
        if (showTimer) 
            textField.text = TIMER_MESSAGE_TEMPLATE + (GameDriver.TimeLimitSeconds - gameDriver.GetTime()).ToString("0.0");
	}
}
