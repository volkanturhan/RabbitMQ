using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _07.ExToExDemo
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

            channel.ExchangeDeclare(exchange: "exchange1", type: "direct", durable: true, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange: "exchange2", type: "direct", durable: true, autoDelete: false, arguments: null);

            channel.QueueDeclare(queue: "queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "queue1", exchange: "exchange1", routingKey: "key1");
            channel.QueueBind(queue: "queue2", exchange: "exchange2", routingKey: "key2");

            channel.ExchangeBind(destination: "exchange2", source: "exchange1", routingKey: "key2");

            channel.BasicPublish(exchange: "exchange1", routingKey: "key1", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key key1"));
            channel.BasicPublish(exchange: "exchange1", routingKey: "key2", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key key2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "queue1");
            channel.QueueDelete(queue: "queue2");

            channel.ExchangeDelete(exchange: "exchange1");
            channel.ExchangeDelete(exchange: "exchange2");


            channel.Close();

            conn.Close();
        }
    }
}