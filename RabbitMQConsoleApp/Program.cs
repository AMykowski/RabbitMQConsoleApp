using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "NewQueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, e) =>
                {
                    byte[] bytes = e.Body.ToArray();
                    System.IO.File.WriteAllBytes(@"D:\PdfFolderNew\transferedPdf.pdf", bytes);
                };

                channel.BasicConsume(queue: "NewQueue", autoAck: true, consumer: consumer);

            }

            Console.ReadLine();
        }
    }
}
