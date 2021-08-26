using RabbitMQ.Client;
using System;
using System.Text;

namespace Topic.Producer
{
    // 主题模式（生产者）
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("生产者");
            IConnectionFactory factory = new ConnectionFactory      // 1、创建连接工厂对象
            {
                HostName = "192.168.2.49",                          // IP
                Port = 5672,                                        // 端口号
                UserName = "admin",                                 // 用户账号
                Password = "admin"                                  // 用户账号
            };

            IConnection connection = factory.CreateConnection();     // 2、创建连接对象
            IModel channel = connection.CreateModel();               // 3、创建连接回话对象
            string exchangeName = "exchange3";                       // 4、交换机名称
            string routeKey = "key.a";                                // 5、匹配的key


            // 把交换机设置成Topic模式
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
            string str;
            do
            {
                Console.Write("发送内容：");
                str = Console.ReadLine();

                //消息内容
                byte[] body = Encoding.UTF8.GetBytes(str);

                //发送消息
                channel.BasicPublish(exchangeName, routeKey, null, body);

            } while (str.Trim().ToLower() != "exit");
            connection.Close();
            channel.Close();
        }
    }
}
