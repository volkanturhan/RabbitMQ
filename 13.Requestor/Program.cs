using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _13.Requestor
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            //Console.Write("Enter the queue name : ");
            //string queueName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare(queue: "requests", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "responses", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false, arguments: null);

            //channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: "");
            //channel.QueueBind(queue: "my.queue2", exchange: "ex.fanout", routingKey: "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Response received:" + message);
            };

            channel.BasicConsume("responses", true, consumer);

            while (true)
            {
                Console.Write("Enter your request:");
                string request = Console.ReadLine();

                if (request == "exit")
                    break;

                channel.BasicPublish("", "requests", null, Encoding.UTF8.GetBytes(request));
            }

            channel.QueueDelete(queue: "requests");
            channel.QueueDelete(queue: "responses");


            channel.Close();

            conn.Close();
        }
    }
}
