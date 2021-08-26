using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Topic.Consumer
{
    // 主题模式中的消费这可以使用通配符进行匹配key
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"输入接受key名称:");
            string routeKey = Console.ReadLine();   // 输入key.* 

            IConnectionFactory factory = new ConnectionFactory      
            {
                HostName = "192.168.2.49",                       
                Port = 5672,                                        
                UserName = "admin",                               
                Password = "admin"                             
            };

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            // 交换机名称
            string exchangeName = "exchange3";

            // 声明交换机并且为主题模式
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            string queueName = DateTime.Now.Second.ToString();

            channel.QueueDeclare(queueName, false, false, false, null);

            // 将队列与交换机进行绑定
            channel.QueueBind(queueName, exchangeName, routeKey, null);

            //定义消费者
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
