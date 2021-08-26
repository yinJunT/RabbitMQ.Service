using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Direct.Consumer
{
    // 消费者
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"输入接受key名称:");
            string routeKey = Console.ReadLine();
            IConnectionFactory factory = new ConnectionFactory  //  创建连接工厂对象
            {
                HostName = "192.168.2.49",          // IP地址
                Port = 5672,                        // 端口号
                UserName = "admin",                 // 用户账号
                Password = "admin"                  // 用户密码
            };
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            // 交换机名称
            string exchangeName = "exchange2";

            // 声明交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            // 消息队列名称
            string queueName = DateTime.Now.Second.ToString();

            // 声明队列
            channel.QueueDeclare(queueName, false, false, false, null);

            // 将队列,key与交换机进行绑定
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            // 定义消费者
            var consumer = new EventingBasicConsumer(channel);
            Console.WriteLine($"队列名称:{queueName}");
            //接收事件
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body.ToArray();//接收到的消息
                Console.WriteLine($"接收到信息为:{Encoding.UTF8.GetString(message)}");
                //返回消息确认
                channel.BasicAck(ea.DeliveryTag, true);
            };
            //开启监听
            channel.BasicConsume(queueName, false, consumer);
            Console.ReadKey();
        }
    }
}
