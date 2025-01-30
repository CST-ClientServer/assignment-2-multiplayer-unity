using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    // Messages
    private static readonly string CHASER_MESSAGE = "You're It";
    private static readonly string RUNNER_MESSAGE = "You're Running";
    private static readonly string WIN_MESSAGE = "You Win";
    private static readonly string LOSE_MESSAGE = "You Died";

    // Components
    private GameDriver gameDriver;
    private TextMeshProUGUI textField;

    // Running variables
    private bool inCountdown = true;
    private float showPeriodSeconds = 2;
    private float timer = 0;

	void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        gameDriver = GameDriver.Instance;
        
        // Add event listeners
        gameDriver.GameRestartEvent.AddListener(() => inCountdown = true);
        gameDriver.GameStartEvent.AddListener(() =>
        {
            inCountdown = false;
            if (gameDriver.IsLocalPlayerIt()) DisplayText(CHASER_MESSAGE);
            else DisplayText(RUNNER_MESSAGE);
        });
        gameDriver.GameEndEvent.AddListener(() => 
        {
            inCountdown = false;
            if (gameDriver.IsLocalPlayerDead()) DisplayText(LOSE_MESSAGE);
            else DisplayText(WIN_MESSAGE);
        });
    }

	private void Update()
	{
        if (inCountdown)
        {
            textField.text = (GameDriver.StartDelaySeconds - Mathf.Ceil(gameDriver.GetTime())).ToString();
            return;
        }
        // Early return if no text is on screen
        if (textField.text.Length == 0) return;

        // Run timer to remove text after 2 seconds
        if (timer > showPeriodSeconds)
        {
            textField.text = "";
        }
        else timer += Time.deltaTime;
	}

	private void DisplayText(string text)
    {
        textField.text = text;
        timer = 0;
    }
}
