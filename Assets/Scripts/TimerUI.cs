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
    private TextMeshPro textField;

    // Running variables
    private bool showTimer;

	void Start()
    {
        textField = GetComponent<TextMeshPro>();
        gameDriver = GameDriver.Instance;

        // Add event listeners
        gameDriver.GameEndEvent.AddListener(() =>
        {
            showTimer = false;
            textField.text = POSTGAME_MESSAGE;
        });
        gameDriver.GameStartEvent.AddListener(() => showTimer = true);
    }

	private void Update()
	{
        if (showTimer) textField.text = TIMER_MESSAGE_TEMPLATE + gameDriver.GetTime();
	}
}
