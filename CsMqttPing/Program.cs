// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.Timers;
using MQTTnet.Exceptions;

class Program
{
    private static System.Timers.Timer? timer;
    private static MqttExample? mqttExample;

    private static void SetTimer()
    {
        // Create a timer with a two second interval.
        timer = new System.Timers.Timer(2000);
        // Hook up the Elapsed event for the timer. 
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;
        Console.WriteLine("Set timer!");
    }

    private static async void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                          e.SignalTime);
        Console.WriteLine("OnTimedEvent called...");
        if (mqttExample is null)
        {
            Console.WriteLine("Tried to publish a ping msg, but mqttExample is null!");
            return;
        }
        await mqttExample.PublishMsg("ping");
    }

    static async Task Main(string[] args)
    {
        try
        {
            mqttExample = new();
            await mqttExample.InitMqttClient();

            if (!mqttExample.IsInitialized())
            {
                Console.WriteLine("mqttExample wasn't initialized!");
                return;
            }

            await mqttExample.Subscribe();
            SetTimer();
            // while (true) {}
            Console.ReadLine();
        }
        catch(MqttClientNotConnectedException)
        {
            Console.WriteLine("Mqtt Client not connected. Quitting...");
        }
    }
}