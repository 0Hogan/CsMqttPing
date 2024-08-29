
// using System.Security.Authentication;
// using System.Security.Cryptography.X509Certificates;

using System.Text;
using MQTTnet;
using MQTTnet.Client;
// using MQTTnet.Client.Connecting;
// using MQTTnet.Client.Options;
// using MQTTnet.Client.Subscribing;

public class MqttExample
{
    public MqttExample() {}

    public async Task InitMqttClient()
    {
        Console.Write("Initializing MqttClient... ");
        var mqttFactory = new MqttFactory();
        mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
                                    .WithTcpServer(serverAddress, serverPort)
                                    .WithCredentials(serverUsername, serverPassword)
                                    .Build();

        try
        {
            var connectResult = await mqttClient.ConnectAsync(mqttClientOptions); /// @mhogan failing here.
            isInitialized = connectResult.ResultCode == MqttClientConnectResultCode.Success;
            Console.WriteLine("Done!");
        }
        catch(System.OperationCanceledException)
        {
            Console.WriteLine($"Unable to connect to mqttClient (Operation Canceled)");
            return;
        }
        // catch(MQTTnet.Exceptions.MqttCommunicationException)
        // {
        //     Console.WriteLine("Unable to connect to mqttClient (couldn't connect)");
        // }
    }

    public async Task DeinitMqttClient()
    {
        if (isInitialized)
        {
            Console.WriteLine($"Disconnecting client associated with topic \"{topic}\"");
            await mqttClient.DisconnectAsync();
            isInitialized = false;
        }
    }

    public async Task PublishMsg(string payload)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic(topic)
                                    .WithPayload(payload)
                                    .Build();
        if (mqttClient is null)
        {
            Console.WriteLine($"MQTT Client hasn't been initialized! Couldn't publish \"{payload}\" to topic \"{topic}\"");
            return;
        }

        Console.WriteLine($"Sending MQTT message \"{payload}\" to topic \"{topic}\".");
        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    
    }

    public async Task Subscribe()
    {
            // Subscribe to a topic
            await mqttClient.SubscribeAsync(topic);

            if (mqttClient is null)
            {
                Console.WriteLine($"Tried to subscribe to {topic}, but mqttClient was uninitialized!");
                return;
            }

            Console.WriteLine($"Subscribing to {topic}!");

            // Callback function when a message is received
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
                if (Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment) == "ping")
                {
                    await PublishMsg("pong");
                }
            };
    }

    public bool IsInitialized() { return isInitialized; }

    private IMqttClient? mqttClient;
    private bool isInitialized = false;
    private string serverAddress = "ip-address";
    private int serverPort = 1883;
    private string serverUsername = "username";
    private string serverPassword = "password";
    
    private string topic = "/test/pingpong";
}
