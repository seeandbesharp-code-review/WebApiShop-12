namespace Services
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string key, string message);
    }
}
