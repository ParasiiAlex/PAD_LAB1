using Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services.Interfaces
{
    public interface IConnectionStorageService
    {
        public void Add(Connection connection);

        public void Remove(string address);

        public IList<Connection> GetConnectionsByTopic(string topic);

        public void UpdateLimit(string address);
    }
}
