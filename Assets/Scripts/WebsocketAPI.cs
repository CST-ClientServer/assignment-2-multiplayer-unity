using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class WebsocketAPI : MonoBehaviour, INetwork
{
	[SerializeField] string serverURL = "http://localhost:11000";
	private HubConnection connection;

	private void Awake()
	{
		connection = new HubConnectionBuilder().WithUrl(serverURL).Build();
		connection.StartAsync();
	}

	public void SendMessage(ByteTag tag, Vector3 data)
	{		
		throw new System.NotImplementedException();
	}

	public void SendMessage(ByteTag tag, bool data)
	{
		throw new System.NotImplementedException();
	}

	public void SendMessage(ByteTag tag, float data)
	{
		throw new System.NotImplementedException();
	}

	public void ReceiveMessage(NetworkController networkController)
	{
		throw new System.NotImplementedException();
	}
}
