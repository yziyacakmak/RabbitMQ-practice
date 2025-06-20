﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();


var queueName = "direct-queue-Critical";

await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false
);

var consumer=new AsyncEventingBasicConsumer(channel);
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer
);

consumer.ReceivedAsync += async (model, ea) =>
{
    await Task.Delay(1000); // Simulate some processing delay
    var body = ea.Body.ToArray();
    var message = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    await channel.BasicAckAsync(
        deliveryTag: ea.DeliveryTag,
        multiple: false
    );
    await Task.CompletedTask;
};
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

