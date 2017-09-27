using System;
using System.Net;

namespace Simp.Rpc.Server
{
    public class ServerOptions
    { 
        private string version;
        private EndPoint endPoint;
        private int acceptThreads = Environment.ProcessorCount;
        private int workThreads = Environment.ProcessorCount * 2;
        private int minThreads = 10;
        private int maxThreads = 500;
        private int maxClients = 1000;
        private int taskQueueSize;
        private int connectTimeout = 10 * 1000;
        private int readTimeout = 30 * 1000;
        private int writeTimeout = 30 * 1000;
        private int receiveBufferSize = 1024 * 64;
        private int sendBufferSize = 1024 * 64;
        private bool keepAlive;
        private int keepAliveTime = 30 * 60;  // 单位秒, 默认 30 分钟
        private bool tcpNoDelay;
        private int linger = 5;

        public string Version
        {
            get => version;
            set => version = value;
        }

        public EndPoint EndPoint
        {
            get => endPoint;
            set => endPoint = value;
        }

        public int AcceptThreads
        {
            get => acceptThreads;
            set => acceptThreads = value;
        }

        public int WorkThreads
        {
            get => workThreads;
            set => workThreads = value;
        }

        public int MinThreads
        {
            get => minThreads;
            set => minThreads = value;
        }

        public int MaxThreads
        {
            get => maxThreads;
            set => maxThreads = value;
        }

        public int MaxClients
        {
            get => maxClients;
            set => maxClients = value;
        }

        public int TaskQueueSize
        {
            get => taskQueueSize;
            set => taskQueueSize = value;
        }

        public int ConnectTimeout
        {
            get => connectTimeout;
            set => connectTimeout = value;
        }

        public int ReadTimeout
        {
            get => readTimeout;
            set => readTimeout = value;
        }

        public int WriteTimeout
        {
            get => writeTimeout;
            set => writeTimeout = value;
        }

        public int ReceiveBufferSize
        {
            get => receiveBufferSize;
            set => receiveBufferSize = value;
        }

        public int SendBufferSize
        {
            get => sendBufferSize;
            set => sendBufferSize = value;
        }

        public bool KeepAlive
        {
            get => keepAlive;
            set => keepAlive = value;
        }

        public int KeepAliveTime
        {
            get => keepAliveTime;
            set => keepAliveTime = value;
        }

        public bool TcpNoDelay
        {
            get => tcpNoDelay;
            set => tcpNoDelay = value;
        }

        public int Linger
        {
            get => linger;
            set => linger = value;
        }


    }
}