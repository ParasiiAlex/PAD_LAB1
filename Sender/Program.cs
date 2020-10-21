using Common;
using Grpc.Net.Client;
using Lab2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Publisher");


            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(Config.BrokerAddress);
            var client = new Publisher.PublisherClient(channel);

            while (true)
            {
                Console.Write("Enter the topic:");
                var message = Console.ReadLine().ToLower();

                Console.Write("Enter the content: ");
                var content = Console.ReadLine();

                var request = new PublishRequest() { Topic = message, Content = content };

                try
                {
                    var reply = await client.PublishMessageAsync(request);
                    Console.WriteLine($"Publsih Reply: {reply.IsSuccess}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error publishing the message: {e.Message}");
                }
            }
        }
    }
}
