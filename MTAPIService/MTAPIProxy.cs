using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MTAPIService
{
    internal class MTAPIProxy : DuplexClientBase<IMTAPI>, IMTAPI, IDisposable
    {
        public MTAPIProxy(InstanceContext callbackContext, Binding binding, EndpointAddress remoteAddress)
            : base(callbackContext, binding, remoteAddress)
        {
            InnerDuplexChannel.Faulted += InnerDuplexChannelFaulted;
        }

        #region IMTAPI Members

        public bool Connect()
        {
            InnerDuplexChannel.Open();
            return Channel.Connect();
        }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        public MTResponse SendCommand(MTCommand command)
        {
            return Channel.SendCommand(command);
        }

        public List<MTQuote> GetQuotes()
        {
            return Channel.GetQuotes();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Close();
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
            }
        }

        #endregion

        #region Private Methods
        private void InnerDuplexChannelFaulted(object sender, EventArgs e)
        {
            Faulted?.Invoke(this, e);
        }

        #endregion

        #region Events
        public event EventHandler Faulted;
        #endregion
    }
}
