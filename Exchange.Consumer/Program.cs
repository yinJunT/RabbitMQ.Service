using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Channels;

namespace Exchange.Consumer
{
    // 消费者
    class Program
    {
        static void Main(string[] args)
        {
            IConnectionFactory factory = new ConnectionFactory
            {
                HostName = "192.168.2.49",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };

            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            // 交换机名称
            string exchangeName = "exchange1";
            // 声明交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            // 消息队列名称
            string queueName = DateTime.Now.Second.ToString();
            // 声明队列
            channel.QueueDeclare(queueName, false, false, false, null);
            // 将队列与交换机进行绑定
            channel.QueueBind(queueName, exchangeName, "", null);
            // 定义消费者
            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine($"队列名称:{queueName}");
            // 接收事件
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body.ToArray(); ;
                // 将消息从字节[]转换回字符串  
                Console.WriteLine($"接收到信息为:{Encoding.UTF8.GetString(message)}");
                // 返回消息确认
                channel.BasicAck(ea.DeliveryTag, true);
            };
               // 开启监听
            channel.BasicConsume(queueName, false, consumer);
            Console.ReadKey();
        }
    }
}
