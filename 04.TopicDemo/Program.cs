using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _04.TopicDemo
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

            channel.ExchangeDeclare(exchange: "ex.topic", type: "topic", durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue3", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "my.queue1", exchange: "ex.topic", routingKey: "*.image.*");
            channel.QueueBind(queue: "my.queue2", exchange: "ex.topic", routingKey: "#.image");
            channel.QueueBind(queue: "my.queue3", exchange: "ex.topic", routingKey: "image.#");

            channel.BasicPublish(exchange: "ex.topic", routingKey: "convert.image.bmp", basicProperties: null, body: Encoding.UTF8.GetBytes("Roting key is convert.image.bmp"));
            channel.BasicPublish(exchange: "ex.topic", routingKey: "convert.bitmap.image", basicProperties: null, body: Encoding.UTF8.GetBytes("Roting key is convert.bitmap.image"));
            channel.BasicPublish(exchange: "ex.topic", routingKey: "image.bitmap.32bit", basicProperties: null, body: Encoding.UTF8.GetBytes("Roting key is image.bitmap.32bit"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");
            channel.QueueDelete(queue: "my.queue2");
            channel.QueueDelete(queue: "my.queue3");

            channel.ExchangeDelete(exchange: "ex.topic");

            channel.Close();

            conn.Close();
        }
    }
}

