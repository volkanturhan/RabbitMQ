using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace _06.DefaultDemo
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

            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "my.queue2", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.BasicPublish(exchange: "", routingKey: "my.queue1", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key my.queue1"));
            channel.BasicPublish(exchange: "", routingKey: "my.queue2", basicProperties: null, body: Encoding.UTF8.GetBytes("Message with routing key my.queue2"));

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");
            channel.QueueDelete(queue: "my.queue2");

            channel.Close();

            conn.Close();
        }
    }
}
