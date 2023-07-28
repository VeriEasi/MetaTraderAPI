using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Channels;
using System.Net.Sockets;
using log4net;

namespace MTAPIService
{
    internal class MTServer : IDisposable, IMTAPIServer
    {
        #region Constants
        private const int WaitResponseTime = 40000; // 40 sec
        private const int StopExpertInterval = 1000; // 1 sec 
        #endregion

        #region Fields
        private static readonly ILog Log = LogManager.GetLogger(typeof(MTServer));

        private readonly MTService _service;
        private readonly List<ServiceHost> _hosts = new List<ServiceHost>();
        private readonly MTExecutorManager _executorManager = new MTExecutorManager();
        private readonly List<MTExpert> _experts = new List<MTExpert>();
        #endregion

        #region ctor
        public MTServer(int port)
        {
            Port = port;
            _service = new MTService(this);
        }
        #endregion

        #region Properties
        public int Port { get; }
        #endregion

        #region Public Methods
        public void Start()
        {
            Log.Debug("Start: begin");

            var hostsInitialized = InitHosts(Port);

            Log.DebugFormat("Start: end. hostsInitialized = {0}", hostsInitialized);
        }

        private bool InitHosts(int port)
        {
            Log.DebugFormat("InitHosts: begin. port = {0}", port);

            int count;

            lock (_hosts)
            {
                if (_hosts.Count > 0)
                {
                    Log.Info("InitHosts: end. Host's has been initialized");
                    return false;
                }

                //init local pipe host
                var localPipeUrl = CreateConnectionAddress(null, port, true);
                var localPipeServiceHost = CreateServiceHost(localPipeUrl, true);
                if (localPipeServiceHost != null)
                {
                    _hosts.Add(localPipeServiceHost);
                }

                //init localhost
                var localUrl = CreateConnectionAddress("localhost", port, false);
                var localServiceHost = CreateServiceHost(localUrl, false);
                if (localServiceHost != null)
                {
                    _hosts.Add(localServiceHost);
                }

                //init network hosts
                var dnsHostName = Dns.GetHostName();
                var ips = Dns.GetHostEntry(dnsHostName);

                foreach (var ipAddress in ips.AddressList)
                {
                    if (ipAddress?.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var ip = ipAddress.ToString();
                        var networkUrl = CreateConnectionAddress(ip, port, false);
                        var serviceHost = CreateServiceHost(networkUrl, false);
                        if (serviceHost != null)
                        {
                            _hosts.Add(serviceHost);
                        }
                    }
                }

                count = _hosts.Count;
            }

            Log.DebugFormat("InitHosts: end. Host's count = {0}", count);

            return true;
        }

        private ServiceHost CreateServiceHost(string serverUrlAdress, bool local)
        {
            Log.DebugFormat("CreateServiceHost: begin. serverUrlAdress = {0}; local = {1}", serverUrlAdress, local);

            if (serverUrlAdress == null)
            {
                Log.Warn("CreateServiceHost: end. serverUrlAdress is not defined");
                return null;
            }

            ServiceHost serviceHost;
            try
            {
                serviceHost = new ServiceHost(_service);
                var binding = CreateConnectionBinding(local);

                serviceHost.AddServiceEndpoint(typeof(IMTAPI), binding, serverUrlAdress);
                serviceHost.Open();
            }
            catch(Exception e) 
            {
                Log.ErrorFormat("CreateServiceHost: Error! {0}", e.Message);
                serviceHost = null;
            }

            Log.Debug("CreateServiceHost: end.");

            return serviceHost;
        }

        public void AddExpert(MTExpert expert)
        {
            Log.DebugFormat("AddExpert: begin. expert {0}", expert);

            if (expert == null)
            {
                Log.Warn("AddExpert: end. expert is not defined");
                return;
            }

            expert.Deinited += ExpertDeinited;
            expert.QuoteChanged += ExpertQuoteChanged;
            expert.OnMtEvent += ExpertOnMTEvent;

            lock (_experts)
            {
                _experts.Add(expert);
            }

            _executorManager.AddExecutor(expert);

            _service.OnQuoteAdded(expert.Quote);

            Log.Debug("AddExpert: end.");
        }

        #endregion

        #region IMTAPIServerCallback Members

        public MTResponse SendCommand(MTCommand command)
        {
            Log.DebugFormat("SendCommand: begin. command {0}", command);

            if (command == null)
            {
                Log.Warn("SendCommand: end. command is not defined");
                return null;
            }

            var task = _executorManager.SendCommand(command);

            //wait for execute command in MetaTrader
            MTResponse response = null;
            try
            {
                response = task.WaitResult(WaitResponseTime);
            }
            catch (Exception ex)
            {
                Log.WarnFormat("SendCommand: Exception - {0}", ex.Message);
            }

            Log.DebugFormat("SendCommand: end. response = {0}", response);

            return response;
        }

        public List<MTQuote> GetQuotes()
        {
            Log.Debug("GetQuotes: called");

            lock (_experts)
            {
                return _experts.Select(s => s.Quote).ToList();
            }
        }

        #endregion

        #region Private Methods
        private void StopTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Log.DebugFormat("StopTimerElapsed: begin");

            int expertsCount;

            lock (_experts)
            {
                expertsCount = _experts.Count;
            }

            if (expertsCount == 0)
            {
                _service.OnStopServer();

                Log.DebugFormat("stopTimer_Elapsed: experts count is 0. Call Stop().");

                Stop();
            }

            if (!(sender is System.Timers.Timer stopTimer)) return;

            stopTimer.Stop();
            stopTimer.Elapsed -= StopTimerElapsed;

            Log.DebugFormat("stopTimer_Elapsed: end");
        }

        private static string CreateConnectionAddress(string host, int port, bool local)
        {
            Log.DebugFormat("CreateConnectionAddress: begin. host = {0}, port = {1}, local = {2}", host, port, local);

            string connectionAddress = null;

            if (local)
            {
                //by Pipe
                connectionAddress = "net.pipe://localhost/MtApiService_" + port;
            }
            else
            {
                //by Socket
                if (host != null)
                {
                    connectionAddress = "net.tcp://" + host + ":" + port + "/MtApiService";
                }
            }

            Log.DebugFormat("CreateConnectionAddress: end. connectionAddress = {0}", connectionAddress);

            return connectionAddress;
        }

        private static Binding CreateConnectionBinding(bool local)
        {
            Log.DebugFormat("CreateConnectionBinding: begin. local = {0}", local);

            Binding connectionBinding;

            if (local)
            {
                //by Pipe
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

                // Commented next statement since it is not required

                connectionBinding = bind;
            }
            else
            {
                //by Socket
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

                // Commented next statement since it is not required

                connectionBinding = bind;
            }

            Log.Debug("CreateConnectionBinding: end.");

            return connectionBinding;
        }

        private void Stop()
        {
            Log.Debug("Stop: begin.");

            _executorManager.Stop();

            lock (_hosts)
            {
                if (_hosts.Count == 0)
                {
                    Log.Debug("Stop: end. Host count is 0.");
                    return;
                }

                try
                {
                    foreach (var host in _hosts)
                    {
                        host.Close();
                    }
                }
                catch (TimeoutException ex)
                {
                    Log.ErrorFormat("Stop: TimeoutException - {0}", ex.Message);

                    foreach (var host in _hosts)
                    {
                        host.Abort();
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Stop: Exception - {0}", ex.Message);
                    // ignored
                }
                finally
                {
                    _hosts.Clear();
                }
            }

            FireOnStopped();

            Log.Debug("Stop: end.");
        }

        private void ExpertDeinited(object sender, EventArgs e)
        {
            Log.Debug("expert_Deinited: begin.");

            if (!(sender is MTExpert expert))
            {
                Log.Warn("ExpertDeinited: end. Expert is not defined.");
                return;
            }

            int expertsCount;

            lock (_experts)
            {
                _experts.Remove(expert);
                expertsCount = _experts.Count;
            }

            _executorManager.RemoveExecutor(expert);

            expert.Deinited -= ExpertDeinited;
            expert.QuoteChanged -= ExpertQuoteChanged;

            _service.OnQuoteRemoved(expert.Quote);

            if (expertsCount == 0)
            {
                var stopTimer = new System.Timers.Timer();
                stopTimer.Elapsed += StopTimerElapsed;
                stopTimer.Interval = StopExpertInterval;
                stopTimer.Start();
            }

            Log.Debug("ExpertDeinited: end.");
        }

        private void ExpertQuoteChanged(MTExpert expert, MTQuote quote)
        {
            Log.DebugFormat("ExpertQuoteChanged: begin. expert = {0}, quote = {1}", expert, quote);

            _service.QuoteUpdate(quote);

            Log.Debug("ExpertQuoteChanged: end.");
        }

        private void ExpertOnMTEvent(MTExpert expert, MTEvent e)
        {
            Log.DebugFormat("ExpertOnMtEvent: begin. expert = {0}, event = {1}", expert, e);

            _service.OnMtEvent(e);

            Log.Debug("ExpertOnMtEvent: end.");
        }

        private void FireOnStopped()
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events
        public event EventHandler Stopped;
        #endregion

        #region IDispose
        public void Dispose()
        {
            Log.Debug("Dispose: begin");

            Stop();

            Log.Debug("Dispose: end.");
        }

        #endregion
    }
}
