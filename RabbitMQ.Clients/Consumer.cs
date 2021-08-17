using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Clients
{
    class Consumer
    {
        private IConnection _connection;
        private IModel _channel;
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);
        public void ConsumeQueue()
        {
            Console.WriteLine("消费者");
            // 1、创建连接工厂对象配置
            IConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "192.168.2.49",      // ip地址
                Port = 5672,                   // 端口号
                UserName = "admin",             // 用户名
                Password = "admin"              // 密码
            };

            Console.CancelKeyPress += (sender, eArgs) =>
            {
                // 设置退出事件，以便消费者将接收它并优雅地退出  
                _resetEvent.Set();
                Console.WriteLine("CancelEvent收到，关闭…");
                // 睡眠1秒，让消费者有时间清理  
                Thread.Sleep(1000);
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 在访问队列之前，请确保队列存在（声明消息队列）
            var queueName = "demo";     // 消息队列名称
            bool durable = false;       // 是否持久化,true持久化,队列会保存磁盘,服务器重启时可以保证不丢失相关信息。
            bool exclusive = false;     // 是否排他,true排他的,如果一个队列声明为排他队列,该队列仅对首次声明它的连接可见,并在连接                             断开时自动删除
            bool autoDelete = false;    // 是否自动删除。true是自动删除。自动删除的前提是：致少有一个消费者连接到这个队列，之后所                              有与这个队列连接的消费者都断开时,才会自动删除

            _channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

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

            // 消费者开启监听
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
