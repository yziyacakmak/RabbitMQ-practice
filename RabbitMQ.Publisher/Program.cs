using RabbitMQ.Client;
using RabbitMQ.Publisher;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "logs-direct",
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false
);

Enum.GetNames(typeof(LogNames)).ToList().ForEach(name =>
{
    var routeKey = $"route-{name}";
    var queueName = $"direct-queue-{name}";
    channel.QueueDeclareAsync(queueName, true, false,false);
    channel.QueueBindAsync(queueName, "logs-direct", routeKey,null);
});


Enumerable.Range(1, 50).ToList().ForEach(async i =>
{
    LogNames log = (LogNames)new Random().Next(0, 4);
    var message = $"Message {i}//{log}";
    var body = System.Text.Encoding.UTF8.GetBytes(message);

    var routeKey = $"route-{log}";

    await channel.BasicPublishAsync(
    exchange: "logs-direct",
    routingKey: routeKey,
    mandatory: false,
    body: body);
    Console.WriteLine($" [x] Sent {message}");

});
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

