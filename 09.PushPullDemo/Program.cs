using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _09.PushPullDemo
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

            channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: "");

            //Send Messages from http://localhost:15672
            // readMessagesWithPushModel(); 
            readMessagesWithPullModel();

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();

            channel.QueueDelete(queue: "my.queue1");

            channel.ExchangeDelete(exchange: "ex.fanout");

            channel.Close();

            conn.Close();
        }

        private static void readMessagesWithPushModel()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Message:" + message);
            };
            string consumerTag = channel.BasicConsume(queue: "my.queue1", autoAck: true, consumer: consumer);

            Console.WriteLine("Subscribed. Press a key to unsubscribe and exit.");
            Console.ReadKey();

            channel.BasicCancel(consumerTag: consumerTag);
        }

        private static void readMessagesWithPullModel()
        {
            Console.WriteLine("Reading messages from queue. Press 'e' to exit.");

            while (true)
            {
                Console.WriteLine("Trying to get a message from the queue...");
                
                BasicGetResult result = channel.BasicGet(queue: "my.queue1", autoAck: true);

                if (result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine("Message:" + message);
                }

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.KeyChar == 'e' || keyInfo.KeyChar=='E')
                    {
                        return;
                    }

                }
                Thread.Sleep(2000);

            }
        }
    }
}