using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

// Standalone consumer that catches order events published by the API and logs them.
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
var topic = configuration["Kafka:Topic"] ?? "orders";
var groupId = configuration["Kafka:GroupId"] ?? "billing-service";

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true
};

using var consumer = new ConsumerBuilder<string, string>(config).Build();
consumer.Subscribe(topic);

Console.WriteLine($"Billing service consumer started. Listening to topic '{topic}' on {bootstrapServers}...");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    while (!cts.IsCancellationRequested)
    {
        try
        {
            var result = consumer.Consume(cts.Token);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Received order event " +
                $"(key={result.Message.Key}, partition={result.Partition.Value}, offset={result.Offset.Value}):");
            Console.WriteLine(result.Message.Value);
        }
        catch (ConsumeException ex)
        {
            Console.WriteLine($"Consume error: {ex.Error.Reason}");
        }
    }
}
catch (OperationCanceledException)
{
    // Graceful shutdown requested via Ctrl+C.
}
finally
{
    consumer.Close();
    Console.WriteLine("Consumer closed.");
}
