// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.Timers;
using Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;

class Program
{
    static async Task InitMqttClient()
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
                                    .WithTcpServer(MqttServerInfo.Address, MqttServerInfo.Port)
                                    .WithCredentials(MqttServerInfo.Username, MqttServerInfo.Password)
                                    .Build();

        try
        {
            var connectResult = await mqttClient.ConnectAsync(mqttClientOptions);
            isInitialized = connectResult.ResultCode == MqttClientConnectResultCode.Success;
        }
        catch(OperationCanceledException)
        {
            Console.WriteLine($"Unable to connect to mqttClient (Operation Canceled)");
        }
        catch(Exception e)
        {
            Console.WriteLine($"Caught some other exception: {e.Message}");
        }
    }

    static async Task DeinitMqttClient()
    {
        if (isInitialized)
        {
            Console.WriteLine($"Disconnecting client associated with topic \"{topic}\"");
            await mqttClient.DisconnectAsync();
            isInitialized = false;
        }
    }

    static void OnPingPongMsgCallback(string payload)
    {
        Console.WriteLine($"I heard: \"{payload}\"");
        if (payload == "ping")
        {
            Console.WriteLine("Responding with pong.");
            PingPongMsg msg = new();
            msg.Msg = "pong";
            _ =pingPongPub.Publish(msg);
        }
    }

    static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();
        mqttClient = mqttFactory.CreateMqttClient();
        
        try
        {
            await InitMqttClient();

            if (!IsInitialized())
            {
                Console.WriteLine("mqttExample wasn't initialized!");
                return;
            }

            pingPongSub = new(mqttClient);
            pingPongPub = new(mqttClient);
            periodicPingPongPub = new(mqttClient, 2000, false);
            periodicPingPongPub.Msg.Msg = "ping";

            await pingPongSub.Subscribe(OnPingPongMsgCallback);
            periodicPingPongPub.Start();

            Console.ReadLine();
        }
        catch(MqttClientNotConnectedException)
        {
            Console.WriteLine("Mqtt Client not connected. Quitting...");
        }
    }
    
    private static Subscriber<PingPongMsg>? pingPongSub;
    private static Publisher<PingPongMsg>? pingPongPub;
    private static PeriodicPublisher<PingPongMsg>? periodicPingPongPub;

    public static bool IsInitialized() { return isInitialized; }

    private static IMqttClient? mqttClient;
    private static bool isInitialized = false;
    private static string topic = "/test/pingpong";    
}