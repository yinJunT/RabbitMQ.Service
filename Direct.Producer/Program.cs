using RabbitMQ.Client;
using System;
using System.Text;

namespace Direct.Producer
{
    // 路由模式生产者
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
            string exchangeName = "exchange2";                       // 4、交换机名称
            string routeKey = "key1";                                // 5、匹配的key

            // 申明一个routeKey值为key1,并在发布消息的时候告诉了RabbitMQ,
            // 消息传递时routeKey必须匹配,才会被队列接收否则消息会被抛弃

            // 把交换机设置成Direct模式
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            string str ;
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
