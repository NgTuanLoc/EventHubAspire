namespace SharedLibrary.Configurations;

public class EventHubSettings
{
    public required string ConsumerGroup { get; set; }
    public required string HubName { get; set; }
}
