using System;

namespace RabbitMQ.Work.Consumers
{
    class Program
    {
        static void Main(string[] args)
        {
            Consumers consumer = new Consumers();
            consumer.ConsumeQueue();
        }
    }
}
