using Mqtt;

class PingPongMsg : Message
{
    public PingPongMsg() : base("/test/iot/pingpong") {}

    public override string GetPayload()
    {
        return Msg;
    }

    public string Msg { get; set; } = "";

}