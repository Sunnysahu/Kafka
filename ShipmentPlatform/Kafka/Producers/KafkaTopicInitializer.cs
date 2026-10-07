using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace ShipmentService.Kafka.Producers;

public class KafkaTopicInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaTopicInitializer> _logger;

    public KafkaTopicInitializer(IConfiguration configuration, ILogger<KafkaTopicInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];

        var topic = _configuration["Kafka:Topic"];

        if (string.IsNullOrWhiteSpace(bootstrapServers))
            throw new InvalidOperationException("Kafka bootstrap server is not configured.");

        if (string.IsNullOrWhiteSpace(topic))
            throw new InvalidOperationException("Kafka topic is not configured.");

        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        try
        {
            await adminClient.CreateTopicsAsync(
                new[]
                {
                    new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = 3,
                        ReplicationFactor = 1
                    }
                });

            _logger.LogInformation("Kafka topic created: {Topic}", topic);
        }
        catch (CreateTopicsException ex)
        {
            if (ex.Results.Any(x => x.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                _logger.LogInformation("Kafka topic already exists: {Topic}", topic);

                return;
            }

            throw;
        }
    }
}