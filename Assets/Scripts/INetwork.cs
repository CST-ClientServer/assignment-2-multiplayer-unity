using UnityEngine;

public interface INetwork
{
    public void SendMessage(ByteTag tag, Vector3 data);
    public void SendMessage(ByteTag tag, bool data);
    public void SendMessage(ByteTag tag, float data);
    public void SendMessage(ByteTag tag, GameDriver.GameState state);
    // Expected to be something to start on a separate thread
    public void ReceiveMessage(NetworkController networkController);
}
