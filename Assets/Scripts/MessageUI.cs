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
    private TextMeshPro textField;

	void Start()
    {
        textField = GetComponent<TextMeshPro>();
        gameDriver = GameDriver.Instance;
        
        // Add event listeners
        gameDriver.GameStartEvent.AddListener(() => DisplayText(LOSE_MESSAGE));
        gameDriver.GameEndEvent.AddListener(() => DisplayText(WIN_MESSAGE));        
    }

    private void DisplayText(string text)
    {
        // TODO: Implement this with animation
    }
}
