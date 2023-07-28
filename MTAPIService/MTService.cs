using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using log4net;

namespace MTAPIService
{
    [ServiceContract(CallbackContract = typeof(IMTAPICallback), SessionMode = SessionMode.Required)]
    public interface IMTAPI
    {
        [OperationContract]
        bool Connect();

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        [OperationContract]
        MTResponse SendCommand(MTCommand command);

        [OperationContract]
        List<MTQuote> GetQuotes();
    }

    [ServiceContract]
    public interface IMTAPICallback
    {
        [OperationContract(IsOneWay = true)]
        void OnQuoteUpdate(MTQuote quote);

        [OperationContract(IsOneWay = true)]
        void OnServerStopped();

        [OperationContract(IsOneWay = true)]
        void OnQuoteAdded(MTQuote quote);

        [OperationContract(IsOneWay = true)]
        void OnQuoteRemoved(MTQuote quote);

        [OperationContract(IsOneWay = true)]
        void OnMtEvent(MTEvent ntEvent);
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                    AutomaticSessionShutdown = true,
                    IncludeExceptionDetailInFaults = true,
                    InstanceContextMode = InstanceContextMode.Single)]
    public sealed class MTService : IMTAPI
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MTService));

        public MTService(IMTAPIServer serverCallback)
        {
            _server = serverCallback ?? throw new ArgumentNullException(nameof(serverCallback));
        }

        #region IMtApi
        public bool Connect()
        {
            Log.Debug("Connect: begin");

            var callback = OperationContext.Current.GetCallbackChannel<IMTAPICallback>();

            if (callback == null)
            {
                Log.Warn("Connect: end. Callback is not defined.");
                return false;
            }

            var connected = false;

            try
            {
                _clientsLocker.AcquireWriterLock(10000);

                try
                {
                    if (_clientCallbacks.Contains(callback) == false)
                        _clientCallbacks.Add(callback);

                    connected = true;
                }
                finally
                {
                    _clientsLocker.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                Log.ErrorFormat("Connect: ApplicationException - {0}", ex.Message);
            }

            Log.DebugFormat("Connect: end. connected = {0}", connected);

            return connected;
        }

        public void Disconnect()
        {
            Log.Debug("Disconnect: begin");

            var callback = OperationContext.Current.GetCallbackChannel<IMTAPICallback>();

            if (callback == null)
            {
                Log.Warn("Disconnect: end. Callback is not definded.");
                return;
            }

            try
            {
                _clientsLocker.AcquireWriterLock(10000);

                try
                {
                    _clientCallbacks.Remove(callback);
                }
                finally
                {
                    _clientsLocker.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                Log.ErrorFormat("Disconnect: ApplicationException - {0}", ex.Message);
            }

            Log.Debug("Disconnect: end.");
        }

        public MTResponse SendCommand(MTCommand command)
        {
            Log.DebugFormat("SendCommand: called. command = {0}", command);

            return _server.SendCommand(command);
        }

        public List<MTQuote> GetQuotes()
        {
            Log.Debug("GetQuotes: called.");

            return _server.GetQuotes();
        }
        #endregion

        #region Public Methods
        public void OnStopServer()
        {
            Log.Debug("OnStopServer: begin.");

            ExecuteCallbackAction(a => a.OnServerStopped());

            Log.Debug("OnStopServer: end.");
        }

        public void QuoteUpdate(MTQuote quote)
        {
            Log.DebugFormat("QuoteUpdate: begin. quote = {0}", quote);

            ExecuteCallbackAction(a => a.OnQuoteUpdate(quote));

            Log.Debug("QuoteUpdate: end.");
        }

        public void OnQuoteAdded(MTQuote quote)
        {
            Log.DebugFormat("OnQuoteAdded: begin. quote = {0}", quote);

            ExecuteCallbackAction(a => a.OnQuoteAdded(quote));

            Log.Debug("OnQuoteAdded: end.");
        }

        public void OnQuoteRemoved(MTQuote quote)
        {
            Log.DebugFormat("OnQuoteRemoved: begin. quote = {0}", quote);

            ExecuteCallbackAction(a => a.OnQuoteRemoved(quote));

            Log.Debug("OnQuoteRemoved: end.");
        }

        public void OnMtEvent(MTEvent e)
        {
            Log.DebugFormat("OnMtEvent: begin.quote = {0}", e);

            ExecuteCallbackAction(a => a.OnMtEvent(e));

            Log.Debug("OnMtEvent: end.");
        }
        #endregion

        #region Private Methods

        private void ExecuteCallbackAction(Action<IMTAPICallback> action)
        {
            Log.Debug("ExecuteCallbackAction: begin.");

            try
            {
                _clientsLocker.AcquireReaderLock(2000);

                List<IMTAPICallback> crashedClientCallbacks = null;

                try
                {
                    foreach (var callback in _clientCallbacks)
                    {
                        try
                        {
                            action(callback);
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("ExecuteCallbackAction: Exception - {0}", ex.Message);

                            if (crashedClientCallbacks == null)
                                crashedClientCallbacks = new List<IMTAPICallback>();

                            crashedClientCallbacks.Add(callback);
                        }
                    }

                    if (crashedClientCallbacks != null)
                    {
                        Log.WarnFormat("ExecuteCallbackAction: crashed callback count = {0}", crashedClientCallbacks.Count);

                        foreach (var crashedCallback in crashedClientCallbacks)
                        {
                            _clientCallbacks.Remove(crashedCallback);
                        }
                    }
                }
                finally
                {
                    _clientsLocker.ReleaseReaderLock();
                }
            }
            catch (ApplicationException ex)
            {
                Log.ErrorFormat("ExecuteCallbackAction: ApplicationException - {0}", ex.Message);
            }

            Log.Debug("ExecuteCallbackAction: end.");
        }

        #endregion

        #region Fields
        private readonly IMTAPIServer _server;
        private readonly List<IMTAPICallback> _clientCallbacks = new List<IMTAPICallback>();
        private readonly ReaderWriterLock _clientsLocker = new ReaderWriterLock();
        #endregion
    }
}
