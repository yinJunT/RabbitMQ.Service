using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Work.Consumers
{
    public class Consumers
    {
        private IConnection _connection;
        private IModel _channel;
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public void ConsumeQueue()
        {
            // 消息队列url
            string _url = "amqps://okhsdetb:umaYY8LXCmgJ08lvbcT2ySqsh8wwrLlx@gerbil.rmq.cloudamqp.com/okhsdetb";

            // 创建连接并打开通道，完成后将其释放  
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_url)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 在访问队列之前，请确保队列存在 
            var queueName = "xtqueue";
            bool durable = false;
            bool exclusive = false;
            bool autoDelete = true;

            _channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            // prefetchCount = 1，这使用BasicQos协议方法告诉RabbitMQ一次不要给一个工人多个消息。
            // 实现work模式，多个消费者
            _channel.BasicQos(0,1,false);
            var consumer = new EventingBasicConsumer(_channel);

            // 添加消息接收事件
            consumer.Received += (model, deliveryEventArgs) =>
            {
                var body = deliveryEventArgs.Body.ToArray();

                // 将消息从字节[]转换回字符串  
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("** 收到的消息:{0}由消费者线程 **", message);

                // 否定信息(即否定信息)， 确认我们已经处理了  
                // 否则它稍后将被重新排队
                _channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
            };

            // 开始消费
            _ = _channel.BasicConsume(consumer, queueName);

            // 等待重置事件并在它触发时清除  
            _resetEvent.WaitOne();
            _channel?.Close();
            _channel = null;
            _connection?.Close();
            _connection = null;
        }
    }
}
