using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using Lab2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private readonly IMessageStorageService _messageStorage;

        public PublisherService(IMessageStorageService messageStorageService)
        {
            _messageStorage = messageStorageService;
        }

        public override Task<PublishReply> PublishMessage(PublishRequest publishRequest, ServerCallContext context)
        {
            Console.WriteLine($"Received: {publishRequest.Topic} | {publishRequest.Content}");

            var message = new Message(publishRequest.Topic, publishRequest.Content);
            _messageStorage.Add(message);

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
