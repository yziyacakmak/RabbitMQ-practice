using RabbitMQ.Client;
using RabbitMQ.Client.Events;
var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();

var queueName = "hello-queue";
//await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

var consumer=new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    await Task.CompletedTask;
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer
);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

