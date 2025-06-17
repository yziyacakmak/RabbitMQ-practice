using RabbitMQ.Client;
var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();

var queueName = "hello-queue";
await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

Enumerable.Range(1, 50).ToList().ForEach(async i =>
{
    var message = $"Message {i}";
    var body = System.Text.Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
    exchange: string.Empty,
    routingKey: queueName,
    mandatory: false,
    body: body);
    Console.WriteLine($" [x] Sent {message}");

});
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

