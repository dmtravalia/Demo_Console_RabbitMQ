using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsoleApp_RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.ConfirmSelect();

                    channel.BasicAcks += Channel_BasicAcks;
                    channel.BasicNacks += Channel_BasicNacks;
                    channel.BasicReturn += Channel_BasicReturn;

                    channel.QueueDeclare(queue: "order",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = $"{DateTime.UtcNow:o} -> Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "orderssssss",
                                         basicProperties: null,
                                         body: body,
                                         mandatory: true);

                    channel.WaitForConfirms(new TimeSpan(0, 0, 5));

                    Console.WriteLine(" [x] Sent {0}", message);
                }
                catch (Exception ex)
                {
                    //Tratar o erro
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void Channel_BasicReturn(object sender, BasicReturnEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Console.WriteLine($"{DateTime.UtcNow:o} -> Basic Return -> Original message -> {message}");
        }

        private static void Channel_BasicNacks(object sender, BasicNackEventArgs e)
        {
            Console.WriteLine($"{DateTime.UtcNow:o} -> Basic Nack");
        }

        private static void Channel_BasicAcks(object sender, BasicAckEventArgs e)
        {
            Console.WriteLine($"{DateTime.UtcNow:o} -> Basic Ack");
        }
    }
}
