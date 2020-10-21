using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Lab2;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Config.SubscriberAddres)
                .Build();

            host.Start();

            Subscribe(host);


            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void Subscribe(IWebHost host)
        {
            var channel = GrpcChannel.ForAddress(Config.BrokerAddress);
            var client = new Subscriber.SubscriberClient(channel);

            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            Console.WriteLine($"Subscriber listening at: {address}");

            Console.WriteLine("Enter the topic: ");
            var topic = Console.ReadLine().ToLower();


            var request = new SubscribeRequest() { Topic = topic, Address = address, Limit = "5" };

            try
            {
                var reply = client.Subscribe(request);
                Console.WriteLine($"Subscribe reply: {reply.IsSuccess}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error subscribing: {e.Message}");
            }
        }
    }
}
