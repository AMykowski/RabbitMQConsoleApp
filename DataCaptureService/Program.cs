using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataCaptureService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\PdfFolder";

            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = "*.pdf";
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
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

                byte[] bytes = System.IO.File.ReadAllBytes(e.FullPath);
                channel.BasicPublish(exchange: "",
                    routingKey: "NewQueue",
                    basicProperties: null, bytes);
            }
        }
    }
}
