using System;
using MT4API.Monitors.Triggers;

namespace MT4API.Monitors
{
    public abstract class MT4MonitorBase
    {
        #region Fields
        private volatile bool _isStarted = false;
        private bool _syncTrigger;
        #endregion

        #region Properties
        /// <summary>
        /// ApiClient
        /// </summary>
        protected MT4APIClient APIClient { get; }
        /// <summary>
        /// Returns true if the <see cref="APIClient"/> is connected.
        /// </summary>
        public bool IsMT4Connected => APIClient.ConnectionState == MT4ConnectionState.Connected;
        /// <summary>
        /// Returns the trigger which will be used to raise the monitoring call.
        /// </summary>
        public IMonitorTrigger MonitorTrigger { get; }
        /// <summary>
        /// Returns true if the Monitor is started.
        /// </summary>
        public bool IsStarted { get => _isStarted; }
        /// <summary>
        /// If true, the <see cref="MonitorTrigger"/> will be stopped or started automatically when <see cref="Start"/> or <see cref="Stop"/> will be called.
        /// <para>CAUTION: If you use the MonitorTrigger for different Monitors, this will stop all monitors if you call stop and <see cref="SyncTrigger"/> is <c>true</c>.</para>
        /// </summary>
        public bool SyncTrigger { get => _syncTrigger; set => _syncTrigger = value; }
        #endregion

        #region ctor
        /// <summary>
        /// Default constructor for Monitors
        /// </summary>
        /// <param name="apiClient">The apiClient which will be used to work with.</param>
        /// <param name="monitorTrigger">The trigger which lead this Monitor to do his work.</param>
        /// <param name="syncTrigger">See property <see cref="SyncTrigger"/>.</param>
        public MT4MonitorBase(MT4APIClient apiClient, IMonitorTrigger monitorTrigger, bool syncTrigger = false)
        {
            APIClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            MonitorTrigger = monitorTrigger ?? throw new ArgumentNullException(nameof(monitorTrigger));
            SyncTrigger = syncTrigger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Let the monitor listen to the <see cref="MonitorTrigger"/>.
        /// </summary>
        public virtual void Start()
        {
            if (!_isStarted)
            {
                APIClient.ConnectionStateChanged += APIClientConnectionStateChanged;
                MonitorTrigger.Raised += MonitorTriggerRaised;
                _isStarted = true;
                OnStart();
                if (SyncTrigger)
                    MonitorTrigger.Start();
            }
        }
        /// <summary>
        /// Let the monitor stop listening to the <see cref="MonitorTrigger"/>.
        /// </summary>
        public virtual void Stop()
        {
            if (_isStarted)
            {
                MonitorTrigger.Raised -= MonitorTriggerRaised;
                APIClient.ConnectionStateChanged -= APIClientConnectionStateChanged;
                _isStarted = false;
                OnStop();
                if (SyncTrigger)
                    MonitorTrigger.Stop();
            }
        }
        private void MonitorTriggerRaised(object sender, EventArgs e) => OnTriggerRaised();
        private void APIClientConnectionStateChanged(object sender, MT4ConnectionEventArgs e)
        {
            if (e.Status == MT4ConnectionState.Connected)
                OnMT4Connected();
            else if (e.Status == MT4ConnectionState.Failed || e.Status == MT4ConnectionState.Disconnected)
                OnMT4Disconnected();
        }
        /// <summary>
        /// Will be called when <see cref="Start"/> will be called.
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Will be called when <see cref="Stop"/> will be called.
        /// </summary>
        protected virtual void OnStop() { }
        /// <summary>
        /// Will be called when the <see cref="APIClient"/> is successfully connected.
        /// </summary>
        protected virtual void OnMT4Connected() { }
        /// <summary>
        /// Will be called when <see cref="APIClient"/> is disconnected.
        /// </summary>
        protected virtual void OnMT4Disconnected() { }
        /// <summary>
        /// Will be called when the <see cref="MonitorTrigger"/> raised.
        /// </summary>
        protected abstract void OnTriggerRaised();
        #endregion
    }
}