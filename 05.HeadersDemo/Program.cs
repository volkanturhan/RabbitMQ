using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _05.HeadersDemo
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

            channel.ExchangeDeclare(exchange: "ex.headers", type: "headers", durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "my.queue1", exchange: "ex.headers", routingKey: "", new Dictionary<string, object>() { { "x-match", "all" }, { "job", "convert" }, { "format", "jpeg" } });
            channel.QueueBind(queue: "my.queue2", exchange: "ex.headers", routingKey: "", new Dictionary<string, object>() { { "x-match", "any" }, { "job", "convert" }, { "format", "jpeg" } });

            IBasicProperties props1 = channel.CreateBasicProperties();
            props1.Headers = new Dictionary<string, object>();
            props1.Headers.Add("job", "convert");
            props1.Headers.Add("format", "jpeg");

            channel.BasicPublish(exchange: "ex.headers", routingKey: "", basicProperties: props1, body: Encoding.UTF8.GetBytes("Message 1"));

            IBasicProperties props2 = channel.CreateBasicProperties();
            props2.Headers = new Dictionary<string, object>();
            props2.Headers.Add("job", "convert");
            props2.Headers.Add("format", "bitmap");

            channel.BasicPublish(exchange: "ex.headers", routingKey: "", basicProperties: props2, body: Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");
            channel.QueueDelete(queue: "my.queue2");

            channel.ExchangeDelete(exchange: "ex.headers");

            channel.Close();

            conn.Close();
        }
    }
}
