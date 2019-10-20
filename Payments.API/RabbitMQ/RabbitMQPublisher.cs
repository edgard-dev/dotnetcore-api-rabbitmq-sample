using System;
using Payments.API.Models;
using RabbitMQ.Client;

namespace Payments.API.RabbitMQ
{
    public class RabbitMQPublisher
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        private const string ExchangeName = "Topic_Exchange";
        private const string PaymentQueueName = "PaymentTopic_Queue";
        
        public RabbitMQPublisher()
        {
            CreateConnection();
        }

        private static void CreateConnection()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost", UserName = "guest", Password = "guest"
            };

            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(ExchangeName, "topic");

            _model.QueueDeclare(PaymentQueueName, true, false, false, null);            

            _model.QueueBind(PaymentQueueName, ExchangeName, "payment.cardpayment");            
        }

        public void Close()
        {
            _connection.Close();
        }

        public void SendPayment(Payment payment)
        {
            SendMessage(payment.Serialize(), "payment.cardpayment");
            Console.WriteLine(" Payment Sent {0}, ${1}", payment.CardNumber, 
                payment.Amount);
        }

        public void SendMessage(byte[] message, string routingKey)
        {
            _model.BasicPublish(ExchangeName, routingKey, null, message);
        }
    }
}