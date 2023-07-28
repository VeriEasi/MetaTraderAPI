using System;
using System.Collections;
using System.ServiceModel;
using System.Collections.Generic;
using log4net;

namespace MTAPIService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MTClient : IMTAPICallback, IDisposable
    {
        private const string ServiceName = "MTAPIService";

        public delegate void MTQuoteHandler(MTQuote quote);
        public delegate void MTEventHandler(MTEvent e);

        #region Fields
        private static readonly ILog Log = LogManager.GetLogger(typeof(MTClient));

        private readonly MTAPIProxy _proxy;
        #endregion

        #region ctor
        public MTClient(string host, int port)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host), "host is null or empty");

            if (port < 0 || port > 65536)
                throw new ArgumentOutOfRangeException(nameof(port), "port value is invalid");

            Host = host;
            Port = port;

            var urlService = $"net.tcp://{host}:{port}/{ServiceName}";

            var bind = new NetTcpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxBufferPoolSize = 2147483647,
                SendTimeout = new TimeSpan(12, 0, 0),
                ReceiveTimeout = new TimeSpan(12, 0, 0),
                ReaderQuotas =
                {
                    MaxArrayLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxDepth = 2147483647,
                    MaxStringContentLength = 2147483647,
                    MaxNameTableCharCount = 2147483647
                }
            };

            _proxy = new MTAPIProxy(new InstanceContext(this), bind, new EndpointAddress(urlService));
            _proxy.Faulted += ProxyFaulted;
        }

        public MTClient(int port)
        {
            if (port < 0 || port > 65536)
                throw new ArgumentOutOfRangeException(nameof(port), "port value is invalid");

            Port = port;

            var urlService = $"net.pipe://localhost/{ServiceName}_{port}";

            var bind = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxBufferPoolSize = 2147483647,
                SendTimeout = new TimeSpan(12, 0, 0),
                ReceiveTimeout = new TimeSpan(12, 0, 0),
                ReaderQuotas =
                {
                    MaxArrayLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxDepth = 2147483647,
                    MaxStringContentLength = 2147483647,
                    MaxNameTableCharCount = 2147483647
                }
            };

            _proxy = new MTAPIProxy(new InstanceContext(this), bind, new EndpointAddress(urlService));
            _proxy.Faulted += ProxyFaulted;
        }


        #endregion

        #region Public Methods
        /// <exception cref="CommunicationException">Thrown when connection failed</exception>
        public void Connect()
        {
            Log.Debug("Connect: begin.");

            if (_proxy.State != CommunicationState.Created)
            {
                Log.ErrorFormat("Connected: end. Client has invalid state {0}", _proxy.State);
                return;
            }

            bool coonected;

            try
            {
                coonected = _proxy.Connect();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Connect: Exception - {0}", ex.Message);

                throw new CommunicationException($"Connection failed to service. {ex.Message}");
            }

            if (coonected == false)
            {
                Log.Error("Connect: end. Connection failed.");
                throw new CommunicationException("Connection failed");
            }

            Log.Debug("Connect: end.");
        }

        public void Disconnect()
        {
            Log.Debug("Disconnect: begin.");

            try
            {
                _proxy.Disconnect();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Disconnect: Exception - {0}", ex.Message);
            }

            Log.Debug("Disconnect: end.");
        }

        /// <exception cref="CommunicationException">Thrown when connection failed</exception>
        public MTResponse SendCommand(int commandType, ArrayList parameters, Dictionary<string, object> namedParams, int expertHandle)
        {
            Log.DebugFormat("SendCommand: begin. commandType = {0}, parameters count = {1}", commandType, parameters?.Count);

            if (IsConnected == false)
            {
                Log.Error("SendCommand: Client is not connected.");
                throw new CommunicationException("Client is not connected.");
            }

            MTResponse result;

            try
            {
                result = _proxy.SendCommand(new MTCommand {
                    CommandType = commandType,
                    Parameters = parameters,
                    NamedParams = namedParams,
                    ExpertHandle = expertHandle});
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("SendCommand: Exception - {0}", ex.Message);

                throw new CommunicationException("Service connection failed! " + ex.Message);
            }

            Log.DebugFormat("SendCommand: end. result = {0}", result);

            return result;
        }

        /// <exception cref="CommunicationException">Thrown when connection failed</exception>
        public List<MTQuote> GetQuotes()
        {
            Log.Debug("GetQuotes: begin.");

            if (IsConnected == false)
            {
                Log.Warn("GetQuotes: end. Client is not connected.");
                return null;
            }

            List<MTQuote> result;

            try
            {
                result = _proxy.GetQuotes();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetQuotes: Exception - {0}", ex.Message);

                throw new CommunicationException($"Service connection failed! {ex.Message}");
            }

            Log.DebugFormat("GetQuotes: end. quotes count = {0}", result?.Count);

            return result;
        }

        #endregion

        #region IMtApiCallback Members

        public void OnQuoteUpdate(MTQuote quote)
        {
            Log.DebugFormat("OnQuoteUpdate: begin. quote = {0}", quote);

            if (quote == null) return;

            QuoteUpdated?.Invoke(quote);

            Log.Debug("OnQuoteUpdate: end.");
        }

        public void OnQuoteAdded(MTQuote quote)
        {
            Log.DebugFormat("OnQuoteAdded: begin. quote = {0}", quote);

            QuoteAdded?.Invoke(quote);

            Log.Debug("OnQuoteAdded: end.");
        }

        public void OnQuoteRemoved(MTQuote quote)
        {
            Log.DebugFormat("OnQuoteRemoved: begin. quote = {0}", quote);

            QuoteRemoved?.Invoke(quote);

            Log.Debug("OnQuoteRemoved: end.");
        }

        public void OnServerStopped()
        {
            Log.Debug("OnServerStopped: begin.");

            ServerDisconnected?.Invoke(this, EventArgs.Empty);

            Log.Debug("OnServerStopped: end.");
        }


        public void OnMtEvent(MTEvent e)
        {
            Log.DebugFormat("OnMtEvent: begin. event = {0}", e);

            MtEventReceived?.Invoke(e);

            Log.Debug("OnMtEvent: end.");
        }

        #endregion

        #region Properties
        public string Host { get; private set; }
        public int Port { get; private set; }

        private bool IsConnected => _proxy.State == CommunicationState.Opened;

        #endregion

        #region Private Methods

        private void ProxyFaulted(object sender, EventArgs e)
        {
            Log.Debug("ProxyFaulted: begin.");

            ServerFailed?.Invoke(this, EventArgs.Empty);

            Log.Debug("ProxyFaulted: end.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Log.Debug("Dispose: begin.");

            _proxy.Dispose();

            Log.Debug("Dispose: end.");
        }

        #endregion

        #region Events
        public event MTQuoteHandler QuoteAdded;
        public event MTQuoteHandler QuoteRemoved;
        public event MTQuoteHandler QuoteUpdated;
        public event EventHandler ServerDisconnected;
        public event EventHandler ServerFailed;
        public event MTEventHandler MtEventReceived;
        #endregion
    }
}
