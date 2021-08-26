using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _03.DirectDemo
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {


            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(exchange: "ex.direct", type: "direct", durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.infos", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.warnings", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.errors", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "my.infos", exchange: "ex.direct", routingKey: "info");
            channel.QueueBind(queue: "my.warnings", exchange: "ex.direct", routingKey: "warning");
            channel.QueueBind(queue: "my.errors", exchange: "ex.direct", routingKey: "error");

            channel.BasicPublish(exchange: "ex.direct", routingKey: "info", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key info"));
            channel.BasicPublish(exchange: "ex.direct", routingKey: "warning", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key warning"));
            channel.BasicPublish(exchange: "ex.direct", routingKey: "error", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key error"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.infos");
            channel.QueueDelete(queue: "my.warnings");
            channel.QueueDelete(queue: "my.errors");

            channel.ExchangeDelete(exchange: "ex.direct");

            channel.Close();

            conn.Close();
        }
    }
}
