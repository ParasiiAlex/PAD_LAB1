using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using System;
using Lab2;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SubscriberService : Subscriber.SubscriberBase
    {
        private readonly IConnectionStorageService _connections;

        public SubscriberService(IConnectionStorageService connectionStorage)
        {
            _connections = connectionStorage;
        }

        public override Task<SubscribeReply> Subscribe(SubscribeRequest subscribeRequest, ServerCallContext context)
        {
            var connection = new Connection(subscribeRequest.Address, subscribeRequest.Topic, subscribeRequest.Limit);
            
            _connections.Add(connection);

            Console.WriteLine($"New client subscribed: {subscribeRequest.Address} | {subscribeRequest.Topic}");

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true
            });
        }
    }
}
