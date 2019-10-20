using System;
using Payments.Service.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Payments.Service.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        
        private const string ExchangeName = "Topic_Exchange";
        private const string PaymentQueueName = "PaymentTopic_Queue";

        public void CreateConnection()
        {
            _factory = new ConnectionFactory {
                HostName = "localhost",
                UserName = "guest", Password = "guest" };            
        }

        public void Close()
        {
            _connection.Close();
        }

        public void ProcessMessages()
        {
            using (_connection = _factory.CreateConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    Console.WriteLine("Listening for Topic <payment.cardpayment>");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine();

                    channel.ExchangeDeclare(ExchangeName, "topic");
                    channel.QueueDeclare(PaymentQueueName, 
                        true, false, false, null);

                    channel.QueueBind(PaymentQueueName, ExchangeName, 
                        "payment.cardpayment");

                    channel.BasicQos(0, 10, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = (Payment)ea.Body.DeSerialize(typeof(Payment));
                        Console.WriteLine("--- Payment - Routing Key <{0}> : {1} : {2}", ea.RoutingKey, message.CardNumber, message.Amount);
                    };
                    channel.BasicConsume(queue: PaymentQueueName,
                                        autoAck: true,
                                        consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");

                    Console.ReadLine();
                }
            }
        }
    }
}