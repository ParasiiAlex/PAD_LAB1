using System;

namespace Common
{
    public class Config
    {
        public const string BrokerAddress = "http://127.0.0.1:5050";
        public const string SubscriberAddres = "http://127.0.0.1:0";

        public const int TIME_TO_WAIT_FOR_WORKER = 2000;
    }
}
