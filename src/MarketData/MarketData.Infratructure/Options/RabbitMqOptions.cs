namespace MarketData.Infratructure.Options;
public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string AmqpUri { get; set; }
    public string ApiUri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
