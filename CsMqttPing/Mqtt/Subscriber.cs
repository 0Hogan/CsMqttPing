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

        /// @note This lambda must be asnychronous to get added to the list of callbacks for the
        ///       MQTT client, but there doesn't need to be a return type for an MQTT callback.
        ///       Thus, we have the warning that the following async method will run synchronously.
        /// @todo Figure out how to fix this in a proper way rather than just ignoring it.
        #pragma warning disable CS1998
        mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var msgPayload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                // await Task.Run(msgCallback(msgPayload));
                msgCallback(msgPayload);
            };
        #pragma warning restore CS1998
    }

    public string Topic { get; }
    private IMqttClient mqttClient;
}