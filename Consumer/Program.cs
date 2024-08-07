using SharedLibrary.Configurations;
using SharedLibrary;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.EventHubs.Processor;
using System.Diagnostics;
using System.Text;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEventHubEventProcessorClientService(builder.Configuration.GetConnectionString("azureeventhub") ?? "");
builder.Services.AddStorageClientService(builder.Configuration.GetConnectionString("blobs") ?? "");
builder.Services.Configure<EventHubSettings>(builder.Configuration.GetSection("EventHub"));
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (EventProcessorClient processorClient) =>
{
    // Register handlers for processing events and handling errors
    processorClient.ProcessEventAsync += ProcessEventHandler;
    processorClient.ProcessErrorAsync += ProcessErrorHandler;

    // Start the processing
    await processorClient.StartProcessingAsync();

    // Wait for 30 seconds for the events to be processed
    await Task.Delay(TimeSpan.FromSeconds(30));

    // Stop the processing
    await processorClient.StopProcessingAsync();

    Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        // Write the body of the event to the console window
        Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        return Task.CompletedTask;
    }

    Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }
    return "Consume Event Succeed";
});

await app.RunAsync();
