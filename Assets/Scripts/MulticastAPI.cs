using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MulticastAPI : MonoBehaviour, INetwork
{
	public static MulticastAPI Instance { get; private set; }
	
	// Constants
	[SerializeField] private string ipAddress = "230.0.0.1";
	[SerializeField] private int port = 11000;

	// Components
	private Socket inSocket;
	private Socket outSocket;
	private IPEndPoint sendEndpoint;
	private EndPoint localEndpoint;
	private EndPoint remoteEndpoint;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		// Create endpoints
		IPAddress ip = IPAddress.Parse(ipAddress);
		sendEndpoint = new IPEndPoint(ip, port);
		localEndpoint = new IPEndPoint(IPAddress.Any, port);
		remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

		// Create sockets
		outSocket = CreateSocket();
		inSocket = CreateSocket(localEndpoint);
	}

	public void SendMessage(ByteTag tag, Vector3 data)
	{
		byte[] byteData = new byte[13];
		byteData[0] = (byte) tag; // Cast to byte is fine for < 255
		Buffer.BlockCopy(new float[] {data.x, data.y, data.z}, 0, byteData, 1, 12); // Skip first because its the tag
		outSocket.SendTo(byteData, sendEndpoint);
	}

	public void SendMessage(ByteTag tag, bool data)
	{
		byte[] byteData = new byte[2];
		byteData[0] = (byte) tag;
		// Convert bool to 0 or 1 since size of bool fluctuates with machine
		byteData[1] = (byte) (data ? 1 : 0);
		outSocket.SendTo(byteData, sendEndpoint);
	}

	public void SendMessage(ByteTag tag, float data)
	{
		byte[] byteData = new byte[5];
		byteData[0] = (byte) tag;
		Buffer.BlockCopy(new float[] { data }, 0, byteData, 1, 4); // Convert float to bytes
		outSocket.SendTo(byteData, sendEndpoint);
	}

	public void SendMessage(ByteTag tag, GameDriver.GameState state)
	{
		byte[] byteData = new byte[2];
		byteData[0] = (byte)tag;
		byteData[1] = (byte)state;
		outSocket.SendTo(byteData, sendEndpoint);
	}

	// This function is expected to be started on a separate thread
	public void ReceiveMessage(NetworkController controller)
	{
		// Set up buffer for reading
		byte[] buffer = new byte[13]; // Biggest size it can be
		try
		{
			// Set up loop to read from socket
			while (true)
			{
				inSocket.ReceiveFrom(buffer, ref remoteEndpoint);
				ByteTag tag = (ByteTag) buffer[0]; // Convert first byte back into tag

				if (tag == ByteTag.TIMER_FLOAT)
				{
					// Handle float data types
					float timer = System.BitConverter.ToSingle(buffer, 1);					
					GameDriver.Instance.SyncTimer(timer);
				}
				else if (tag == ByteTag.INTERACT_TRIGGER) controller.TriggerInteract();
				else if (tag == ByteTag.JUMP_TRIGGER) controller.TriggerJump();
				else if (tag != ByteTag.POSITION_VECTOR && tag != ByteTag.FORWARD_VECTOR)
				{
					bool value = (byte)buffer[1] == 1 ? true : false;
					// Handle the boolean data types
					switch (tag)
					{						
						case ByteTag.DEAD_BOOL:
							controller.ToggleDead(value);
							break;
						case ByteTag.SPRINTING_BOOL:
							controller.ToggleSprint(value);
							break;
						case ByteTag.CROUCHING_BOOL:
							controller.ToggleCrouch(value);
							break;
					}
				}
				else if (tag == ByteTag.GAME_STATE_CHANGE)
				{
					// Handle game state changes
					GameDriver.GameState state = (GameDriver.GameState)buffer[1];
					if (state == GameDriver.GameState.PRE_GAME) GameDriver.Instance.RestartGame();
					else if (state == GameDriver.GameState.POST_GAME) GameDriver.Instance.EndGame();
					else GameDriver.Instance.StartGame(); // Last state it could be is start game
				}
				else
				{
					// Handle the vector data types
					float[] floatValues = new float[3];
					Buffer.BlockCopy(buffer, 1, floatValues, 0, 12);
					if (tag == ByteTag.POSITION_VECTOR)
						controller.MovePlayer(new Vector3(floatValues[0], floatValues[1], floatValues[2]));
					else if (tag == ByteTag.FORWARD_VECTOR)
						controller.RotatePlayer(new Vector3(floatValues[0], floatValues[1], floatValues[2]));
				}
			}
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}

	}

	private Socket CreateSocket(EndPoint localEndpoint = null)
	{
		try
		{
			Socket multicastSocket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			if (localEndpoint == null) return multicastSocket; // Early return for creating sender socket

			// Set up receiver socket
			multicastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
			multicastSocket.Bind(localEndpoint);
			multicastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false); // Disable loopback
			multicastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 16);
			MulticastOption options = new(IPAddress.Parse(ipAddress), IPAddress.Any);
			multicastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, options);
			return multicastSocket;
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
			return null;
		}
	}
}
