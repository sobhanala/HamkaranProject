# Producerâ€“Consumer System

A C#/.NET 8 message-processing example with an ASP.NET Core broker API and separate producer and consumer console applications. Plugins are loaded from DLLs through shared interfaces.

For the original Persian project explanation, see [README.fa.md](README.fa.md).

## Components

- `hamkaran project/`: broker API, message queue, and controllers.
- `producer/`: loads the producer plugin and sends messages to the broker.
- `consumer/`: loads the consumer plugin and processes received messages.
- `ProdducerDll/`, `ConsumerDll/`: example plugin implementations.
- `IProduce/`, `IConsumer/`: plugin contracts and configuration attributes.
- `Logger/`: shared logging implementation.

## Build and run

Install the .NET 8 SDK and build the solution from the repository root:

```sh
dotnet build "hamkaran project.sln"
dotnet dev-certs https --trust
```

Start these commands in three separate terminals, each from the repository root:

```sh
dotnet run --project "hamkaran project/hamkaran project.csproj"
dotnet run --project producer/producer.csproj
dotnet run --project consumer/consumer.csproj
```

The loaders use `https://localhost:5000/api/produce` and `https://localhost:5000/api/consume`. Build in the default Debug configuration: the example loaders expect the plugin DLLs under `bin/Debug/net8.0`.

## Extending the example

Implement the producer or consumer interface in a new class library and configure it with the corresponding attribute. Update the loader's DLL path when using a different plugin or build configuration.

The producer uses an in-memory queue for retrying failed sends. Restarting the process can lose that queued state; this example does not establish durable or exactly-once delivery guarantees. Review the broker and consumer implementations for their storage behavior.
