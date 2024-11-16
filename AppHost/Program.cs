var builder = DistributedApplication.CreateBuilder(args);

// External Resources
var eventHubs = builder.AddAzureEventHubs("azureeventhub")
                        .RunAsEmulator()
                        .AddEventHub("my-event-hub");

var blobs = builder.AddAzureStorage("storage")
                        .RunAsEmulator()
                        .AddBlobs("blobs");

builder.AddProject<Projects.Publisher>("publisher")
        .WithReference(eventHubs).WaitFor(eventHubs);

builder.AddProject<Projects.Consumer>("consumer")
        .WithReference(eventHubs).WaitFor(eventHubs)
        .WithReference(blobs).WaitFor(blobs);

await builder.Build().RunAsync();