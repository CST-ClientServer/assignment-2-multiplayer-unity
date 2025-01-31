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
        clock.Start();
	}

    public async void BroadcastVector(int tag, float[] vector)
    {
        if (broadcasting)
            await Clients.Others.SendAsync("OnVectorReceived", tag, vector);
    }

    public async void BroadcastBool(int tag, bool state)
    {        
        if (broadcasting)
            await Clients.Others.SendAsync("OnBoolReceived", tag, state);            
    }
    
    public async void BroadcastFloat(int tag, float number)
    {        
        if (broadcasting)
            await Clients.Others.SendAsync("OnFloatReceived", tag, number);           
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

