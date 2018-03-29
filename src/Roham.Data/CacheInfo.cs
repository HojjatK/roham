namespace Roham.Data
{
    public class CacheInfo
    {
        public CacheInfo(CacheProviders provider)
        {
            Provider = provider;
        }

        public CacheInfo(
            CacheProviders provider,
            string host,
            int port,
            bool ssl = false,
            string password = null,
            int? db = null,
            string client = null,
            int? connectTimeout = null,
            int? sendTimeout = null,
            int? receiveTimeout = null,
            int? idleTimeOutSecs = null,
            string namespacePrefix = null) : this(provider)
        {
            Host = host;
            Port = port;
            Ssl = ssl;
            Password = password;
            Db = db;
            Client = client;
            ConnectTimeout = connectTimeout;
            SendTimeout = sendTimeout;
            ReceiveTimeout = receiveTimeout;
            IdleTimeOutSecs = idleTimeOutSecs;
            NamespacePrefix = namespacePrefix;
        }

        public CacheProviders Provider { get; }

        /// <summary>
        /// Connection hostname or IP address 
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Connection Port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Indicates if this is an SSL connection
        /// </summary>
        public bool Ssl { get; }

        /// <summary>
        /// UrlEncoded version of the Password 
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// The DB this connection should be set to
        /// </summary>
        public int? Db { get; }

        /// <summary>
        /// A text alias to specify for this connection for analytic purposes
        /// </summary>
        public string Client { get; }

        /// <summary>
        /// Timeout in ms for making a TCP Socket connection
        /// </summary>
        public int? ConnectTimeout { get; }

        /// <summary>
        /// Timeout in ms for making a synchronous TCP Socket Send
        /// </summary>
        public int? SendTimeout { get; }

        /// <summary>
        /// Timeout in ms for waiting for a synchronous TCP Socket Receive
        /// </summary>
        public int? ReceiveTimeout { get; }

        /// <summary>
        /// Timeout in Seconds for an Idle connection to be considered active
        /// </summary>
        public int? IdleTimeOutSecs { get; }

        /// <summary>
        /// Use a custom prefix for ServiceStack.Redis internal index colletions
        /// </summary>
        public string NamespacePrefix { get; }

    }
}
