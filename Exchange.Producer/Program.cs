using RabbitMQ.Client;
using System;
using System.Text;
using System.Xml.Linq;

namespace Exchange.Producer
{
    // 发布订阅模式(fanout)
    class Program
    {
        // 生产者实现，把队列替换成了交换机，
        // 发布消息时把交换机名称告诉RabbitMQ，把交换机设置成fanout发布模式
        static void Main(string[] args)
        {
            Console.WriteLine("生产者");
            IConnectionFactory factory = new ConnectionFactory 
            {
                HostName = "192.168.2.49", 
                Port = 5672,        
                UserName = "admin",
                Password = "admin"
            };

            IConnection connection = factory.CreateConnection();   // 创建连接对象
            IModel channel = connection.CreateModel();             // 创建连接会话对象
            string exchangeName = "exchange1";                     // 交换机名称

            // 把交换机设置成fanout发布订阅模式
            channel.ExchangeDeclare(exchangeName, type: "fanout");
            string str = string.Empty;
            do
            {
                Console.WriteLine("发送内容");
                str = Console.ReadLine();

                // 5、消息内容
                byte[] body = Encoding.UTF8.GetBytes(str);
                channel.BasicPublish(exchangeName, "", null, body);

                // 6、发送消息
                Console.WriteLine("成功发送消息：" + str);

            } while (str.Trim().ToLower() != "exit");

            connection.Close();
            channel.Close();
        }
    }
}
