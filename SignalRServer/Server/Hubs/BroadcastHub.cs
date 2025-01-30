using System.Numerics;
using Microsoft.AspNetCore.SignalR;

public class BroadcastHub : Hub
{
    public async Task BroadcastVector(int tag, float[] vector)
    {       
        await Clients.All.SendAsync("OnVectorReceived", tag, vector);
    }

    public async Task BroadcastBool(int tag, bool state)
    {
        await Clients.All.SendAsync("OnBoolReceived", tag, state);
    }
    
    public async Task BroadcastFloat(int tag, float number)
    {
        await Clients.All.SendAsync("OnFloatReceived", tag, number);
    }
}

