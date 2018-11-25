using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Collections.Generic;

namespace AutomationCore.EventBus
{
    public class RabbitMQClient : IEventBusClient
    {
        private readonly IAdvancedBus _bus;
        private Dictionary<string, IQueue> _queues;
        private Dictionary<string, IExchange> _exchanges;

        public RabbitMQClient()
        {
            _exchanges = new Dictionary<string, IExchange>();
            _queues = new Dictionary<string, IQueue>();
            _bus = RabbitHutch.CreateBus("host=localhost").Advanced;
        }
        public RabbitMQClient DeclareExchange(string exchangeName, string type)
        {
            var exchange = _bus.ExchangeDeclare(exchangeName, type);
            _exchanges.Add(exchangeName, exchange);
            return this;
        }

        public RabbitMQClient DeclareQueue(string queueName)
        {
            var queue = _bus.QueueDeclare(queueName);
            _queues.Add(queueName, queue);
            return this;
        }

        public object DeclareQueue(object queuename)
        {
            throw new NotImplementedException();
        }

        public RabbitMQClient BindQueue(string queueName, string exchangeName, string toppic = "")
        {
            var exchange = _exchanges[exchangeName];
            var queue = _queues[queueName];
            _bus.Bind(exchange, queue, toppic);
            return this;
        }

        public RabbitMQClient Consume<T>(string queueName, Action<IMessage<T>, MessageReceivedInfo> onMessage) where T : class
        {
            var queue = _queues[queueName];
            _bus.Consume(queue, onMessage);
            return this;
        }

        public void SendMessage<T>(string exchangeName, IMessage<T> message, string routingKey = "") where T : class
        {
            var exchange = _exchanges[exchangeName];
            _bus.Publish<T>(exchange, routingKey, false, message);
        } 
    }

    public interface IEventBusClient
    {
        RabbitMQClient DeclareExchange(string exchangeName, string type);
        RabbitMQClient DeclareQueue(string queueName);
        RabbitMQClient BindQueue(string queueName, string exchangeName, string toppic = "");
        RabbitMQClient Consume<T>(string queueName, Action<IMessage<T>, MessageReceivedInfo> onMessage) where T : class;
        void SendMessage<T>(string exchangeName, IMessage<T> message, string routingKey = "") where T : class;
    }
}
