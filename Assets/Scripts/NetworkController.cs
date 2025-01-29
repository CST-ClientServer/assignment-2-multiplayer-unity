using System.Threading;
using UnityEngine;

// Expected to be attached to remote player
public class NetworkController : MonoBehaviour
{
    [SerializeField] private Transform networkManager;
    private INetwork network;
    private Player remotePlayer;

    void Start()
    {
        remotePlayer = GetComponent<Player>();
        network = networkManager.GetComponent<INetwork>();
        (new Thread(() => network.ReceiveMessage(this))).Start(); // Start receiver thread
	}

    public void MovePlayer(Vector3 position)
    {
        remotePlayer.SetPosition(position);
    }

    public void RotatePlayer(Vector3 forward)
    {
        remotePlayer.SetForward(forward);
    }

    public void TriggerInteract()
    {
        remotePlayer.Interact();
    }

    public void TriggerJump()
    {
        remotePlayer.Jump(remotePlayer.JumpStrength);
    }

    public void ToggleSprint(bool sprint)
    {
        remotePlayer.Sprint(sprint);
    }

    public void ToggleCrouch(bool crouch)
    {
        remotePlayer.Crouch(crouch);
    }

    public void ToggleDead(bool dead)
    {
        if (dead) remotePlayer.KillPlayer();
        else remotePlayer.RevivePlayer();
    }

    public void SetChasing(bool chasing)
    {
        remotePlayer.IsChasing = chasing;
    }
}
