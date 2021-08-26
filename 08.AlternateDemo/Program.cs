using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _08.AlternateDemo
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

            channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange: "ex.direct", type: "direct", durable: true, autoDelete: false, arguments: new Dictionary<string, object>() { { "alternate-exchange","ex.fanout"} });

            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.unrouted", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "my.queue1", exchange: "ex.direct", routingKey: "video");
            channel.QueueBind(queue: "my.queue2", exchange: "ex.direct", routingKey: "image");
            channel.QueueBind(queue: "my.unrouted", exchange: "ex.fanout", routingKey: "");


            channel.BasicPublish(exchange: "ex.direct", routingKey: "video", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key video"));
            channel.BasicPublish(exchange: "ex.direct", routingKey: "text", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key text"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");
            channel.QueueDelete(queue: "my.queue2");
            channel.QueueDelete(queue: "my.unrouted");

            channel.ExchangeDelete(exchange: "ex.direct");
            channel.ExchangeDelete(exchange: "ex.fanout");


            channel.Close();

            conn.Close();
        }
    }
}