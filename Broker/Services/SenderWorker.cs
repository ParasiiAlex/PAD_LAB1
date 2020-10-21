using Broker.Models;
using Broker.Services.Interfaces;
using Common;
using Grpc.Core;
using Lab2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SenderWorker : IHostedService
    {
        private Timer _timer;

        private readonly IMessageStorageService _messageStorage;
        private readonly IConnectionStorageService _connectionStorage;


        public SenderWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _messageStorage = scope.ServiceProvider.GetRequiredService<IMessageStorageService>();
                _connectionStorage = scope.ServiceProvider.GetRequiredService<IConnectionStorageService>();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, 0, Config.TIME_TO_WAIT_FOR_WORKER);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            Message message = _messageStorage.GetNext();
            while(message != null)
            {
                if (message != null)
                {
                    var connections = _connectionStorage.GetConnectionsByTopic(message.Topic);

                    foreach(var connection in connections)
                    {

                        var client = new Notifier.NotifierClient(connection.GrpcChannel);
                        var request = new NotifyRequest() { Content = message.Content };

                        try
                        {
                            if (connection.Limit <= 0)
                            {
                                client.Notify(new NotifyRequest() { Content = "Message limit is out. You are disconnected." });
                                _connectionStorage.Remove(connection.Address);
                                continue;
                            }

                            var reply = client.Notify(request);
                            _connectionStorage.UpdateLimit(connection.Address);
                            Console.WriteLine($"Notified subscriber {connection.Address} with {message.Content}. Response: {reply}");
                        } 
                        catch(RpcException re)
                        {
                            if (re.StatusCode == StatusCode.Internal)
                            {
                                _connectionStorage.Remove(connection.Address);
                            }
                            Console.WriteLine(re.Message);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error notifying subscriber {connection.Address}. {e.Message}");
                        }
                    }
                }

                message = _messageStorage.GetNext();
            }
        }
    }
}
