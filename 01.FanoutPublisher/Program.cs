using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _01.FanoutPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false, arguments: null);

            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);


            channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: "");
            channel.QueueBind(queue: "my.queue2", exchange: "ex.fanout", routingKey: "");

            channel.BasicPublish(exchange: "ex.fanout", routingKey: "", basicProperties: null, body: Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish(exchange: "ex.fanout", routingKey: "", basicProperties: null, body: Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");
            channel.QueueDelete(queue: "my.queue2");

            channel.ExchangeDelete(exchange: "ex.fanout");

            channel.Close();

            conn.Close();
        }
    }
}
