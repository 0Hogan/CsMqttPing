using MQTTnet;
using MQTTnet.Client;

namespace Mqtt;

public class Publisher<msgType> where msgType : Message, new()
{
    public Publisher(IMqttClient client)
    {
        mqttClient = client;
        Topic = new msgType().Topic;
    }

    public async Task Publish(msgType msg)
    {
        var mqttMsg = new MqttApplicationMessageBuilder()
                                    .WithTopic(Topic)
                                    .WithPayload(msg.GetPayload())
                                    .Build();

        await mqttClient.PublishAsync(mqttMsg, CancellationToken.None);
    }

    private IMqttClient mqttClient;
    public string Topic { get; }

}