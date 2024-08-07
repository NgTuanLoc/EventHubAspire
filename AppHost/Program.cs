var builder = DistributedApplication.CreateBuilder(args);

// External Resources
var eventHubs = builder.AddAzureEventHubs("azureeventhub")
                        .RunAsEmulator()
                        .AddEventHub("my-event-hub");

var blobs = builder.AddAzureStorage("storage")
                        .RunAsEmulator()
                        .WithAnnotation(new ContainerImageAnnotation
                        {
                            Registry = "mcr.microsoft.com",
                            Image = "azure-storage/azurite",
                            Tag = "3.31.0"
                        })
                        .AddBlobs("blobs");

builder.AddProject<Projects.Publisher>("publisher")
        .WithReference(eventHubs);

builder.AddProject<Projects.Consumer>("consumer")
        .WithReference(eventHubs)
        .WithReference(blobs);

await builder.Build().RunAsync();