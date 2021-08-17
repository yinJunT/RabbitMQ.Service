using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("生产者");

            // 1、创建连接工厂对象配置
            IConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "192.168.2.49",      // ip地址
                Port = 5672,                   // 端口号
                UserName = "admin",             // 用户名
                Password = "admin"              // 密码
            };

            // 2、创建连接对象
            IConnection connection = factory.CreateConnection();

            // 3、创建连接回话对象
            IModel channel = connection.CreateModel();

            string name = "test";
            // 4、声明一个队列
            channel.QueueDeclare(
                queue: name,                    // 消息队列的名称
                durable: false,                 // 是否持久化,true持久化队列会保存磁盘,服务器重启时可以保证不丢失相关信息
                exclusive: false,         
                autoDelete: false,              
                arguments: null              
                );
            string str = string.Empty;
            do
            {
                Console.WriteLine("发送内容");
                str = Console.ReadLine();

                // 5、消息内容
                byte[] body = Encoding.UTF8.GetBytes(str);
                channel.BasicPublish("", name, null, body);

                // 6、发送消息
                Console.WriteLine("成功发送消息：" + str);

            } while (str.Trim().ToLower() != "exit");

            connection.Close();
            channel.Close();
        }


    }
}
