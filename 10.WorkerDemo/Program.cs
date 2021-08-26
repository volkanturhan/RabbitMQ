using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _10.WorkerDemo
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            Console.Write("Enter the name for this worker : ");
            string workerName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare(queue: "queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"[{workerName}] Message: {message}");
            };

            var consumerTag = channel.BasicConsume(queue: "my.queue1", autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages. Press a to exit.");
            Console.ReadKey();


            channel.QueueDelete(queue: "my.queue1");

            channel.ExchangeDelete(exchange: "ex.fanout");

            channel.Close();

            conn.Close();
        }

    }
}