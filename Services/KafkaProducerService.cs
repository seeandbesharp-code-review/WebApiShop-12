using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        readonly IProducer<string, string> _producer;
        readonly string _topic;
        readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            // Connection details come from app settings.
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            _topic = configuration["Kafka:Topic"] ?? "orders";
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync(string key, string message)
        {
            try
            {
                var result = await _producer.ProduceAsync(_topic, new Message<string, string>
                {
                    Key = key,
                    Value = message
                });
                _logger.LogInformation($"Produced message to {result.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError($"Failed to deliver message to Kafka: {ex.Error.Reason}");
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}
