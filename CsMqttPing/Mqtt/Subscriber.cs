using System.Text;

using MQTTnet;
using MQTTnet.Client;

namespace Mqtt;

public class Subscriber<msgType> where msgType : Message, new()
{

    public Subscriber(IMqttClient client)
    {
        mqttClient = client;
        Topic = new msgType().Topic;
    }

    public async Task Subscribe(Action<string> msgCallback)
    {
        await mqttClient.SubscribeAsync(Topic);

        mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                msgCallback(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
            };
    }

    public string Topic { get; }
    private IMqttClient mqttClient;
}