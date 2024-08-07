using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedLibrary;
using SharedLibrary.Configurations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventHubProducerClientService(builder.Configuration.GetConnectionString("azureeventhub") ?? "");
builder.Services.Configure<EventHubSettings>(builder.Configuration.GetSection("EventHub"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (EventHubProducerClient producerClient) =>
{
    int numOfEvents = 3;

    // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
    // of the application, which is best practice when events are being published or read regularly.
    // TODO: Replace the <CONNECTION_STRING> and <HUB_NAME> placeholder values

    // Create a batch of events
    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

    for (int i = 1; i <= numOfEvents; i++)
    {
        if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}"))))
        {
            // if it is too large for the batch
            throw new ArgumentException($"Event {i} is too large for the batch and cannot be sent.");
        }
    }

    try
    {
        // Use the producer client to send the batch of events to the event hub
        await producerClient.SendAsync(eventBatch);
        Console.WriteLine($"A batch of {numOfEvents} events has been published.");
    }
    finally
    {
        await producerClient.DisposeAsync();
    }
    return "Publisher Event Succeed";
});

await app.RunAsync();

