using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Models
{
    public class Connection
    {
        public Connection(string address, string topic, string limit)
        {
            Address = address;
            Topic = topic;
            int t_limit = 0; 
            int.TryParse(limit, out t_limit);
            Limit = t_limit;
            GrpcChannel = GrpcChannel.ForAddress(address);
        }

        public string Address { get; }
        public string Topic { get; }
        public int Limit { get; set; }
        public GrpcChannel GrpcChannel { get; }
    }
}
