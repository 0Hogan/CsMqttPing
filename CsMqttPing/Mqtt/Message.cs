namespace Mqtt;

public abstract class Message
{
    public Message(string topic)
    {
        Topic = topic;
    }

    public abstract string GetPayload();
    public string Topic { get; }
}