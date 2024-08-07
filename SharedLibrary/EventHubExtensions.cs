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
            var option = service.GetService<IOptions<StorageSettings>>()!.Value;
            var blobService = new BlobServiceClient(connectionString);

            //get a BlobContainerClient
            var container = blobService.GetBlobContainerClient(option.ContainerName);

            //you can check if the container exists or not, then determine to create it or not
            bool isExist = container.Exists();
            if (!isExist)
            {
                container.Create();
            }

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
