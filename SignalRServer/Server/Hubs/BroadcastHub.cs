using System.Numerics;
using Microsoft.AspNetCore.SignalR;

public class BroadcastHub : Hub
{
    private static readonly int broadcastPeriod = 1;
    private static readonly int downPeriod = 40;
    private bool broadcasting = true;

    public BroadcastHub()
    {
        Thread clock = new(() => RunClock());
    }

    public async Task BroadcastVector(int tag, float[] vector)
    {
        Console.WriteLine("Received Vector");
        if (broadcasting)
            await Clients.All.SendAsync("OnVectorReceived", tag, vector);
    }

    public async Task BroadcastBool(int tag, bool state)
    {
        Console.WriteLine("Received Bool");
        if (broadcasting)
            await Clients.All.SendAsync("OnBoolReceived", tag, state);
    }
    
    public async Task BroadcastFloat(int tag, float number)
    {
        Console.WriteLine("Received Float");
        if (broadcasting)
            await Clients.All.SendAsync("OnFloatReceived", tag, number);
    }

    private void RunClock()
    {
        while(true)
        {
            Thread.Sleep(broadcastPeriod);
            broadcasting = false;
            Thread.Sleep(downPeriod);
            broadcasting = true;
        }
    }
}

