using MT5API;
using System.Threading;
using System;

namespace Program4Test
{
    class Program
    {
        static readonly EventWaitHandle TraderConnnectionWaiter = new AutoResetEvent(false);
        static readonly MT5APIClient MT5API = new MT5APIClient();

        static void Main(string[] _)
        {
            Console.WriteLine("Application started.");

            // Trader Bot
            MT5API.ConnectionStateChanged += MTAPITraderConnectionStateChanged;
            MT5API.BeginConnect(8228);
            TraderConnnectionWaiter.WaitOne();

            if (MT5API.ConnectionState != MT5ConnectionState.Connected)
            {
                Console.WriteLine("Connection to MetaTrader failed.");
                Thread.Sleep(5000);
                return;
            }
            MQLRates[] RET = new MQLRates[10];
            MT5API.CopyRates("EURUSD", EnumTimeframes.PERIOD_M1, 0, 10, out RET);
            Console.WriteLine(MT5API.AccountInfoDouble(EnumAccountInfoDouble.ACCOUNT_BALANCE));

            Console.ReadKey();
        }

        static void MTAPITraderConnectionStateChanged(object sender, MT5ConnectionEventArgs e)
        {
            switch (e.Status)
            {
                case MT5ConnectionState.Connecting:
                    Console.WriteLine("Connnecting to trader account...");
                    break;
                case MT5ConnectionState.Connected:
                    Console.WriteLine("Connnected to trader account.");
                    TraderConnnectionWaiter.Set();
                    break;
                case MT5ConnectionState.Disconnected:
                    Console.WriteLine("Disconnected from trader account.");
                    TraderConnnectionWaiter.Set();
                    break;
                case MT5ConnectionState.Failed:
                    Console.WriteLine("Connecting to trader account failed.");
                    TraderConnnectionWaiter.Set();
                    break;
            }
        }
    }
}
