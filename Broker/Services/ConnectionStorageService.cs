using Broker.Models;
using Broker.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Broker.Services
{
    public class ConnectionStorageService : IConnectionStorageService
    {
        private readonly List<Connection> _connections;
        private readonly object _locker;

        public ConnectionStorageService()
        {
            _connections = new List<Connection>();
            _locker = new object();
        }

        public void Add(Connection connection)
        {
            lock(_locker)
            {
                _connections.Add(connection);
            }
        }

        public IList<Connection> GetConnectionsByTopic(string topic)
        {
            lock(_locker)
            {
                var filteredConnection = _connections.Where(x => x.Topic == topic).ToList();
                return filteredConnection;
            }
        }

        public void Remove(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(x => x.Address == address);
            }
        }

        public void UpdateLimit(string address)
        {
            lock (_locker)
            {
                var index = _connections.FindIndex(x => x.Address == address);
                _connections[index].Limit--;
            }
        }
    }
}
