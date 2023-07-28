using System;
using log4net;
using System.Collections.Generic;

namespace MTAPIService
{
    public class MTExpert: ITaskExecutor
    {
        public delegate void MTQuoteHandler(MTExpert expert, MTQuote quote);
        public delegate void MTEventHandler(MTExpert expert, MTEvent e);

        #region Private Fields
        private static readonly ILog Log = LogManager.GetLogger(typeof(MTExpert));

        private readonly IMetaTraderHandler _mtHadler;
        private MTCommandTask _currentTask;
        private readonly Queue<MTCommandTask> _taskQueue = new Queue<MTCommandTask>();
        private readonly object _locker = new object();
        #endregion

        #region Public Methods
        public MTExpert(int handle, string symbol, double bid, double ask, IMetaTraderHandler mtHandler)
        {
            if (mtHandler == null)
                throw new ArgumentNullException(nameof(mtHandler));

            _quote = new MTQuote { ExpertHandle = handle, Instrument = symbol, Bid = bid, Ask =  ask};
            Handle = handle;
            _mtHadler = mtHandler;
        }

        public virtual void Deinit()
        {
            Log.Debug("Deinit: begin.");

            FireOnDeinited();

            Log.Debug("Deinit: end.");
        }

        public void SendResponse(MTResponse response)
        {
            Log.DebugFormat("SendResponse: begin. response = {0}", response);

            _currentTask.SetResult(response);
            _currentTask = null;

            Log.Debug("SendResponse: end.");
        }

        public virtual int GetCommandType()
        {
            Log.Debug("GetCommandType: called.");

            _currentTask = DequeueTask();

            return _currentTask?.Command?.CommandType ?? 0;
        }

        public object GetCommandParameter(int index)
        {
            Log.DebugFormat("GetCommandParameter: called. index = {0}", index);

            var command = _currentTask?.Command;
            if (command?.Parameters != null && index >= 0 && index < command.Parameters.Count)
            {
                return command.Parameters[index];
            }

            return null;
        }

        public object GetNamedParameter(string name)
        {
            Log.DebugFormat("GetNamedParameter: called. name = {0}", name);

            var command = _currentTask?.Command;
            if (command == null)
            {
                Log.Warn("GetNamedParameter: command is not defined in task.");
                return null;
            }

            if (command.NamedParams == null)
            {
                Log.Warn("GetNamedParameter: NamedParams is not defined in command.");
                return null;
            }

            return command.NamedParams[name];
        }

        public bool ContainsNamedParameter(string name)
        {
            Log.DebugFormat("ContainsNamedParameter: called. name = {0}", name);

            var command = _currentTask?.Command;
            if (command == null)
            {
                Log.Warn("ContainsNamedParameter: command is not defined in task.");
                return false;
            }

            if (command.NamedParams == null)
            {
                Log.Warn("ContainsNamedParameter: NamedParams is not defined in command.");
                return false;
            }

            return command.NamedParams.ContainsKey(name);
        }

        public virtual void SendEvent(MTEvent mtEvent)
        {
            Log.DebugFormat("SendEvent: begin. event = {0}", mtEvent);

            FireOnMtEvent(mtEvent);

            Log.Debug("SendEvent: end.");
        }

        public void UpdateQuote(MTQuote quote)
        {
            Log.DebugFormat("UpdateQuote: begin. quote = {0}", quote);

            Quote = quote;

            Log.Debug("UpdateQuote: end.");
        }

        public override string ToString()
        {
            return $"ExpertHandle = {Handle}, Quote = {Quote}";
        }

        #endregion

        #region ITaskExecutor

        public void Execute(MTCommandTask task)
        {
            lock (_taskQueue)
            {
                _taskQueue.Enqueue(task);
            }

            NotifyCommandReady();
        }

        #endregion

        #region Properties

        private MTQuote _quote;
        public MTQuote Quote
        {
            get
            {
                lock (_locker)
                {
                    return _quote;
                }
            }
            private set
            {
                lock (_locker)
                {
                    _quote = value;
                }

                FireOnQuoteChanged(value);
            }
        }

        public int Handle { get; }
        #endregion

        #region Private Methods
        private MTCommandTask DequeueTask()
        {
            Log.Debug("DequeueTask: called.");

            MTCommandTask task;
            int count;

            lock (_locker)
            {
                count = _taskQueue.Count;
                task = _taskQueue.Count > 0 ? _taskQueue.Dequeue() : null;
            }

            Log.DebugFormat("DequeueTask: end. left task count = {0}.", count);

            return task;
        }

        private void NotifyCommandReady()
        {
            Log.Debug("NotifyCommandReady: begin.");

            _mtHadler.SendTickToMetaTrader(Handle);

            Log.Debug("NotifyCommandReady: end.");
        }

        private void FireOnQuoteChanged(MTQuote quote)
        {
            QuoteChanged?.Invoke(this, quote);
        }

        private void FireOnDeinited()
        {
            Deinited?.Invoke(this, EventArgs.Empty);
        }

        private void FireOnMtEvent(MTEvent mtEvent)
        {
            OnMtEvent?.Invoke(this, mtEvent);
        }
        #endregion

        #region Events
        public event EventHandler Deinited;
        public event MTQuoteHandler QuoteChanged;
        public event MTEventHandler OnMtEvent;
        #endregion
    }
}
