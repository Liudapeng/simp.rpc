namespace Simp.Rpc.Client
{
    public class ClientOptions
    { 
        private string name;
        private int workThreads;
        private bool reuseAddress;
        private int connectTimeout = 10 * 1000;
        private int acquireTimeout = 10 * 1000;
        private int readTimeout = 500 * 1000;
        private int writeTimeout = 5 * 1000;
        private int maxConnections = 500;
        private int maxPendingAcquires = 100;
        private int receiveBufferSize = 1024 * 64;
        private int sendBufferSize = 1024 * 64;
        private bool tcpNoDelay;
        private bool keepAlive;
        private int keepAliveTime = 30 * 60;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int WorkThreads
        {
            get => workThreads;
            set => workThreads = value;
        }

        public bool ReuseAddress
        {
            get => reuseAddress;
            set => reuseAddress = value;
        }

        public int ConnectTimeout
        {
            get => connectTimeout;
            set => connectTimeout = value;
        }

        public int AcquireTimeout
        {
            get => acquireTimeout;
            set => acquireTimeout = value;
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

        public int MaxConnections
        {
            get => maxConnections;
            set => maxConnections = value;
        }

        public int MaxPendingAcquires
        {
            get => maxPendingAcquires;
            set => maxPendingAcquires = value;
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

        public bool TcpNoDelay
        {
            get => tcpNoDelay;
            set => tcpNoDelay = value;
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
 
    }
}