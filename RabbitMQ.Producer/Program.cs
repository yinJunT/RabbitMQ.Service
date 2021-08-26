using RabbitMQ.Client;
using System;
using System.Text;
namespace RabbitMQ.Producer
{
    // 非本地生产者
    class Program
    {
        // AMQP服务器的url应该类似于amqps://[username]:[password]@[instance]/[vhost]  
        private static readonly string _url = "amqps://okhsdetb:umaYY8LXCmgJ08lvbcT2ySqsh8wwrLlx@gerbil.rmq.cloudamqp.com/okhsdetb";

        static void Main(string[] args)
        {
            // 1、创建一个ConnectionFactory并将Uri设置为clouddamqp url  
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_url)
            };

            // 2、创建连接对象,并打开通道，完成后将其释放  
            using var connection = factory.CreateConnection();

            // 3、创建连接回话对象，完成后并释放
            using var channel = connection.CreateModel();

            // 4、在发布队列之前，请确保队列存在（创建队列）
            var queueName = "xtqueue";
            channel.QueueDeclare(
                    queue: queueName,          // 消息队列的名称
                    durable: false,            // 是否持久化,true持久化队列会保存磁盘,服务器重启时可以保证不丢失相关信息
                    exclusive: false,
                    autoDelete: true,
                    arguments: null
            );
            while (true)
            {
                Console.WriteLine("输入消息并按下返回键发布(按ctrl-c退出)");
                var message = Console.ReadLine();

                // 放在队列上的数据必须是字节数组  
                var data = Encoding.UTF8.GetBytes(message);

                // 以队列名称作为路由键发布到“默认交换器
                var exchangeName = "";
                var routingKey = queueName;
                channel.BasicPublish(exchangeName, routingKey, null, data);
                Console.WriteLine("发布的消息 {0}", message);
            }
        }
    }
}
