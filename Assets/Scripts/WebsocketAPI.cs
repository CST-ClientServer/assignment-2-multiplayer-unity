using Microsoft.AspNetCore.SignalR.Client;
using System;
using UnityEngine;
using UnityEngine.LightTransport;

public class WebsocketAPI : MonoBehaviour, INetwork
{
	// Server properties
	[SerializeField] string serverURL = "https://localhost:8000";
	private static readonly string endpoint = "/broadcast";
	private HubConnection connection;

	// Components
	private NetworkController controller;

	private async void Awake()
	{
		connection = new HubConnectionBuilder().WithUrl(serverURL + endpoint, options =>
		{
			options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
		}).Build();
		await connection.StartAsync();

		// Handle float arrays as vectors
		connection.On<int, float[]>("OnVectorReceived", (int tag, float[] vectorData) => 
		{
			ByteTag convertedTag = (ByteTag) tag;			
			if (convertedTag == ByteTag.POSITION_VECTOR) controller.MovePlayer(new Vector3(vectorData[0], vectorData[1], vectorData[2]));
			else controller.RotatePlayer(new Vector3(vectorData[0], vectorData[1], vectorData[2])); // Rotational vector
		});
		// Handle floats
		connection.On<int, float>("OnFloatReceived", (int tag, float num) =>
		{
			ByteTag convertedTag = (ByteTag) tag;
			if (convertedTag == ByteTag.TIMER_FLOAT) GameDriver.Instance.SyncTimer(num);
		});
		// Handle bools
		connection.On<int, bool>("OnBoolReceived", (int tag, bool flag) =>
		{
			ByteTag convertedTag = (ByteTag) tag;
			switch (convertedTag)
			{
                case ByteTag.CHASING_BOOL:
					controller.SetChasing(flag);
					break;
				case ByteTag.DEAD_BOOL:
					controller.ToggleDead(flag);
					break;
				case ByteTag.SPRINTING_BOOL:
					controller.ToggleSprint(flag);
					break;
				case ByteTag.CROUCHING_BOOL:
					controller.ToggleCrouch(flag);
					break;
			}
        });
	}

    private void OnDestroy()
    {
		connection.StopAsync();
    }

    public async void SendMessage(ByteTag tag, Vector3 data)
	{
		await connection.SendAsync("BroadcastVector", tag, new float[] { data.x, data.y, data.z });		
	}

	public async void SendMessage(ByteTag tag, bool data)
	{
		await connection.SendAsync("BroadcastBool", tag, data);
	}

	public async void SendMessage(ByteTag tag, float data)
	{
		await connection.SendAsync("BroadcastFloat", tag, data);
	}

	public void ReceiveMessage(NetworkController networkController)
	{
		// Setting value since websocket variant uses events instead of manual listens
		controller = networkController;
	}
}
