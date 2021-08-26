using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _02.FanoutConsumer
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            //AutoAch True ise,
            var consumerTag = channel.BasicConsume(queue: "my.queue1", autoAck: false, consumer: consumer);
            Console.WriteLine("Waiting for messages. Press any key to exit.");
            Console.ReadKey();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine("Message:" + message);

            //channel.BasicAck(e.DeliveryTag, false);
            channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: false);

        }
    }
}
