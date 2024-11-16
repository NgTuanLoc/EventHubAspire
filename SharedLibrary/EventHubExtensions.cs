using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedLibrary.Configurations;
using static System.Reflection.Metadata.BlobBuilder;

namespace SharedLibrary;

public static class EventHubExtensions
{
    public static IServiceCollection AddEventHubProducerClientService(this IServiceCollection services, string connectionString)
    {
        services.AddScoped(service =>
        {
            var option = service.GetService<IOptions<EventHubSettings>>()!.Value;
            return new EventHubProducerClient(connectionString, option.HubName);
        });

        return services;
    }

    public static IServiceCollection AddStorageClientService(this IServiceCollection services, string connectionString)
    {
        services.AddScoped(service =>
        {
            var blobClient = service.GetRequiredService<BlobServiceClient>();
            var option = service.GetService<IOptions<StorageSettings>>()!.Value;
            blobClient.CreateBlobContainer(option.ContainerName);
            return new BlobContainerClient(connectionString, option.ContainerName);
        });
        return services;
    }

    public static IServiceCollection AddEventHubEventProcessorClientService(this IServiceCollection services, string connectionString)
    {
        services.AddScoped(service =>
        {
            var storageClient = service.GetService<BlobContainerClient>();
            var option = service.GetService<IOptions<EventHubSettings>>()!.Value;

            return new EventProcessorClient(storageClient, option.ConsumerGroup, connectionString, option.HubName);
        });

        return services;
    }

    public static IServiceCollection AddSchemaRegistry(this IServiceCollection services)
    {
        var schemaRegistryClient = new SchemaRegistryClient("ntloc-event-hub.servicebus.windows.net", new DefaultAzureCredential());

        services.AddScoped(_ => schemaRegistryClient);
        return services;
    }
}
