using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace _11.WorkQueuesDemo
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            Console.Write("Enter the name for this worker : ");
            string workerName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            //Eğer Workerda bir tamamlanmamış bir iş varsa o worker'a iş göndermez
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            channel.QueueDeclare(queue: "my.queue1", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange: "ex.fanout", type: "fanout", durable: true, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "my.queue1", exchange: "ex.fanout", routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                int durationInSeconds = Int32.Parse(message);
                Console.WriteLine($"[{workerName}] Task Started. Durastion: {durationInSeconds}");

                Thread.Sleep(durationInSeconds * 1000);

                Console.WriteLine("FINISHED");

                //autoAck False olduğu için Ack işlemini, işlem tamamlandığında burada işletiyoruz.

                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            };
            //autoAck true olduğunda,kuyruktaki bir task bitene kadar diğerlerini işleme almaz, false olursa ??aslında böyle değil araştır 
            var consumerTag = channel.BasicConsume(queue: "my.queue1", autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. Press a to exit.");
            Console.ReadKey();


            channel.QueueDelete(queue: "my.queue1");

            channel.ExchangeDelete(exchange: "ex.fanout");

            channel.Close();

            conn.Close();
        }


    }
}
