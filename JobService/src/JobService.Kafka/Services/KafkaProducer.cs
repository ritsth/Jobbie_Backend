using Confluent.Kafka;
using System.Text.Json;

namespace JobService.Kafka.Services
{
    /// <summary>
    /// KafkaProducer responsible for sending job-related messages (Create, Update, Delete) to a given topic.
    /// </summary>
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All // Ensures delivery guarantee
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = topic;
        }

        /// <summary>
        /// Sends a message to the Kafka topic.
        /// </summary>
        /// <typeparam name="T">The type of the message payload.</typeparam>
        /// <param name="key">The key for partitioning the message.</param>
        /// <param name="message">The message payload.</param>
        public async Task SendMessageAsync<T>(string key, T message)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                await _producer.ProduceAsync(_topic, new Message<string, string>
                {
                    Key = key,
                    Value = jsonMessage
                });
                Console.WriteLine($"Message sent to topic {_topic}: {jsonMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to Kafka: {ex.Message}");
                throw;
            }
        }
    }
}
