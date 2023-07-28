using System;
using System.Collections.Generic;
using log4net;

namespace MTAPIService
{
    public class MTAdapter
    {
        #region Fields
        private const string LogProfileName = "MTAPIService";

        private static readonly ILog Log = LogManager.GetLogger(typeof(MTAdapter));
        private static readonly MTAdapter Instance = new MTAdapter();

        private readonly Dictionary<int, MTServer> _servers = new Dictionary<int, MTServer>();
        private readonly Dictionary<int, MTExpert> _experts = new Dictionary<int, MTExpert>();
        #endregion

        #region Init Instance

        private MTAdapter()
        {
            LogConfigurator.Setup(LogProfileName);
        }
        
        static MTAdapter() 
        {
        }

        public static MTAdapter GetInstance()
        {
            return Instance;
        }

        #endregion


        #region Public Methods
        public void AddExpert(int port, MTExpert expert)
        {
            if (expert == null)
                throw new ArgumentNullException(nameof(expert));

            Log.InfoFormat("AddExpert: begin. expert = {0}", expert);

            MTServer server;
            lock (_servers)
            {
                if (_servers.ContainsKey(port))
                {
                    server = _servers[port];
                }
                else
                {
                    server = new MTServer(port);
                    server.Stopped += ServerStopped;
                    _servers[port] = server;

                    server.Start();
                }
            }

            lock (_experts)
            {
                _experts[expert.Handle] = expert;
            }

            server.AddExpert(expert);
            expert.Deinited += ExpertOnDeinited;

            Log.Info("AddExpert: end");
        }

        public void RemoveExpert(int expertHandle)
        {
            Log.InfoFormat("RemoveExpert: begin. expertHandle = {0}", expertHandle);

            MTExpert expert = null;

            lock (_experts)
            {
                if (_experts.ContainsKey(expertHandle))
                {
                    expert = _experts[expertHandle];
                }
            }

            if (expert != null)
            {
                expert.Deinit();
            }
            else
            {
                Log.WarnFormat("RemoveExpert: expert with id {0} has not been found.", expertHandle);
            }

            Log.Info("RemoveExpert: end");
        }

        public void SendQuote(int expertHandle, string symbol, double bid, double ask)
        {
            Log.DebugFormat("UpdateQuote: begin. symbol = {0}, bid = {1}, ask = {2}", symbol, bid, ask);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert != null)
            {
                expert.UpdateQuote(new MTQuote { Instrument = symbol, Bid = bid, Ask = ask, ExpertHandle = expertHandle });
            }
            else
            {
                Log.WarnFormat("UpdateQuote: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("UpdateQuote: end");
        }

        public void SendEvent(int expertHandle, int eventType, string payload)
        {
            Log.DebugFormat("SendEvent: begin. eventType = {0}, payload = {1}", eventType, payload);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert != null)
            {
                expert.SendEvent(new MTEvent { EventType = eventType, Payload = payload, ExpertHandle = expertHandle });
            }
            else
            {
                Log.WarnFormat("SendEvent: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("SendEvent: end");
        }

        public void SendResponse(int expertHandle, MTResponse response)
        {
            Log.DebugFormat("SendResponse: begin. id = {0}, response = {1}", expertHandle, response);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert != null)
            {
                expert.SendResponse(response);
            }
            else
            {
                Log.WarnFormat("SendResponse: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("SendResponse: end");
        }

        public int GetCommandType(int expertHandle)
        {
            Log.DebugFormat("GetCommandType: begin. expertHandle = {0}", expertHandle);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert == null)
            {
                Log.WarnFormat("GetCommandType: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetCommandType() ?? 0;

            Log.DebugFormat("GetCommandType: end. retval = {0}", retval);

            return retval;
        }

        public T GetCommandParameter<T>(int expertHandle, int index)
        {
            Log.DebugFormat("GetCommandParameter: begin. expertHandle = {0}, index = {1}", expertHandle, index);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert == null)
            {
                Log.WarnFormat("GetCommandParameter: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetCommandParameter(index);

            Log.DebugFormat("GetCommandParameter: end. retval = {0}", retval);

            return (T)retval;
        }

        public object GetNamedParameter(int expertHandle, string name)
        {
            Log.DebugFormat("GetNamedParameter: begin. expertHandle = {0}, name = {1}", expertHandle, name);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert == null)
            {
                Log.WarnFormat("GetNamedParameter: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetNamedParameter(name);

            Log.DebugFormat("GetNamedParameter: end. retval = {0}", retval);

            return retval;
        }

        public bool ContainsNamedParameter(int expertHandle, string name)
        {
            Log.DebugFormat("ContainsNamedParameter: begin. expertHandle = {0}, name = {1}", expertHandle, name);

            MTExpert expert;
            lock (_experts)
            {
                expert = _experts[expertHandle];
            }

            if (expert == null)
            {
                Log.WarnFormat("ContainsNamedParameter: expert with id {0} has not been found.", expertHandle);
            }

            bool retval = expert != null && expert.ContainsNamedParameter(name);

            Log.DebugFormat("ContainsNamedParameter: end. retval = {0}", retval);

            return retval;
        }

        public void LogError(string message)
        {
            Log.Error(message);
        }
        #endregion

        #region Private Methods
        private void ServerStopped(object sender, EventArgs e)
        {
            var server = (MTServer)sender;
            server.Stopped -= ServerStopped;

            var port = server.Port;

            Log.InfoFormat("ServerStopped: port = {0}", port);

            lock (_servers)
            {
                if (_servers.ContainsKey(port))
                {
                    _servers.Remove(port);
                }
            }
        }


        private void ExpertOnDeinited(object sender, EventArgs eventArgs)
        {
            Log.Debug("ExpertOnDeinited: begin.");

            if (!(sender is MTExpert expert))
            {
                Log.Warn("expert_Deinited: end. Expert is not defined.");
                return;
            }

            lock (_experts)
            {
                if (_experts.ContainsKey(expert.Handle))
                {
                    _experts.Remove(expert.Handle);
                }
            }

            Log.DebugFormat("ExpertOnDeinited: removed expert {0}", expert.Handle);

            Log.Debug("ExpertOnDeinited: end.");
        }
        #endregion
    }
}
