using Confluent.Kafka;
using System.Text.Json;

namespace JobService.Kafka.Services
{
    /// <summary>
    /// KafkaProducer responsible for sending messages (Approved, Updated, Deleted) to a given topic.
    /// </summary>
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = topic;
        }

        public async Task SendMessageAsync<T>(string key, T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = key,
                Value = jsonMessage
            });
        }
    }
}
