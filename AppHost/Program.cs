var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("eventHubsConnectionName")
                        .RunAsEmulator()
                        .AddEventHub("MyHub");

//var exampleService = builder.AddProject<Projects.ExampleService>()
//                            .WithReference(eventHubs);

await builder.Build().RunAsync();