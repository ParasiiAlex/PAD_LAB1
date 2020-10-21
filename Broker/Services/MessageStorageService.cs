using Broker.Models;
using Broker.Services.Interfaces;
using DiskQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;

namespace Broker.Services
{
    public class MessageStorageService : IMessageStorageService
    {
        //private readonly ConcurrentQueue<Message> _messages;
        private IPersistentQueue _diskQueue;

        public MessageStorageService()
        {
            //_messages = new ConcurrentQueue<Message>();
            _diskQueue = new PersistentQueue("messagesQueue");
        }

        public void Add(Message message)
        {
            //_messages.Enqueue(message);
            using (var session = _diskQueue.OpenSession())
            {
                var byteMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                session.Enqueue(byteMessage);
                session.Flush();
            }

        }

        public Message GetNext()
        {
            Message message;
            //_messages.TryDequeue(out message);
            using (var session = _diskQueue.OpenSession())
            {
                var byteMessage = session.Dequeue();
                session.Flush();
                if (byteMessage == null)
                {
                    return null;
                }
                message = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(byteMessage));
            }
            return message;
        }

        //public bool IsEmpty()
        //{
         //   _diskQueue.
          //  return _messages.IsEmpty;
        //}
    }
}
