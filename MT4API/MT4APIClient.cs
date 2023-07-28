using System;
using System.Collections.Generic;
using System.Linq;
using MTAPIService;
using System.Drawing;
using System.Collections;
using System.ServiceModel;
using MT4API.Requests;
using MT4API.Responses;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MT4API.Events;

namespace MT4API
{
    public delegate void MTAPIQuoteHandler(object sender, string symbol, double bid, double ask);

    public sealed class MT4APIClient
    {
        #region MetaTrader Constants

        //Special constant
        public const int NULL = 0;
        public const int EMPTY = -1;

        private const string LogProfileName = "MT4APIClient";
        #endregion

        #region Private Fields
        private static readonly MTLog Log = LogConfigurator.GetLogger(typeof(MT4APIClient));

        private MTClient _client;
        private readonly object _locker = new object();
        private MT4ConnectionState _connectionState = MT4ConnectionState.Disconnected;
        private volatile bool _isBacktestingMode;
        private int _executorHandle;
        #endregion

        #region ctor

        public MT4APIClient()
        {
#if (DEBUG)
            const LogLevel logLevel = LogLevel.Debug;
#else
            const LogLevel logLevel = LogLevel.Info;
#endif
            LogConfigurator.Setup(LogProfileName, logLevel);
        }
        #endregion

        #region Public Methods
        ///<summary>
        ///Connect with MetaTrader API. Async method.
        ///</summary>
        ///<param name="host">Address of MetaTrader host (ex. 192.168.1.2)</param>
        ///<param name="port">Port of host connection (default 8222) </param>
        public void BeginConnect(string host, int port)
        {
            Log.Info($"BeginConnect: host = {host}, port = {port}");
            Task.Factory.StartNew(() => Connect(host, port));
        }

        ///<summary>
        ///Connect with MetaTrader API. Async method.
        ///</summary>
        ///<param name="port">Port of host connection (default 8222) </param>
        public void BeginConnect(int port)
        {
            Log.Info($"BeginConnect: port = {port}");
            Task.Factory.StartNew(() => Connect(port));
        }

        ///<summary>
        ///Disconnect from MetaTrader API. Async method.
        ///</summary>
        public void BeginDisconnect()
        {
            Log.Info("BeginDisconnect called.");
            Task.Factory.StartNew(() => Disconnect(false));
        }

        ///<summary>
        ///Load quotes connected into MetaTrader API.
        ///</summary>
        public List<MT4Quote> GetQuotes()
        {
            var client = Client;
            var quotes = client?.GetQuotes();
            return quotes?.Select(q => new MT4Quote(q)).ToList();
        }
        #endregion

        #region Properties

        ///<summary>
        ///Connection status of MetaTrader API.
        ///</summary>
        public MT4ConnectionState ConnectionState
        {
            get
            {
                lock (_locker)
                {
                    return _connectionState;
                }
            }
        }

        ///<summary>
        ///Handle of expert used to execute commands
        ///</summary>
        public int ExecutorHandle
        {
            get
            {
                lock (_locker)
                {
                    return _executorHandle;
                }
            }
            set
            {
                lock (_locker)
                {
                    _executorHandle = value;
                }
            }
        }
        #endregion


        #region Deprecated Methods
        [Obsolete("OrderCloseByCurrentPrice is deprecated, please use OrderClose instead.")]
        public bool OrderCloseByCurrentPrice(int ticket, int slippage)
        {
            return OrderClose(ticket, slippage);
        }

        [Obsolete("OrderClosePrice is deprecated, please use GetOrder instead.")]
        public double OrderClosePrice()
        {
            return SendCommand<double>(MT4CommandType.OrderClosePrice, null);
        }

        [Obsolete("OrderClosePrice is deprecated, please use GetOrder instead.")]
        public double OrderClosePrice(int ticket)
        {
            var commandParameters = new ArrayList { ticket };
            double retVal = SendCommand<double>(MT4CommandType.OrderClosePriceByTicket, commandParameters);

            return retVal;
        }

        [Obsolete("OrderCloseTime is deprecated, please use GetOrder instead.")]
        public DateTime OrderCloseTime()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.OrderCloseTime, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        [Obsolete("OrderComment is deprecated, please use GetOrder instead.")]
        public string OrderComment()
        {
            return SendCommand<string>(MT4CommandType.OrderComment, null);
        }

        [Obsolete("OrderCommission is deprecated, please use GetOrder instead.")]
        public double OrderCommission()
        {
            return SendCommand<double>(MT4CommandType.OrderCommission, null);
        }

        [Obsolete("OrderExpiration is deprecated, please use GetOrder instead.")]
        public DateTime OrderExpiration()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.OrderExpiration, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        [Obsolete("OrderLots is deprecated, please use GetOrder instead.")]
        public double OrderLots()
        {
            return SendCommand<double>(MT4CommandType.OrderLots, null);
        }

        [Obsolete("OrderMagicNumber is deprecated, please use GetOrder instead.")]
        public int OrderMagicNumber()
        {
            return SendCommand<int>(MT4CommandType.OrderMagicNumber, null);
        }

        [Obsolete("OrderOpenPrice is deprecated, please use GetOrder instead.")]
        public double OrderOpenPrice()
        {
            return SendCommand<double>(MT4CommandType.OrderOpenPrice, null);
        }

        [Obsolete("OrderOpenPrice is deprecated, please use GetOrder instead.")]
        public double OrderOpenPrice(int ticket)
        {
            var commandParameters = new ArrayList { ticket };
            var retVal = SendCommand<double>(MT4CommandType.OrderOpenPriceByTicket, commandParameters);

            return retVal;
        }

        [Obsolete("OrderOpenTime is deprecated, please use GetOrder instead.")]
        public DateTime OrderOpenTime()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.OrderOpenTime, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        [Obsolete("OrderProfit is deprecated, please use GetOrder instead.")]
        public double OrderProfit()
        {
            return SendCommand<double>(MT4CommandType.OrderProfit, null);
        }

        [Obsolete("OrderStopLoss is deprecated, please use GetOrder instead.")]
        public double OrderStopLoss()
        {
            return SendCommand<double>(MT4CommandType.OrderStopLoss, null);
        }

        [Obsolete("OrderSymbol is deprecated, please use GetOrder instead.")]
        public string OrderSymbol()
        {
            return SendCommand<string>(MT4CommandType.OrderSymbol, null);
        }

        [Obsolete("OrderTakeProfit is deprecated, please use GetOrder instead.")]
        public double OrderTakeProfit()
        {
            return SendCommand<double>(MT4CommandType.OrderTakeProfit, null);
        }

        [Obsolete("OrderTicket is deprecated, please use GetOrder instead.")]
        public int OrderTicket()
        {
            return SendCommand<int>(MT4CommandType.OrderTicket, null);
        }

        [Obsolete("OrderType is deprecated, please use GetOrder instead.")]
        public TradeOperation OrderType()
        {
            var retVal = SendCommand<int>(MT4CommandType.OrderType, null);

            return (TradeOperation)retVal;
        }

        [Obsolete("OrderSwap is deprecated, please use GetOrder instead.")]
        public double OrderSwap()
        {
            return SendCommand<double>(MT4CommandType.OrderSwap, null);
        }
        #endregion

        #region Trading functions

        public int OrderSend(string symbol, TradeOperation cmd, double volume, double price, int slippage, double stoploss, double takeprofit
            , string comment, int magic, DateTime expiration, Color arrowColor)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}, magic = {magic}, expiration = {expiration}, arrowColor = {arrowColor}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)cmd,
                Volume = volume,
                Price = price,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment,
                Magic = magic,
                Expiration = MT4APITimeConverter.ConvertToMTTime(expiration),
                ArrowColor = MT4APIColorConverter.ConvertToMT4Color(arrowColor)
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSend(string symbol, TradeOperation cmd, double volume, double price, int slippage, double stoploss, double takeprofit
                    , string comment, int magic, DateTime expiration)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}, magic = {magic}, expiration = {expiration}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)cmd,
                Volume = volume,
                Price = price,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment,
                Magic = magic,
                Expiration = MT4APITimeConverter.ConvertToMTTime(expiration)
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSend(string symbol, TradeOperation cmd, double volume, double price, int slippage, double stoploss, double takeprofit
                    , string comment, int magic)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}, magic = {magic}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)cmd,
                Volume = volume,
                Price = price,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment,
                Magic = magic
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSend(string symbol, TradeOperation cmd, double volume, double price, int slippage, double stoploss, double takeprofit
                    , string comment)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)cmd,
                Volume = volume,
                Price = price,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSend(string symbol, TradeOperation cmd, double volume, double price, int slippage, double stoploss, double takeprofit)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)cmd,
                Volume = volume,
                Price = price,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSend(string symbol, TradeOperation cmd, double volume, string price, int slippage, double stoploss, double takeprofit)
        {
            Log.Debug($"OrderSend: symbol = {symbol}, cmd = {cmd}, volume = {volume}, price = {price}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}");

            return double.TryParse(price, out double dPrice) ?
                OrderSend(symbol, cmd, volume, dPrice, slippage, stoploss, takeprofit) : 0;
        }

        public int OrderSendBuy(string symbol, double volume, int slippage)
        {
            Log.Debug($"OrderSendBuy: symbol = {symbol}, volume = {volume}, slippage = {slippage}");

            return OrderSendBuy(symbol, volume, slippage, 0, 0, null, 0);
        }

        public int OrderSendSell(string symbol, double volume, int slippage)
        {
            return OrderSendSell(symbol, volume, slippage, 0, 0, null, 0);
        }

        public int OrderSendBuy(string symbol, double volume, int slippage, double stoploss, double takeprofit)
        {
            return OrderSendBuy(symbol, volume, slippage, stoploss, takeprofit, null, 0);
        }

        public int OrderSendSell(string symbol, double volume, int slippage, double stoploss, double takeprofit)
        {
            return OrderSendSell(symbol, volume, slippage, stoploss, takeprofit, null, 0);
        }

        public int OrderSendBuy(string symbol, double volume, int slippage, double stoploss, double takeprofit, string comment, int magic)
        {
            Log.Debug($"OrderSendBuy: symbol = {symbol}, volume = {volume}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}, magic = {magic}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)TradeOperation.OP_BUY,
                Volume = volume,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment,
                Magic = magic,
            });
            return response?.Ticket ?? -1;
        }

        public int OrderSendSell(string symbol, double volume, int slippage, double stoploss, double takeprofit, string comment, int magic)
        {
            Log.Debug($"OrderSendSell: symbol = {symbol}, volume = {volume}, slippage = {slippage}, stoploss = {stoploss}, takeprofit = {takeprofit}, comment = {comment}, magic = {magic}");

            var response = SendRequest<OrderSendResponse>(new OrderSendRequest
            {
                Symbol = symbol,
                Cmd = (int)TradeOperation.OP_SELL,
                Volume = volume,
                Slippage = slippage,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Comment = comment,
                Magic = magic,
            });
            return response?.Ticket ?? -1;
        }

        public bool OrderClose(int ticket, double lots, double price, int slippage, Color color)
        {
            Log.Debug($"OrderClose: ticket = {ticket}, lots = {lots}, price = {price}, slippage = {slippage}, color = {color}");

            var response = SendRequest<ResponseBase>(new OrderCloseRequest
            {
                Ticket = ticket,
                Lots = lots,
                Price = price,
                Slippage = slippage,
                ArrowColor = MT4APIColorConverter.ConvertToMT4Color(color)
            });
            return response != null;
        }

        public bool OrderClose(int ticket, double lots, double price, int slippage)
        {
            Log.Debug($"OrderClose: ticket = {ticket}, lots = {lots}, price = {price}, slippage = {slippage}");

            var response = SendRequest<ResponseBase>(new OrderCloseRequest
            {
                Ticket = ticket,
                Lots = lots,
                Price = price,
                Slippage = slippage,
            });
            return response != null;
        }

        public bool OrderClose(int ticket, double lots, int slippage)
        {
            Log.Debug($"OrderClose: ticket = {ticket}, lots = {lots}, slippage = {slippage}");

            var response = SendRequest<ResponseBase>(new OrderCloseRequest
            {
                Ticket = ticket,
                Lots = lots,
                Slippage = slippage,
            });
            return response != null;
        }

        public bool OrderClose(int ticket, int slippage)
        {
            Log.Debug($"OrderClose: ticket = {ticket}, slippage = {slippage}");

            var response = SendRequest<ResponseBase>(new OrderCloseRequest
            {
                Ticket = ticket,
                Slippage = slippage,
            });
            return response != null;
        }

        public bool OrderCloseBy(int ticket, int opposite, Color color)
        {
            Log.Debug($"OrderCloseBy: ticket = {ticket}, opposite = {opposite}, color = {color}");

            var response = SendRequest<ResponseBase>(new OrderCloseByRequest
            {
                Ticket = ticket,
                Opposite = opposite,
                ArrowColor = MT4APIColorConverter.ConvertToMT4Color(color)
            });
            return response != null;
        }

        public bool OrderCloseBy(int ticket, int opposite)
        {
            Log.Debug($"OrderCloseBy: ticket = {ticket}, opposite = {opposite}");

            var response = SendRequest<ResponseBase>(new OrderCloseByRequest
            {
                Ticket = ticket,
                Opposite = opposite,
            });
            return response != null;
        }

        public bool OrderDelete(int ticket, Color color)
        {
            Log.Debug($"OrderDelete: ticket = {ticket}, color = {color}");

            var response = SendRequest<ResponseBase>(new OrderDeleteRequest
            {
                Ticket = ticket,
                ArrowColor = MT4APIColorConverter.ConvertToMT4Color(color),
            });
            return response != null;
        }

        public bool OrderDelete(int ticket)
        {
            Log.Debug($"OrderDelete: ticket = {ticket}");

            var response = SendRequest<ResponseBase>(new OrderDeleteRequest
            {
                Ticket = ticket,
            });
            return response != null;
        }

        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, DateTime expiration, Color arrowColor)
        {
            Log.Debug($"OrderModify: ticket = {ticket}, price = {price}, stoploss = {stoploss}, takeprofit = {takeprofit}, expiration = {expiration}, arrowColor = {arrowColor}");

            var response = SendRequest<ResponseBase>(new OrderModifyRequest
            {
                Ticket = ticket,
                Price = price,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Expiration = MT4APITimeConverter.ConvertToMTTime(expiration),
                ArrowColor = MT4APIColorConverter.ConvertToMT4Color(arrowColor)
            });
            return response != null;
        }

        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, DateTime expiration)
        {
            Log.Debug($"OrderModify: ticket = {ticket}, price = {price}, stoploss = {stoploss}, takeprofit = {takeprofit}, expiration = {expiration}");

            var response = SendRequest<ResponseBase>(new OrderModifyRequest
            {
                Ticket = ticket,
                Price = price,
                StopLoss = stoploss,
                TakeProfit = takeprofit,
                Expiration = MT4APITimeConverter.ConvertToMTTime(expiration),
            });
            return response != null;
        }

        public void OrderPrint()
        {
            SendCommand<object>(MT4CommandType.OrderPrint, null);
        }

        public bool OrderSelect(int index, OrderSelectMode select, OrderSelectSource pool)
        {
            Log.Debug($"OrderSelect: index = {index}, select = {select}, pool = {pool}");

            var commandParameters = new ArrayList { index, (int)select, (int)pool };
            return SendCommand<bool>(MT4CommandType.OrderSelect, commandParameters);
        }

        public bool OrderSelect(int index, OrderSelectMode select)
        {
            return OrderSelect(index, select, OrderSelectSource.MODE_TRADES);
        }

        public int OrdersHistoryTotal()
        {
            return SendCommand<int>(MT4CommandType.OrdersHistoryTotal, null);
        }

        public int OrdersTotal()
        {
            return SendCommand<int>(MT4CommandType.OrdersTotal, null);
        }

        public bool OrderCloseAll()
        {
            return SendCommand<bool>(MT4CommandType.OrderCloseAll, null);
        }

        public MT4Order GetOrder(int index, OrderSelectMode select, OrderSelectSource pool)
        {
            var response = SendRequest<GetOrderResponse>(new GetOrderRequest { Index = index, Select = (int) select, Pool = (int) pool});
            return response?.Order;
        }

        public List<MT4Order> GetOrders(OrderSelectSource pool)
        {
            var response = SendRequest<GetOrdersResponse>(new GetOrdersRequest { Pool = (int)pool });
            return response != null ? response.Orders : new List<MT4Order>();
        }
        #endregion

        #region Checkup

        ///<summary>
        ///Returns the contents of the system variable _LastError.
        ///After the function call, the contents of _LastError are reset.
        ///</summary>
        ///<returns>
        ///Returns the value of the last error that occurred during the execution of an mql4 program.
        ///</returns>
        public int GetLastError()
        {
            return SendCommand<int>(MT4CommandType.GetLastError, null);
        }

        ///<summary>
        ///Checks connection between client terminal and server.
        ///</summary>
        ///<returns>
        ///It returns true if connection to the server was successfully established, otherwise, it returns false.
        ///</returns>
        public bool IsConnected()
        {
            return SendCommand<bool>(MT4CommandType.IsConnected, null);
        }

        ///<summary>
        ///Checks if the Expert Advisor runs on a demo account.
        ///</summary>
        ///<returns>
        ///Returns true if the Expert Advisor runs on a demo account, otherwise returns false.
        ///</returns>
        public bool IsDemo()
        {
            return SendCommand<bool>(MT4CommandType.IsDemo, null);
        }

        ///<summary>
        ///Checks if the DLL function call is allowed for the Expert Advisor.
        ///</summary>
        ///<returns>
        ///Returns true if the DLL function call is allowed for the Expert Advisor, otherwise returns false.
        ///</returns>
        public bool IsDllsAllowed()
        {
            return SendCommand<bool>(MT4CommandType.IsDllsAllowed, null);
        }

        ///<summary>
        ///Checks if Expert Advisors are enabled for running.
        ///</summary>
        ///<returns>
        ///Returns true if Expert Advisors are enabled for running, otherwise returns false.
        ///</returns>
        public bool IsExpertEnabled()
        {
            return SendCommand<bool>(MT4CommandType.IsExpertEnabled, null);
        }

        ///<summary>
        ///Checks if the Expert Advisor can call library function.
        ///</summary>
        ///<returns>
        ///Returns true if the Expert Advisor can call library function, otherwise returns false.
        ///</returns>
        public bool IsLibrariesAllowed()
        {
            return SendCommand<bool>(MT4CommandType.IsLibrariesAllowed, null);
        }

        ///<summary>
        ///Checks if Expert Advisor runs in the Strategy Tester optimization mode.
        ///</summary>
        ///<returns>
        ///Returns true if Expert Advisor runs in the Strategy Tester optimization mode, otherwise returns false.
        ///</returns>
        public bool IsOptimization()
        {
            return SendCommand<bool>(MT4CommandType.IsOptimization, null);
        }

        ///<summary>
        ///Checks the forced shutdown of an mql4 program.
        ///</summary>
        ///<returns>
        ///Returns true, if the _StopFlag system variable contains a value other than 0. 
        ///A nonzero value is written into _StopFlag, if a mql4 program has been commanded to complete its operation. 
        ///In this case, you must immediately terminate the program, otherwise the program will be completed 
        ///forcibly from the outside after 3 seconds.
        ///</returns>
        public bool IsStopped()
        {
            return SendCommand<bool>(MT4CommandType.IsStopped, null);
        }

        ///<summary>
        ///Checks if the Expert Advisor runs in the testing mode.
        ///</summary>
        ///<returns>
        ///Returns true if the Expert Advisor runs in the testing mode, otherwise returns false.
        ///</returns>
        public bool IsTesting()
        {
            return SendCommand<bool>(MT4CommandType.IsTesting, null);
        }

        ///<summary>
        ///Checks if the Expert Advisor is allowed to trade and trading context is not busy.
        ///</summary>
        ///<returns>
        ///Returns true if the Expert Advisor is allowed to trade and trading context is not busy, otherwise returns false.
        ///</returns>
        public bool IsTradeAllowed()
        {
            return SendCommand<bool>(MT4CommandType.IsTradeAllowed, null);
        }

        ///<summary>
        ///Returns the information about trade context.
        ///</summary>
        ///<returns>
        ///Returns true if a thread for trading is occupied by another Expert Advisor, otherwise returns false.
        ///</returns>
        public bool IsTradeContextBusy()
        {
            return SendCommand<bool>(MT4CommandType.IsTradeContextBusy, null);
        }

        ///<summary>
        ///Checks if the Expert Advisor is tested in visual mode.
        ///</summary>
        ///<returns>
        ///Returns true if the Expert Advisor is tested with checked "Visual Mode" button, otherwise returns false.
        ///</returns>
        public bool IsVisualMode()
        {
            return SendCommand<bool>(MT4CommandType.IsVisualMode, null);
        }

        ///<summary>
        ///Returns the code of a reason for deinitialization.
        ///</summary>
        ///<returns>
        ///Returns the value of _UninitReason which is formed before OnDeinit() is called. 
        ///Value depends on the reasons that led to deinitialization.
        ///</returns>
        public int UninitializeReason()
        {
            return SendCommand<int>(MT4CommandType.UninitializeReason, null);
        }

        ///<summary>
        ///Print the error description.
        ///</summary>
        public string ErrorDescription(int errorCode)
        {
            var commandParameters = new ArrayList { errorCode };
            return SendCommand<string>(MT4CommandType.ErrorDescription, commandParameters);
        }

        ///<summary>
        ///Returns the value of a corresponding property of the mql4 program environment. 
        ///</summary>
        ///<param name="propertyId">Identifier of a property. Can be one of the values of the ENUM_TERMINAL_INFO_STRING enumeration.</param>
        ///<returns>
        ///Value of string type.
        ///</returns>
        public string TerminalInfoString(EnumTerminalInfoString propertyId)
        {
            var commandParameters = new ArrayList { (int)propertyId };
            return SendCommand<string>(MT4CommandType.TerminalInfoString, commandParameters);
        }

        ///<summary>
        ///Returns the value of a corresponding property of the mql4 program environment.
        ///</summary>
        ///<param name="propertyId">Identifier of a property. Can be one of the values of the ENUM_TERMINAL_INFO_INTEGER enumeration.</param>
        ///<returns>
        ///Value of int type.
        ///</returns>
        public int TerminalInfoInteger(EnumTerminalInfoInteger propertyId)
        {
            var commandParameters = new ArrayList { (int)propertyId };
            return SendCommand<int>(MT4CommandType.TerminalInfoInteger, commandParameters);
        }

        ///<summary>
        ///Returns the value of a corresponding property of the mql4 program environment.
        ///</summary>
        ///<param name="propertyId">Identifier of a property. Can be one of the values of the ENUM_TERMINAL_INFO_DOUBLE enumeration.</param>
        ///<returns>
        ///Value of double type.
        ///</returns>
        public double TerminalInfoDouble(EnumTerminalInfoDouble propertyId)
        {
            var commandParameters = new ArrayList { (int)propertyId };
            return SendCommand<double>(MT4CommandType.TerminalInfoDouble, commandParameters);
        }

        ///<summary>
        ///Returns the name of company owning the client terminal.
        ///</summary>
        ///<returns>
        ///The name of company owning the client terminal.
        ///</returns>
        public string TerminalCompany()
        {
            return SendCommand<string>(MT4CommandType.TerminalCompany, null);
        }

        ///<summary>
        ///Returns client terminal name.
        ///</summary>
        ///<returns>
        ///Client terminal name.
        ///</returns>
        public string TerminalName()
        {
            return SendCommand<string>(MT4CommandType.TerminalName, null);
        }

        ///<summary>
        ///Returns the directory, from which the client terminal was launched.
        ///</summary>
        ///<returns>
        ///The directory, from which the client terminal was launched.
        ///</returns>
        public string TerminalPath()
        {
            return SendCommand<string>(MT4CommandType.TerminalPath, null);
        }

        #endregion

        #region Account functions

        public double AccountBalance()
        {
            return SendCommand<double>(MT4CommandType.AccountBalance, null);
        }

        public double AccountCredit()
        {
            return SendCommand<double>(MT4CommandType.AccountCredit, null);
        }

        public string AccountCompany()
        {
            return SendCommand<string>(MT4CommandType.AccountCompany, null);
        }

        public string AccountCurrency()
        {
            return SendCommand<string>(MT4CommandType.AccountCurrency, null);
        }

        public double AccountEquity()
        {
            return SendCommand<double>(MT4CommandType.AccountEquity, null);
        }

        public double AccountFreeMargin()
        {
            return SendCommand<double>(MT4CommandType.AccountFreeMargin, null);
        }

        public double AccountFreeMarginCheck(string symbol, TradeOperation cmd, double volume)
        {
            var commandParameters = new ArrayList { symbol, (int)cmd, volume };
            return SendCommand<double>(MT4CommandType.AccountFreeMarginCheck, commandParameters);
        }

        public double AccountFreeMarginMode()
        {
            return SendCommand<double>(MT4CommandType.AccountFreeMarginMode, null);
        }

        public int AccountLeverage()
        {
            return SendCommand<int>(MT4CommandType.AccountLeverage, null);
        }

        public double AccountMargin()
        {
            return SendCommand<double>(MT4CommandType.AccountMargin, null);
        }

        public string AccountName()
        {
            return SendCommand<string>(MT4CommandType.AccountName, null);
        }

        public int AccountNumber()
        {
            return SendCommand<int>(MT4CommandType.AccountNumber, null);
        }

        public double AccountProfit()
        {
            return SendCommand<double>(MT4CommandType.AccountProfit, null);
        }

        public string AccountServer()
        {
            return SendCommand<string>(MT4CommandType.AccountServer, null);
        }

        public int AccountStopoutLevel()
        {
            return SendCommand<int>(MT4CommandType.AccountStopoutLevel, null);
        }

        public int AccountStopoutMode()
        {
            return SendCommand<int>(MT4CommandType.AccountStopoutMode, null);
        }

        public bool ChangeAccount(string login, string password, string host)
        {
            var commandParameters = new ArrayList { login, password, host};
            return SendCommand<bool>(MT4CommandType.ChangeAccount, commandParameters);
        }

        #endregion

        #region Common Function

        public void Alert(string msg)
        {
            var commandParameters = new ArrayList { msg };
            SendCommand<object>(MT4CommandType.Alert, commandParameters);
        }

        public void Comment(string msg)
        {
            var commandParameters = new ArrayList { msg };
            SendCommand<object>(MT4CommandType.Comment, commandParameters);
        }

        public int GetTickCount()
        {
            return SendCommand<int>(MT4CommandType.GetTickCount, null);
        }

        public int MessageBox(string text, string caption, int flag)
        {
            var commandParameters = new ArrayList { text, caption, flag };
            return SendCommand<int>(MT4CommandType.MessageBoxA, commandParameters);
        }

        public int MessageBox(string text, string caption)
        {
            return MessageBox(text, caption, EMPTY);
        }

        public int MessageBox(string text)
        {
            var commandParameters = new ArrayList { text };
            return SendCommand<int>(MT4CommandType.MessageBox, commandParameters);
        }

        public bool PlaySound(string filename)
        {
            var commandParameters = new ArrayList { filename };
            return SendCommand<bool>(MT4CommandType.PlaySound, commandParameters);
        }

        public void Print(string msg)
        {
            var commandParameters = new ArrayList { msg };
            SendCommand<object>(MT4CommandType.Print, commandParameters);
        }

        public bool SendFTP(string filename)
        {
            var commandParameters = new ArrayList { filename };
            return SendCommand<bool>(MT4CommandType.SendFTP, commandParameters);
        }

        public bool SendFTP(string filename, string ftpPath)
        {
            var commandParameters = new ArrayList { filename, ftpPath };
            return SendCommand<bool>(MT4CommandType.SendFTPA, commandParameters);
        }

        public bool SendMail(string subject, string someText)
        {
            var commandParameters = new ArrayList { subject, someText };
            return SendCommand<bool>(MT4CommandType.SendMail, commandParameters);
        }

        public void Sleep(int milliseconds)
        {
            var commandParameters = new ArrayList { milliseconds };
            SendCommand<object>(MT4CommandType.Sleep, commandParameters);
        }

        #endregion

        #region Date and Time Functions

        public int Day()
        {
            return SendCommand<int>(MT4CommandType.Day, null);
        }

        public int DayOfWeek()
        {
            return SendCommand<int>(MT4CommandType.DayOfWeek, null);
        }

        public int DayOfYear()
        {
            return SendCommand<int>(MT4CommandType.DayOfYear, null);
        }

        public int Hour()
        {
            return SendCommand<int>(MT4CommandType.Hour, null);
        }

        public int Minute()
        {
            return SendCommand<int>(MT4CommandType.Minute, null);
        }

        public int Month()
        {
            return SendCommand<int>(MT4CommandType.Month, null);
        }

        public int Seconds()
        {
            return SendCommand<int>(MT4CommandType.Seconds, null);
        }

        public DateTime TimeCurrent()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.TimeCurrent, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        public DateTime TimeGMT()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.TimeGMT, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        public int TimeDay(DateTime date)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(date) };
            return SendCommand<int>(MT4CommandType.TimeDay, commandParameters);
        }

        public int TimeDayOfWeek(DateTime date)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(date) };
            return SendCommand<int>(MT4CommandType.TimeDayOfWeek, commandParameters);
        }

        public int TimeDayOfYear(DateTime date)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(date) };
            return SendCommand<int>(MT4CommandType.TimeDayOfYear, commandParameters);
        }

        public int TimeHour(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.TimeHour, commandParameters);
        }

        public DateTime TimeLocal()
        {
            var commandResponse = SendCommand<int>(MT4CommandType.TimeLocal, null);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        public int TimeMinute(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.TimeMinute, commandParameters);
        }

        public int TimeMonth(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.TimeMonth, commandParameters);
        }

        public int TimeSeconds(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.TimeSeconds, commandParameters);
        }

        public int TimeYear(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.TimeYear, commandParameters);
        }

        public int Year(DateTime time)
        {
            var commandParameters = new ArrayList { MT4APITimeConverter.ConvertToMTTime(time) };
            return SendCommand<int>(MT4CommandType.Year, commandParameters);
        }

        #endregion

        #region Global Variables Functions
        public bool GlobalVariableCheck(string name)
        {
            var commandParameters = new ArrayList { name };
            return SendCommand<bool>(MT4CommandType.GlobalVariableCheck, commandParameters);
        }

        public bool GlobalVariableDel(string name)
        {
            var commandParameters = new ArrayList { name };
            return SendCommand<bool>(MT4CommandType.GlobalVariableDel, commandParameters);
        }

        public double GlobalVariableGet(string name)
        {
            var commandParameters = new ArrayList { name };
            return SendCommand<double>(MT4CommandType.GlobalVariableGet, commandParameters);
        }

        public string GlobalVariableName(int index)
        {
            var commandParameters = new ArrayList { index };
            return SendCommand<string>(MT4CommandType.GlobalVariableName, commandParameters);
        }

        public DateTime GlobalVariableSet(string name, double value)
        {
            var commandParameters = new ArrayList { name, value };
            var commandResponse = SendCommand<int>(MT4CommandType.GlobalVariableSet, commandParameters);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        public bool GlobalVariableSetOnCondition(string name, double value, double checkValue)
        {
            var commandParameters = new ArrayList { name, value, checkValue };
            return SendCommand<bool>(MT4CommandType.GlobalVariableSetOnCondition, commandParameters);
        }

        public int GlobalVariablesDeleteAll(string prefixName)
        {
            var commandParameters = new ArrayList { prefixName };
            return SendCommand<int>(MT4CommandType.GlobalVariableSetOnCondition, commandParameters);
        }

        public int GlobalVariablesTotal()
        {
            return SendCommand<int>(MT4CommandType.GlobalVariablesTotal, null);
        }

        #endregion

        #region Technical Indicators
        public double iAC(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iAC, commandParameters);
        }

        public double iAD(string symbol, int timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, shift };
            return SendCommand<double>(MT4CommandType.iAD, commandParameters);
        }

        public double iAlligator(string symbol, int timeframe, int jawPeriod, int jawShift, int teethPeriod, int teethShift, int lipsPeriod, int lipsShift, int maMethod, int appliedPrice, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, jawPeriod, jawShift, teethPeriod, teethShift, lipsPeriod, lipsShift, maMethod, appliedPrice, mode, shift };
            return SendCommand<double>(MT4CommandType.iAlligator, commandParameters);
        }

        public double iADX(string symbol, int timeframe, int period, int appliedPrice, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, mode, shift };
            return SendCommand<double>(MT4CommandType.iADX, commandParameters);
        }

        public double iATR(string symbol, int timeframe, int period, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, shift };
            return SendCommand<double>(MT4CommandType.iATR, commandParameters);
        }

        public double iAO(string symbol, int timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, shift };
            return SendCommand<double>(MT4CommandType.iAO, commandParameters);
        }

        public double iBearsPower(string symbol, int timeframe, int period, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iBearsPower, commandParameters);
        }

        public double iBands(string symbol, int timeframe, int period, int deviation, int bandsShift, int appliedPrice, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, deviation, bandsShift, appliedPrice, mode, shift };
            return SendCommand<double>(MT4CommandType.iBands, commandParameters);
        }

        public double iBandsOnArray(double[] array, int total, int period, int deviation, int bandsShift, int mode, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(period);
            commandParameters.Add(deviation);
            commandParameters.Add(bandsShift);
            commandParameters.Add(mode);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iBandsOnArray, commandParameters);
        }

        public double iBullsPower(string symbol, int timeframe, int period, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iBullsPower, commandParameters);
        }

        public double iCCI(string symbol, int timeframe, int period, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iCCI, commandParameters);
        }

        public double iCCIOnArray(double[] array, int total, int period, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] { });
            commandParameters.Add(total);
            commandParameters.Add(period);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iCCIOnArray, commandParameters);
        }

        public double iCustom(string symbol, int timeframe, string name, int[] parameters, int mode, int shift)
        {
            var response = SendRequest<ICustomResponse>(new ICustomRequest
            {
                Symbol = symbol,
                Timeframe = timeframe,
                Name = name,
                Mode = mode,
                Shift = shift,
                Params = new ArrayList(parameters),
                ParamsType = ICustomRequest.ParametersType.Int
            });
            return response?.Value ?? double.NaN;
        }

        public double iCustom(string symbol, int timeframe, string name, double[] parameters, int mode, int shift)
        {
            var response = SendRequest<ICustomResponse>(new ICustomRequest
            {
                Symbol = symbol,
                Timeframe = timeframe,
                Name = name,
                Mode = mode,
                Shift = shift,
                Params = new ArrayList(parameters),
                ParamsType = ICustomRequest.ParametersType.Double
            });
            return response?.Value ?? double.NaN;
        }

        public double iCustom(string symbol, int timeframe, string name, int mode, int shift)
        {
            var response = SendRequest<ICustomResponse>(new ICustomRequest
            {
                Symbol = symbol,
                Timeframe = timeframe,
                Name = name,
                Mode = mode,
                Shift = shift
            });
            return response?.Value ?? double.NaN;
        }

        public double iDeMarker(string symbol, int timeframe, int period, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, shift };
            return SendCommand<double>(MT4CommandType.iDeMarker, commandParameters);
        }

        public double iEnvelopes(string symbol, int timeframe, int maPeriod, int maMethod, int maShift, int appliedPrice, double deviation, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, maPeriod, maMethod, maShift, appliedPrice, deviation, mode, shift };
            return SendCommand<double>(MT4CommandType.iEnvelopes, commandParameters);
        }

        public double iEnvelopesOnArray(double[] array, int total, int maPeriod, int maMethod, int maShift, double deviation, int mode, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(maPeriod);
            commandParameters.Add(maMethod);
            commandParameters.Add(maShift);
            commandParameters.Add(deviation);
            commandParameters.Add(mode);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iEnvelopesOnArray, commandParameters);
        }

        public double iForce(string symbol, int timeframe, int period, int maMethod, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, maMethod, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iForce, commandParameters);
        }

        public double iFractals(string symbol, int timeframe, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, mode, shift };
            return SendCommand<double>(MT4CommandType.iFractals, commandParameters);
        }

        public double iGator(string symbol, int timeframe, int jawPeriod, int jawShift, int teethPeriod, int teethShift, int lipsPeriod, int lipsShift, int maMethod, int appliedPrice, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, jawPeriod, jawShift, teethPeriod, teethShift, lipsPeriod, lipsShift, maMethod, appliedPrice, mode, shift };
            return SendCommand<double>(MT4CommandType.iGator, commandParameters);
        }

        public double iIchimoku(string symbol, int timeframe, int tenkanSen, int kijunSen, int senkouSpanB, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, tenkanSen, kijunSen, senkouSpanB, mode, shift };
            return SendCommand<double>(MT4CommandType.iIchimoku, commandParameters);
        }

        public double iBWMFI(string symbol, int timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, shift };
            return SendCommand<double>(MT4CommandType.iBWMFI, commandParameters);
        }

        public double iMomentum(string symbol, int timeframe, int period, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iMomentum, commandParameters);
        }

        public double iMomentumOnArray(double[] array, int total, int period, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(period);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iMomentumOnArray, commandParameters);
        }

        public double iMFI(string symbol, int timeframe, int period, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, shift };
            return SendCommand<double>(MT4CommandType.iMFI, commandParameters);
        }

        public double iMA(string symbol, int timeframe, int period, int maShift, int maMethod, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, maShift, maMethod, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iMA, commandParameters);
        }

        public double iMAOnArray(double[] array, int total, int period, int maShift, int maMethod, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(period);
            commandParameters.Add(maShift);
            commandParameters.Add(maMethod);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iMAOnArray, commandParameters);
        }

        public double iOsMA(string symbol, int timeframe, int fastEmaPeriod, int slowEmaPeriod, int signalPeriod, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, fastEmaPeriod, slowEmaPeriod, signalPeriod, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iOsMA, commandParameters);
        }

        public double iMACD(string symbol, int timeframe, int fastEmaPeriod, int slowEmaPeriod, int signalPeriod, int appliedPrice, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, fastEmaPeriod, slowEmaPeriod, signalPeriod, appliedPrice, mode, shift };
            return SendCommand<double>(MT4CommandType.iMACD, commandParameters);
        }

        public double iOBV(string symbol, int timeframe, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iOBV, commandParameters);
        }

        public double iSAR(string symbol, int timeframe, double step, double maximum, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, step, maximum, shift };
            return SendCommand<double>(MT4CommandType.iSAR, commandParameters);
        }

        public double iRSI( string symbol, int timeframe, int period, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iRSI, commandParameters);
        }

        public double iRSIOnArray(double[] array, int total, int period, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(period);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iMomentumOnArray, commandParameters);
        }

        public double iRVI(string symbol, int timeframe, int period, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, mode, shift };
            return SendCommand<double>(MT4CommandType.iRVI, commandParameters);
        }

        public double iStdDev(string symbol, int timeframe, int maPeriod, int maShift, int maMethod, int appliedPrice, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, maPeriod, maShift, maMethod, appliedPrice, shift };
            return SendCommand<double>(MT4CommandType.iStdDev, commandParameters);
        }

        public double iStdDevOnArray(double[] array, int total, int maPeriod, int maShift, int maMethod, int shift)
        {
            var arraySize = array?.Length ?? 0;
            var commandParameters = new ArrayList { arraySize };
            commandParameters.AddRange(array ?? new double[] {});
            commandParameters.Add(total);
            commandParameters.Add(maPeriod);
            commandParameters.Add(maShift);
            commandParameters.Add(maMethod);
            commandParameters.Add(shift);

            return SendCommand<double>(MT4CommandType.iStdDevOnArray, commandParameters);
        }

        public double iStochastic(string symbol, int timeframe, int pKperiod, int pDperiod, int slowing, int method, int priceField, int mode, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, pKperiod, pDperiod, slowing, method, priceField, mode, shift };
            return SendCommand<double>(MT4CommandType.iStochastic, commandParameters);
        }

        public double iWPR(string symbol, int timeframe, int period, int shift)
        {
            var commandParameters = new ArrayList { symbol, timeframe, period, shift };
            return SendCommand<double>(MT4CommandType.iWPR, commandParameters);
        }
        #endregion

        #region Timeseries access
        public int iBars(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<int>(MT4CommandType.iBars, commandParameters);
        }

        public int iBarShift(string symbol, ChartPeriod timeframe, DateTime time, bool exact)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, MT4APITimeConverter.ConvertToMTTime(time), exact };
            return SendCommand<int>(MT4CommandType.iBarShift, commandParameters);
        }

        public double iClose(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iClose, commandParameters);
        }

        public double iHigh(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iHigh, commandParameters);
        }

        public int iHighest(string symbol, ChartPeriod timeframe, SeriesIdentifier type, int count, int start)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, (int)type, count, start };
            return SendCommand<int>(MT4CommandType.iHighest, commandParameters);
        }

        public double iLow(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iLow, commandParameters);
        }

        public int iLowest(string symbol, ChartPeriod timeframe, SeriesIdentifier type, int count, int start)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, (int)type, count, start };
            return SendCommand<int>(MT4CommandType.iLowest, commandParameters);
        }

        public double iOpen(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iOpen, commandParameters);
        }

        public DateTime iTime(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            var commandResponse = SendCommand<int>(MT4CommandType.iTime, commandParameters);
            return MT4APITimeConverter.ConvertFromMTTime(commandResponse);
        }

        public double iVolume(string symbol, ChartPeriod timeframe, int shift)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe, shift };
            return SendCommand<double>(MT4CommandType.iVolume, commandParameters);
        }

        //public double[] iCloseArray(string symbol, ChartPeriod timeframe, int shift, int valueCount)
        //{
        //    int doubleArraySendLimit = DoubleArrayLimit;
        //    int limitCount = valueCount / doubleArraySendLimit;
        //    int valueCountModulo = valueCount - limitCount * doubleArraySendLimit;

        //    var resultArray = new double[valueCount];
        //    for (int i = 0; i < limitCount; i++)
        //    {
        //        var commandParameters = new ArrayList { symbol, (int)timeframe, i * doubleArraySendLimit, doubleArraySendLimit };
        //        var result = SendCommand<double[]>(MtCommandType.iCloseArray, commandParameters);
        //        if (result != null)
        //            Array.Copy(result, 0, resultArray, i * doubleArraySendLimit, doubleArraySendLimit);
        //    }

        //    if (valueCountModulo > 0)
        //    {
        //        var commandParameters = new ArrayList { symbol, (int)timeframe, limitCount * doubleArraySendLimit, valueCountModulo };
        //        var result = SendCommand<double[]>(MtCommandType.iCloseArray, commandParameters);
        //        if (result != null)
        //            Array.Copy(result, 0, resultArray, limitCount * doubleArraySendLimit, valueCountModulo);
        //    }
        //    return resultArray;

        //    var commandParameters = new ArrayList { symbol, (int)timeframe, shift, valueCount };
        //    return SendCommand<double[]>(MtCommandType.iCloseArray, commandParameters);
        //}

        public double[] iCloseArray(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<double[]>(MT4CommandType.iCloseArray, commandParameters);
        }

        public double[] iHighArray(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<double[]>(MT4CommandType.iHighArray, commandParameters);
        }

        public double[] iLowArray(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<double[]>(MT4CommandType.iLowArray, commandParameters);
        }

        public double[] iOpenArray(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<double[]>(MT4CommandType.iOpenArray, commandParameters);
        }

        public double[] iVolumeArray(string symbol, ChartPeriod timeframe)
        {
            var commandParameters = new ArrayList { symbol, (int)timeframe };
            return SendCommand<double[]>(MT4CommandType.iVolumeArray, commandParameters);
        }

        public DateTime[] iTimeArray(string symbol, ChartPeriod timeframe)
        {
            DateTime[] result = null;

            var commandParameters = new ArrayList { symbol, (int)timeframe };
            
            var response = SendCommand<int[]>(MT4CommandType.iTimeArray, commandParameters);

            if (response != null)
            {
                result = new DateTime[response.Length];

                for(var i = 0; i < response.Length; i++)
                {
                    result[i] = MT4APITimeConverter.ConvertFromMTTime(response[i]);
                }
            }

            return result;
        }

        public bool RefreshRates()
        {
            return SendCommand<bool>(MT4CommandType.RefreshRates, null);
        }

        public List<MQLRates> CopyRates(string symbolName, EnumTimeframes timeframe, int startPos, int count)
        {
            var response = SendRequest<CopyRatesResponse>(new CopyRates1Request
            {
                SymbolName = symbolName,
                Timeframe = timeframe,
                StartPos = startPos,
                Count = count
            });
            return response?.Rates;
        }

        public List<MQLRates> CopyRates(string symbolName, EnumTimeframes timeframe, DateTime startTime, int count)
        {
            var response = SendRequest<CopyRatesResponse>(new CopyRates2Request
            {
                SymbolName = symbolName,
                Timeframe = timeframe,
                StartTime = MT4APITimeConverter.ConvertToMTTime(startTime),
                Count = count
            });
            return response?.Rates;
        }

        public List<MQLRates> CopyRates(string symbolName, EnumTimeframes timeframe, DateTime startTime, DateTime stopTime)
        {
            var response = SendRequest<CopyRatesResponse>(new CopyRates3Request
            {
                SymbolName = symbolName,
                Timeframe = timeframe,
                StartTime = MT4APITimeConverter.ConvertToMTTime(startTime),
                StopTime = MT4APITimeConverter.ConvertToMTTime(stopTime)
            });
            return response?.Rates;
        }

        ///<summary>
        ///Returns information about the state of historical data.
        ///</summary>
        ///<param name="symbolName">Symbol name.</param>
        ///<param name="timeframe">Period.</param>
        ///<param name="propId">Identifier of the requested property, value of the ENUM_SERIES_INFO_INTEGER enumeration.</param>
        ///<returns>
        ///Returns value of the long type.
        ///</returns>
        public long SeriesInfoInteger(string symbolName, EnumTimeframes timeframe, EnumSeriesInfoInteger propId)
        {
            var response = SendRequest<SeriesInfoIntegerResponse>(new SeriesInfoIntegerRequest
            {
                SymbolName = symbolName,
                Timeframe = (int)timeframe,
                PropID = (int)propId
            });

            return response?.Value ?? 0;
        }

        #endregion

        #region Market Info

        ///<summary>
        ///Returns various data about securities listed in the "Market Watch" window.
        ///</summary>
        ///<param name="symbol">Symbol name.</param>
        ///<param name="type">Request identifier that defines the type of information to be returned. Can be any of values of request identifiers.</param>
        ///<returns>
        ///Returns various data about securities listed in the "Market Watch" window.
        ///</returns>
        public double MarketInfo(string symbol, MarketInfoModeType type)
        {
            var commandParameters = new ArrayList { symbol, (int)type };
            return SendCommand<double>(MT4CommandType.MarketInfo, commandParameters);
        }

        ///<summary>
        ///Returns the number of available (selected in Market Watch or all) symbols.
        ///</summary>
        ///<param name="selected">Request mode. Can be true or false.</param>
        ///<returns>
        ///If the 'selected' parameter is true, the function returns the number of symbols selected in MarketWatch. If the value is false, it returns the total number of all symbols.
        ///</returns>
        public int SymbolsTotal(bool selected)
        {
            var commandParameters = new ArrayList { selected };
            return SendCommand<int>(MT4CommandType.SymbolsTotal, commandParameters);
        }

        ///<summary>
        ///Returns the name of a symbol.
        ///</summary>
        ///<param name="pos">Order number of a symbol.</param>
        ///<param name="selected">Request mode. If the value is true, the symbol is taken from the list of symbols selected in MarketWatch. If the value is false, the symbol is taken from the general list.</param>
        ///<returns>
        ///Value of string type with the symbol name.
        ///</returns>
        public string SymbolName(int pos, bool selected)
        {
            var commandParameters = new ArrayList { pos, selected };
            return SendCommand<string>(MT4CommandType.SymbolName, commandParameters);
        }

        ///<summary>
        ///Selects a symbol in the Market Watch window or removes a symbol from the window.
        ///</summary>
        ///<param name="name">Symbol name</param>
        ///<param name="select">Switch. If the value is false, a symbol should be removed from MarketWatch, otherwise a symbol should be selected in this window. A symbol can't be removed if the symbol chart is open, or there are open orders for this symbol.</param>
        ///<returns>
        ///In case of failure returns false.
        ///</returns>
        public bool SymbolSelect(string name, bool select)
        {
            var commandParameters = new ArrayList { name, select };
            return SendCommand<bool>(MT4CommandType.SymbolSelect, commandParameters);
        }

        ///<summary>
        ///Returns the corresponding property of a specified symbol.
        ///</summary>
        ///<param name="name">Symbol name</param>
        ///<param name="propId">Identifier of a symbol property. The value can be one of the values of the EnumSymbolInfoInteger enumeration</param>
        ///<returns>
        ///The value of long type.
        ///</returns>
        public long SymbolInfoInteger(string name, EnumSymbolInfoInteger propId)
        {
            var commandParameters = new ArrayList { name, (int)propId };
            return SendCommand<long>(MT4CommandType.SymbolInfoInteger, commandParameters);
        }

        ///<summary>
        ///Returns the corresponding property of a specified symbol.
        ///</summary>
        ///<param name="name">Symbol name</param>
        ///<param name="propId">Identifier of a symbol property. The value can be one of the values of the ENUM_SYMBOL_INFO_STRING enumeration.</param>
        ///<returns>
        ///The value of string type.
        ///</returns>
        public string SymbolInfoString(string name, EnumSymbolInfoString propId)
        {
            var commandParameters = new ArrayList { name, (int)propId };
            return SendCommand<string>(MT4CommandType.SymbolInfoString, commandParameters);
        }

        ///<summary>
        ///Allows receiving time of beginning and end of the specified quoting/trading  sessions for a specified symbol and day of week.
        /// 
        ///</summary>
        ///<param name="symbol">Symbol name.</param>
        ///<param name="dayOfWeek">Day of the week.</param>
        ///<param name="index">Ordinal number of a session, whose beginning and end time we want to receive. Indexing of sessions starts with 0.</param>
        ///<param name="type">Session type: Quote, Trade</param>
        ///<returns>
        ///The value session.
        ///</returns>
        public MT4Session SymbolInfoSession(string symbol, DayOfWeek dayOfWeek, uint index, SessionType type)
        {
            var responce = SendRequest<SessionResponse>(new SessionRequest { Symbol = symbol, DayOfWeek = dayOfWeek, SessionIndex = (int)index, SessionType = type });
            return responce?.Session;
        }

        ///<summary>
        ///Returns the corresponding property of a specified symbol.
        ///</summary>
        ///<param name="symbolName">Symbol name.</param>
        ///<param name="propId">Identifier of a symbol property. The value can be one of the values of the ENUM_SYMBOL_INFO_DOUBLE enumeration.</param>
        ///<returns>
        /// The value of double type.
        ///</returns>
        public double SymbolInfoDouble(string symbolName, EnumSymbolInfoDouble propId)
        {
            var response = SendRequest<SymbolInfoDoubleResponse>(new SymbolInfoDoubleRequest
            {
                SymbolName = symbolName,
                PropID = (int)propId
            });

            return response?.Value ?? 0;
        }

        ///<summary>
        ///Returns the corresponding property of a specified symbol.
        ///</summary>
        ///<param name="symbol">Symbol name.</param>
        ///<returns>
        /// MqlTick object, to which the current prices and time of the last price update will be placed.
        ///</returns>
        public MQLTick SymbolInfoTick(string symbol)
        {
            var response = SendRequest<SymbolInfoTickResponse>(new SymbolInfoTickRequest
            {
                Symbol = symbol
            });

            return response?.Tick;
        }
        #endregion

        #region Chart Operations

        ///<summary>
        ///Returns the ID of the current chart.
        ///</summary>
        ///<returns>
        /// Value of long type.
        ///</returns>
        public long ChartId()
        {
            return SendCommand<long>(MT4CommandType.ChartId, null);
        }

        ///<summary>
        ///This function calls a forced redrawing of a specified chart.
        ///</summary>
        public void ChartRedraw(long chartId = 0)
        {
            var commandParameters = new ArrayList { chartId };
            SendCommand<object>(MT4CommandType.ChartRedraw, commandParameters);
        }

        ///<summary>
        ///Applies a specific template from a specified file to the chart.
        ///</summary>
        ///<param name="chartId">Chart ID.</param>
        ///<param name="filename">The name of the file containing the template.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartApplyTemplate(long chartId, string filename)
        {
            var commandParameters = new ArrayList { chartId, filename };
            return SendCommand<bool>(MT4CommandType.ChartApplyTemplate, commandParameters);
        }

        ///<summary>
        ///Saves current chart settings in a template with a specified name.
        ///</summary>
        ///<param name="chartId">Chart ID.</param>
        ///<param name="filename">The filename to save the template. The ".tpl" extension will be added to the filename automatically; there is no need to specify it. The template is saved in data_folder\templates\ and can be used for manual application in the terminal. If a template with the same filename already exists, the contents of this file will be overwritten.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartSaveTemplate(long chartId, string filename)
        {
            var commandParameters = new ArrayList { chartId, filename };
            return SendCommand<bool>(MT4CommandType.ChartSaveTemplate, commandParameters);
        }

        ///<summary>
        ///The function returns the number of a subwindow where an indicator is drawn.
        ///</summary>
        ///<param name="chartId">Chart ID.</param>
        ///<param name="indicatorShortname">Short name of the indicator.</param>
        ///<returns>
        ///Subwindow number in case of success. In case of failure the function returns -1.
        ///</returns>
        public int ChartWindowFind(long chartId, string indicatorShortname)
        {
            var commandParameters = new ArrayList { chartId, indicatorShortname };
            return SendCommand<int>(MT4CommandType.ChartWindowFind, commandParameters);
        }

        ///<summary>
        ///The function returns the number of a subwindow where an indicator is drawn.
        ///</summary>
        ///<param name="chartId">Chart ID.</param>
        ///<param name="subWindow">The number of the chart subwindow. 0 means the main chart window.</param>
        ///<param name="time">The time value on the chart, for which the value in pixels along the X axis will be received.</param>
        ///<param name="price">The price value on the chart, for which the value in pixels along the Y axis will be received.</param>
        ///<param name="x">The variable, into which the conversion of time to X will be received. The origin is in the upper left corner of the main chart window.</param>
        ///<param name="y">The variable, into which the conversion of price to Y will be received. The origin is in the upper left corner of the main chart window.</param>
        ///<returns>
        ///Subwindow number in case of success. In case of failure the function returns -1.
        ///</returns>
        public bool ChartTimePriceToXY(long chartId, int subWindow, DateTime? time, double price, out int x, out int y)
        {
            var commandParameters = new ArrayList { chartId, subWindow, MT4APITimeConverter.ConvertToMTTime(time), price };
            var str = SendCommand<string>(MT4CommandType.ChartTimePriceToXY, commandParameters);
            var res = false;
            x = 0;
            y = 0;
            if (!string.IsNullOrEmpty(str) && str.Contains(";"))
            {
                var values = str.Split(';');
                if (values.Length > 1)
                {
                    int.TryParse(values[0], out x);
                    int.TryParse(values[1], out y);
                    res = true;
                }
            }
            return res;
        }

        ///<summary>
        ///The function returns the number of a subwindow where an indicator is drawn.
        ///</summary>
        ///<param name="chartId">Chart ID.</param>
        ///<param name="x">The variable, into which the conversion of time to X will be received. The origin is in the upper left corner of the main chart window.</param>
        ///<param name="y">The variable, into which the conversion of price to Y will be received. The origin is in the upper left corner of the main chart window.</param>
        ///<param name="subWindow">The number of the chart subwindow. 0 means the main chart window.</param>
        ///<param name="time">The time value on the chart, for which the value in pixels along the X axis will be received.</param>
        ///<param name="price">The price value on the chart, for which the value in pixels along the Y axis will be received.</param>
        ///<returns>
        ///Subwindow number in case of success. In case of failure the function returns -1.
        ///</returns>
        public bool ChartXYToTimePrice(long chartId, int x, int y, out int subWindow, out DateTime? time, out double price)
        {
            var commandParameters = new ArrayList { chartId, x, y };
            var str = SendCommand<string>(MT4CommandType.ChartXYToTimePrice, commandParameters);
            var res = false;
            subWindow = 0;
            time = null;
            price = double.NaN;
            if (!string.IsNullOrEmpty(str) && str.Contains(";"))
            {
                var values = str.Split(';');
                if (values.Length > 2)
                {
                    int.TryParse(values[0], out subWindow);
                    int.TryParse(values[1], out int mt4Time);
                    time = MT4APITimeConverter.ConvertFromMTTime(mt4Time);
                    double.TryParse(values[2], out price);
                    res = true;
                }
            }
            return res;
        }

        ///<summary>
        ///Opens a new chart with the specified symbol and period.
        ///</summary>
        ///<param name="symbol">Chart symbol. NULL means the symbol of the  current chart (the Expert Advisor is attached to).</param>
        ///<param name="period"> Chart period (timeframe). Can be one of the ENUM_TIMEFRAMES values. 0 means the current chart period.</param>
        ///<returns>
        ///If successful, it returns the opened chart ID. Otherwise returns 0.
        ///</returns>
        public long ChartOpen(string symbol, EnumTimeframes period)
        {
            var commandParameters = new ArrayList { symbol, (int)period };
            return SendCommand<long>(MT4CommandType.ChartOpen, commandParameters);
        }

        ///<summary>
        ///Returns the ID of the first chart of the client terminal.
        ///</summary>
        public long ChartFirst()
        {
            return SendCommand<long>(MT4CommandType.ChartFirst, null);
        }

        ///<summary>
        ///Returns the chart ID of the chart next to the specified one.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 does not mean the current chart. 0 means "return the first chart ID".</param>
        ///<returns>
        ///Chart ID. If this is the end of the chart list, it returns -1.
        ///</returns>
        public long ChartNext(long chartId)
        {
            var commandParameters = new ArrayList { chartId };
            return SendCommand<long>(MT4CommandType.ChartNext, commandParameters);
        }

        ///<summary>
        ///Closes the specified chart.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<returns>
        ///If successful, returns true, otherwise false.
        ///</returns>
        public bool ChartClose(long chartId)
        {
            var commandParameters = new ArrayList { chartId };
            return SendCommand<bool>(MT4CommandType.ChartClose, commandParameters);
        }

        ///<summary>
        ///Returns the symbol name for the specified chart.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<returns>
        ///If chart does not exist, the result will be an empty string.
        ///</returns>
        public string ChartSymbol(long chartId)
        {
            var commandParameters = new ArrayList { chartId };
            return SendCommand<string>(MT4CommandType.ChartSymbol, commandParameters);
        }

        ///<summary>
        ///Returns the timeframe period of specified chart.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<returns>
        ///The function returns one of the ENUM_TIMEFRAMES values. If chart does not exist, it returns 0.
        ///</returns>
        public EnumTimeframes ChartPeriod(long chartId)
        {
            var commandParameters = new ArrayList { chartId };
            return (EnumTimeframes) SendCommand<int>(MT4CommandType.ChartPeriod, commandParameters);
        }

        ///<summary>
        ///Sets a value for a corresponding property of the specified chart. Chart property should be of a double type.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. Can be one of the ENUM_CHART_PROPERTY_DOUBLE values (except the read-only properties).</param>
        ///<param name="value">Property value.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartSetDouble(long chartId, EnumChartPropertyDouble propId, double value)
        {
            var commandParameters = new ArrayList { chartId, (int)propId, value };
            return SendCommand<bool>(MT4CommandType.ChartSetDouble, commandParameters);
        }

        ///<summary>
        ///Sets a value for a corresponding property of the specified chart. Chart property must be datetime, int, color, bool or char.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. It can be one of the ENUM_CHART_PROPERTY_INTEGER value (except the read-only properties).</param>
        ///<param name="value">Property value.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartSetInteger(long chartId, EnumChartPropertyInteger propId, long value)
        {
            var commandParameters = new ArrayList { chartId, (int)propId, value };
            return SendCommand<bool>(MT4CommandType.ChartSetInteger, commandParameters);
        }

        ///<summary>
        ///Sets a value for a corresponding property of the specified chart. Chart property must be of the string type.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. Its value can be one of the ENUM_CHART_PROPERTY_STRING values (except the read-only properties).</param>
        ///<param name="value">Property value string. String length cannot exceed 2045 characters (extra characters will be truncated).</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartSetString(long chartId, EnumChartPropertyString propId, string value)
        {
            var commandParameters = new ArrayList { chartId, (int)propId, value };
            return SendCommand<bool>(MT4CommandType.ChartSetString, commandParameters);
        }

        ///<summary>
        ///Sets a value for a corresponding property of the specified chart. Chart property must be of the string type.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. This value can be one of the ENUM_CHART_PROPERTY_DOUBLE values.</param>
        ///<param name="subWindow">Number of the chart subwindow. For the first case, the default value is 0 (main chart window). The most of the properties do not require a subwindow number.</param>
        ///<returns>
        ///The value of double type.
        ///</returns>
        public double ChartGetDouble(long chartId, EnumChartPropertyDouble propId, int subWindow = 0)
        {
            var commandParameters = new ArrayList { chartId, (int)propId, subWindow };
            return SendCommand<double>(MT4CommandType.ChartGetDouble, commandParameters);
        }

        ///<summary>
        ///Returns the value of a corresponding property of the specified chart. Chart property must be of datetime, int or bool type.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. This value can be one of the ENUM_CHART_PROPERTY_INTEGER values.</param>
        ///<param name="subWindow">Number of the chart subwindow. For the first case, the default value is 0 (main chart window). The most of the properties do not require a subwindow number.</param>
        ///<returns>
        ///The value of long type.
        ///</returns>
        public long ChartGetInteger(long chartId, EnumChartPropertyInteger propId, int subWindow = 0)
        {
            var commandParameters = new ArrayList { chartId, (int)propId, subWindow };
            return SendCommand<long>(MT4CommandType.ChartGetInteger, commandParameters);
        }

        ///<summary>
        ///Returns the value of a corresponding property of the specified chart. Chart property must be of string type.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="propId">Chart property ID. This value can be one of the ENUM_CHART_PROPERTY_STRING values.</param>
        ///<returns>
        ///The value of string type.
        ///</returns>
        public string ChartGetString(long chartId, EnumChartPropertyString propId)
        {
            var commandParameters = new ArrayList { chartId, (int)propId };
            return SendCommand<string>(MT4CommandType.ChartGetString, commandParameters);
        }

        ///<summary>
        ///Performs shift of the specified chart by the specified number of bars relative to the specified position in the chart.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="position">Chart position to perform a shift. Can be one of the ENUM_CHART_POSITION values.</param>
        ///<param name="shift">Number of bars to shift the chart. Positive value means the right shift (to the end of chart), negative value means the left shift (to the beginning of chart). The zero shift can be used to navigate to the beginning or end of chart.</param>
        ///<returns>
        ///Returns true if successful, otherwise returns false.
        ///</returns>
        public bool ChartNavigate(long chartId, int position, int shift = 0)
        {
            var commandParameters = new ArrayList { chartId, position, shift };
            return SendCommand<bool>(MT4CommandType.ChartNavigate, commandParameters);
        }

        ///<summary>
        ///Performs shift of the specified chart by the specified number of bars relative to the specified position in the chart.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="subWindow">Number of the chart subwindow. 0 denotes the main chart subwindow.</param>
        ///<param name="indicatorShortname">The short name of the indicator which is set in the INDICATOR_SHORTNAME property with the IndicatorSetString() function. To get the short name of an indicator use the ChartIndicatorName() function.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartIndicatorDelete(long chartId, int subWindow, string indicatorShortname)
        {
            var commandParameters = new ArrayList { chartId, subWindow, indicatorShortname };
            return SendCommand<bool>(MT4CommandType.ChartIndicatorDelete, commandParameters);
        }

        ///<summary>
        ///Returns the short name of the indicator by the number in the indicators list on the specified chart window.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="subWindow">Number of the chart subwindow. 0 denotes the main chart subwindow.</param>
        ///<param name="index">the index of the indicator in the list of indicators. The numeration of indicators start with zero, i.e. the first indicator in the list has the 0 index. To obtain the number of indicators in the list use the ChartIndicatorsTotal() function.</param>
        ///<returns>
        ///The short name of the indicator which is set in the INDICATOR_SHORTNAME property with the IndicatorSetString() function.
        ///</returns>
        public string ChartIndicatorName(long chartId, int subWindow, int index)
        {
            var commandParameters = new ArrayList { chartId, subWindow, index };
            return SendCommand<string>(MT4CommandType.ChartIndicatorName, commandParameters);
        }

        ///<summary>
        ///Returns the number of all indicators applied to the specified chart window.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="subWindow">Number of the chart subwindow. 0 denotes the main chart subwindow.</param>
        ///<returns>
        ///The number of indicators in the specified chart window.
        ///</returns>
        public int ChartIndicatorsTotal(long chartId, int subWindow)
        {
            var commandParameters = new ArrayList { chartId, subWindow };
            return SendCommand<int>(MT4CommandType.ChartIndicatorsTotal, commandParameters);
        }

        ///<summary>
        ///Returns the number (index) of the chart subwindow the Expert Advisor or script has been dropped to. 0 means the main chart window.
        ///</summary>
        public int ChartWindowOnDropped()
        {
            return SendCommand<int>(MT4CommandType.ChartWindowOnDropped, null);
        }

        ///<summary>
        ///Returns the price coordinate corresponding to the chart point the Expert Advisor or script has been dropped to.
        ///</summary>
        public double ChartPriceOnDropped()
        {
            return SendCommand<double>(MT4CommandType.ChartPriceOnDropped, null);
        }

        ///<summary>
        ///Returns the time coordinate corresponding to the chart point the Expert Advisor or script has been dropped to.
        ///</summary>
        public DateTime ChartTimeOnDropped()
        {
            var res = SendCommand<int>(MT4CommandType.ChartTimeOnDropped, null);
            return MT4APITimeConverter.ConvertFromMTTime(res);
        }

        ///<summary>
        ///Returns the X coordinate of the chart point the Expert Advisor or script has been dropped to.
        ///</summary>
        public int ChartXOnDropped()
        {
            return SendCommand<int>(MT4CommandType.ChartXOnDropped, null);
        }

        ///<summary>
        ///Returns the Y coordinateof the chart point the Expert Advisor or script has been dropped to.
        ///</summary>
        public int ChartYOnDropped()
        {
            return SendCommand<int>(MT4CommandType.ChartYOnDropped, null);
        }

        ///<summary>
        ///Changes the symbol and period of the specified chart. The function is asynchronous, i.e. it sends the command and does not wait for its execution completion.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="symbol">Chart symbol. NULL value means the current chart symbol (Expert Advisor is attached to)</param>
        ///<param name="period">Chart period (timeframe). Can be one of the ENUM_TIMEFRAMES values. 0 means the current chart period.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartSetSymbolPeriod(long chartId, string symbol, EnumTimeframes period)
        {
            var commandParameters = new ArrayList { chartId, symbol, (int)period };
            return SendCommand<bool>(MT4CommandType.ChartSetSymbolPeriod, commandParameters);
        }

        ///<summary>
        ///Saves current chart screen shot as a GIF, PNG or BMP file depending on specified extension.
        ///</summary>
        ///<param name="chartId">Chart ID. 0 means the current chart.</param>
        ///<param name="filename">Screenshot file name. Cannot exceed 63 characters. Screenshot files are placed in the \Files directory.</param>
        ///<param name="width">Screenshot width in pixels.</param>
        ///<param name="height">Screenshot height in pixels.</param>
        ///<param name="alignMode">Output mode of a narrow screenshot.</param>
        ///<returns>
        ///Returns true if the command has been added to chart queue, otherwise false.
        ///</returns>
        public bool ChartScreenShot(long chartId, string filename, int width, int height, EnumAlignMode alignMode = EnumAlignMode.ALIGN_RIGHT)
        {
            var commandParameters = new ArrayList { chartId, filename, width, height, (int)alignMode };
            return SendCommand<bool>(MT4CommandType.ChartScreenShot, commandParameters);
        }

        ///<summary>
        ///Returns the amount of bars visible on the chart.
        ///</summary>
        ///<returns>
        ///The amount of bars visible on the chart.
        ///</returns>
        public int WindowBarsPerChart()
        {
            return SendCommand<int>(MT4CommandType.WindowBarsPerChart, null);
        }

        ///<summary>
        ///Returns the name of the executed Expert Advisor, script, custom indicator, or library.
        ///</summary>
        ///<returns>
        ///The name of the executed Expert Advisor, script, custom indicator, or library, depending on the MQL4 program, from which this function has been called.
        ///</returns>
        public string WindowExpertName()
        {
            return SendCommand<string>(MT4CommandType.WindowExpertName, null);
        }

        ///<summary>
        ///Returns the window index containing this specified indicator.
        ///</summary>
        ///<param name="name">Indicator short name.</param>
        ///<returns>
        ///If indicator with name was found, the function returns the window index containing this specified indicator, otherwise it returns -1.
        ///</returns>
        public int WindowFind(string name)
        {
            var commandParameters = new ArrayList { name };
            return SendCommand<int>(MT4CommandType.WindowFind, commandParameters);
        }

        ///<summary>
        ///Returns index of the first visible bar in the current chart window.
        ///</summary>
        ///<returns>
        ///Index of the first visible bar number in the current chart window.
        ///</returns>
        public int WindowFirstVisibleBar()
        {
            return SendCommand<int>(MT4CommandType.WindowFirstVisibleBar, null);
        }

        ///<summary>
        ///Returns the system handle of the chart window.
        ///</summary>
        ///<param name="symbol">Symbol.</param>
        ///<param name="timeframe">Timeframe. It can be any of Timeframe enumeration values. 0 means the current chart timeframe.</param>
        ///<returns>
        ///Returns the system handle of the chart window. If the chart of symbol and timeframe has not been opened by the moment of function calling, 0 will be returned.
        ///</returns>
        public int WindowHandle(string symbol, int timeframe)
        {
            var commandParameters = new ArrayList { symbol, timeframe };
            return SendCommand<int>(MT4CommandType.WindowHandle, commandParameters);
        }

        ///<summary>
        ///Returns the visibility flag of the chart subwindow.
        ///</summary>
        ///<param name="index">Subwindow index.</param>
        ///<returns>
        ///Returns true if the chart subwindow is visible, otherwise returns false. The chart subwindow can be hidden due to the visibility properties of the indicator placed in it.
        ///</returns>
        public bool WindowIsVisible(int index)
        {
            var commandParameters = new ArrayList { index };
            return SendCommand<bool>(MT4CommandType.WindowIsVisible, commandParameters);
        }

        ///<summary>
        ///Returns the window index where Expert Advisor, custom indicator or script was dropped.
        ///</summary>
        ///<returns>
        ///The window index where Expert Advisor, custom indicator or script was dropped. This value is valid if the Expert Advisor, custom indicator or script was dropped by mouse.
        ///</returns>
        public int WindowOnDropped()
        {
            return SendCommand<int>(MT4CommandType.WindowOnDropped, null);
        }

        ///<summary>
        ///Returns the maximal value of the vertical scale of the specified subwindow of the current chart.
        ///</summary>
        ///<param name="index">Chart subwindow index (0 - main chart window).</param>
        ///<returns>
        ///The maximal value of the vertical scale of the specified subwindow of the current chart.
        ///</returns>
        public int WindowPriceMax(int index = 0)
        {
            var commandParameters = new ArrayList { index };
            return SendCommand<int>(MT4CommandType.WindowPriceMax, commandParameters);
        }

        ///<summary>
        ///Returns the minimal value of the vertical scale of the specified subwindow of the current chart.
        ///</summary>
        ///<param name="index">Chart subwindow index (0 - main chart window).</param>
        ///<returns>
        ///The minimal value of the vertical scale of the specified subwindow of the current chart.
        ///</returns>
        public int WindowPriceMin(int index = 0)
        {
            var commandParameters = new ArrayList { index };
            return SendCommand<int>(MT4CommandType.WindowPriceMin, commandParameters);
        }

        ///<summary>
        ///Returns the price of the chart point where Expert Advisor or script was dropped.
        ///</summary>
        ///<returns>
        ///The price of the chart point where Expert Advisor or script was dropped. This value is only valid if the expert or script was dropped by mouse.
        ///</returns>
        public double WindowPriceOnDropped()
        {
            return SendCommand<double>(MT4CommandType.WindowPriceOnDropped, null);
        }

        ///<summary>
        ///Redraws the current chart forcedly.
        ///</summary>
        ///<returns>
        ///Redraws the current chart forcedly. It is normally used after the objects properties have been changed.
        ///</returns>
        public void WindowRedraw()
        {
            SendCommand<object>(MT4CommandType.WindowRedraw, null);
        }

        ///<summary>
        ///Saves current chart screen shot as a GIF file.
        ///</summary>
        ///<param name="filename">Screen shot file name. Screenshot is saved to \Files folder.</param>
        ///<param name="sizeX">Screen shot width in pixels.</param>
        ///<param name="sizeY">Screen shot height in pixels.</param>
        ///<param name="startBar">Index of the first visible bar in the screen shot. If 0 value is set, the current first visible bar will be shot. If no value or negative value has been set, the end-of-chart screen shot will be produced, indent being taken into consideration.</param>
        ///<param name="chartScale">Horizontal chart scale for screen shot. Can be in the range from 0 to 5. If no value or negative value has been set, the current chart scale will be used.</param>
        ///<param name="chartMode"> Chart displaying mode. It can take the following values: CHART_BAR (0 is a sequence of bars), CHART_CANDLE (1 is a sequence of candlesticks), CHART_LINE (2 is a close prices line). If no value or negative value has been set, the chart will be shown in its current mode.</param>
        ///<returns>
        ///Returns true if succeed, otherwise false.
        ///</returns>
        public bool WindowScreenShot(string filename, int sizeX, int sizeY, int startBar = -1, int chartScale = -1, int chartMode = -1)
        {
            var commandParameters = new ArrayList { filename, sizeX, sizeY, startBar, chartScale, chartMode };
            return SendCommand<bool>(MT4CommandType.WindowScreenShot, commandParameters);
        }

        ///<summary>
        ///Returns the time of the chart point where Expert Advisor or script was dropped.
        ///</summary>
        ///<returns>
        ///The time value of the chart point where expert or script was dropped. This value is only valid if the expert or script was dropped by mouse.
        ///</returns>
        public DateTime WindowTimeOnDropped()
        {
            var res = SendCommand<int>(MT4CommandType.WindowTimeOnDropped, null);
            return MT4APITimeConverter.ConvertFromMTTime(res);
        }

        ///<summary>
        ///Returns total number of indicator windows on the chart.
        ///</summary>
        ///<returns>
        ///Total number of indicator windows on the chart (including main chart).
        ///</returns>
        public int WindowsTotal()
        {
            return SendCommand<int>(MT4CommandType.WindowsTotal, null);
        }

        ///<summary>
        ///Returns the value at X axis in pixels for the chart window client area point at which the Expert Advisor or script was dropped.
        ///</summary>
        ///<returns>
        ///The value at X axis in pixels for the chart window client area point at which the expert or script was dropped. The value will be true only if the expert or script were moved with the mouse ("Drag'n'Drop") technique.
        ///</returns>
        public int WindowXOnDropped()
        {
            return SendCommand<int>(MT4CommandType.WindowXOnDropped, null);
        }

        ///<summary>
        ///Returns the value at Y axis in pixels for the chart window client area point at which the Expert Advisor or script was dropped.
        ///</summary>
        ///<returns>
        ///Returns the value at Y axis in pixels for the chart window client area point at which the Expert Advisor or script was dropped. The value will be true only if the expert or script were moved with the mouse ("Drag'n'Drop") technique.
        ///</returns>
        public int WindowYOnDropped()
        {
            return SendCommand<int>(MT4CommandType.WindowYOnDropped, null);
        }
        #endregion

        #region Object Functions

        ///<summary>
        ///The function creates an object with the specified name, type, and the initial coordinates in the specified chart subwindow of the specified chart.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object. The name must be unique within a chart, including its subwindows.</param>
        ///<param name="objectType">Object type.</param>
        ///<param name="subWindow">Number of the chart subwindow. 0 means the main chart window.</param>
        ///<param name="time1">The time coordinate of the first anchor point.</param>
        ///<param name="price1">The price coordinate of the first anchor point.</param>
        ///<param name="time2">The time coordinate of the second anchor point.</param>
        ///<param name="price2">The price coordinate of the second anchor point.</param>
        ///<param name="time3">The time coordinate of the third anchor point.</param>
        ///<param name="price3">The price coordinate of the third anchor point.</param>
        ///<returns>
        ///Returns true or false depending on whether the object is created or not. 
        ///</returns>
        public bool ObjectCreate(long chartId, string objectName, EnumObject objectType, int subWindow, 
            DateTime? time1, double price1, DateTime? time2 = null, double? price2 = null, DateTime? time3 = null, double? price3 = null)
        {
            var namedParams = new Dictionary<string, object>
            {
                {nameof(chartId), chartId},
                {nameof(objectName), objectName},
                {nameof(objectType), (int) objectType},
                {nameof(subWindow), subWindow},
                {nameof(time1), MT4APITimeConverter.ConvertToMTTime(time1)},
                {nameof(price1), price1}
            };

            if (time2 != null)
                namedParams.Add(nameof(time2), MT4APITimeConverter.ConvertToMTTime(time2.Value));
            if (price2 != null)
                namedParams.Add(nameof(price2), price2.Value);
            if (time3 != null)
                namedParams.Add(nameof(time3), MT4APITimeConverter.ConvertToMTTime(time3.Value));
            if (price3 != null)
                namedParams.Add(nameof(price3), price3.Value);

            return SendCommand<bool>(MT4CommandType.ObjectCreate, null, namedParams);
        }

        ///<summary>
        ///The function returns the name of the corresponding object by its index in the objects list.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectIndex">Object index. This value must be greater or equal to 0 and less than ObjectsTotal().</param>
        ///<param name="subWindow">Number of the chart window. Must be greater or equal to -1 (-1 mean all subwindows, 0 means the main chart window) and less than WindowsTotal().</param>
        ///<param name="objectType">Type of the object. The value can be one of the values of the EnumObject enumeration. EMPTY (-1) means all types.</param>
        ///<returns>
        ///Name of the object is returned in case of success.
        ///</returns>
        public string ObjectName(long chartId, int objectIndex, int subWindow = EMPTY, int objectType = EMPTY)
        {
            var commandParameters = new ArrayList { chartId, objectIndex, subWindow, objectType };
            return SendCommand<string>(MT4CommandType.ObjectName, commandParameters);
        }

        ///<summary>
        ///The function removes the object with the specified name at the specified chart.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of object to be deleted.</param>
        ///<returns>
        ///Returns true if the removal was successful, otherwise returns false.
        ///</returns>
        public bool ObjectDelete(long chartId, string objectName)
        {
            var commandParameters = new ArrayList { chartId, objectName };
            return SendCommand<bool>(MT4CommandType.ObjectDelete, commandParameters);
        }

        ///<summary>
        ///The function removes the object with the specified name at the specified chart.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="subWindow">Number of the chart window. Must be greater or equal to -1 (-1 mean all subwindows, 0 means the main chart window) and less than WindowsTotal().</param>
        ///<param name="objectType">Type of the object. The value can be one of the values of the EnumObject enumeration. EMPTY (-1) means all types.</param>
        ///<returns>
        ///Returns true if the removal was successful, otherwise returns false.
        ///</returns>
        public int ObjectsDeleteAll(long chartId, int subWindow = EMPTY, int objectType = EMPTY)
        {
            var commandParameters = new ArrayList { chartId, subWindow, objectType };
            return SendCommand<int>(MT4CommandType.ObjectsDeleteAll, commandParameters);
        }

        ///<summary>
        ///The function searches for an object having the specified name.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">The name of the object to find.</param>
        ///<returns>
        ///If successful the function returns the number of the subwindow (0 means the main window of the chart), in which the object is found. 
        ///</returns>
        public int ObjectFind(long chartId, string objectName)
        {
            var commandParameters = new ArrayList { chartId, objectName };
            return SendCommand<int>(MT4CommandType.ObjectFind, commandParameters);
        }

        ///<summary>
        ///The function returns the time value for the specified price value of the specified object.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="value">Price value.</param>
        ///<param name="lineId">Line identifier.</param>
        ///<returns>
        ///The time value for the specified price value of the specified object.
        ///</returns>
        public DateTime ObjectGetTimeByValue(long chartId, string objectName, double value, int lineId = 0)
        {
            var commandParameters = new ArrayList { chartId, objectName, value, lineId };
            var res = SendCommand<int>(MT4CommandType.ObjectGetTimeByValue, commandParameters);
            return MT4APITimeConverter.ConvertFromMTTime(res);
        }

        ///<summary>
        ///The function returns the price value for the specified time value of the specified object.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="time">Time value.</param>
        ///<param name="lineId">Line identifier.</param>
        ///<returns>
        ///The price value for the specified time value of the specified object.
        ///</returns>
        public double ObjectGetValueByTime(long chartId, string objectName, DateTime? time, int lineId = 0)
        {
            var commandParameters = new ArrayList { chartId, objectName, time, lineId };
            return SendCommand<double>(MT4CommandType.ObjectGetValueByTime, commandParameters);
        }

        ///<summary>
        ///The function changes coordinates of the specified anchor point of the object at the specified chart
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="pointIndex">Index of the anchor point.</param>
        ///<param name="time">Time coordinate of the selected anchor point.</param>
        ///<param name="price">Price coordinate of the selected anchor point.</param>
        ///<returns>
        ///If successful, returns true, in case of failure returns false.
        ///</returns>
        public bool ObjectMove(long chartId, string objectName, int pointIndex, DateTime? time, double price)
        {
            var commandParameters = new ArrayList { chartId, objectName, pointIndex, MT4APITimeConverter.ConvertToMTTime(time), price };
            return SendCommand<bool>(MT4CommandType.ObjectMove, commandParameters);
        }

        ///<summary>
        ///The function changes coordinates of the specified anchor point of the object at the specified chart.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="subWindow">Number of the chart subwindow. 0 means the main chart window, -1 means all the subwindows of the chart, including the main window.</param>
        ///<param name="type">Type of the object. The value can be one of the values of the EnumObject enumeration. EMPTY(-1) means all types.</param>
        ///<returns>
        ///The number of objects.
        ///</returns>
        public int ObjectsTotal(long chartId, int subWindow = EMPTY, int type = EMPTY)
        {
            var commandParameters = new ArrayList { chartId, subWindow, type };
            return SendCommand<int>(MT4CommandType.ObjectsTotal, commandParameters);
        }

        ///<summary>
        ///The function returns the value of the corresponding object property.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyDouble enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. For the first variant, the default modifier value is equal to 0. Most properties do not require a modifier.</param>
        ///<returns>
        ///Value of the double type.
        ///</returns>
        public double ObjectGetDouble(long chartId, string objectName, EnumObjectPropertyDouble propId, int propModifier = 0)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propModifier };
            return SendCommand<double>(MT4CommandType.ObjectGetDouble, commandParameters);
        }

        ///<summary>
        ///The function returns the value of the corresponding object property. The object property must be of the datetime, int, color, bool or char type.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyDouble enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. For the first variant, the default modifier value is equal to 0. Most properties do not require a modifier.</param>
        ///<returns>
        ///The long value.
        ///</returns>
        public long ObjectGetInteger(long chartId, string objectName, EnumObjectPropertyInteger propId, int propModifier = 0)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propModifier };
            return SendCommand<long>(MT4CommandType.ObjectGetInteger, commandParameters);
        }

        ///<summary>
        ///The function returns the value of the corresponding object property. The object property must be of the string type.
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyString enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. For the first variant, the default modifier value is equal to 0. Most properties do not require a modifier.  It denotes the number of the level in Fibonacci tools and in the graphical object Andrew's pitchfork. The numeration of levels starts from zero.</param>
        ///<returns>
        ///String value.
        ///</returns>
        public string ObjectGetString(long chartId, string objectName, EnumObjectPropertyString propId, int propModifier = 0)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propModifier };
            return SendCommand<string>(MT4CommandType.ObjectGetString, commandParameters);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the double type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyDouble enumeration.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully.
        ///</returns>
        public bool ObjectSetDouble(long chartId, string objectName, EnumObjectPropertyDouble propId, double propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            return SendCommand<bool>(MT4CommandType.ObjectSetDouble, commandParameters);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the double type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyDouble enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. It denotes the number of the level in Fibonacci tools and in the graphical object Andrew's pitchfork. The numeration of levels starts from zero.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully.
        ///</returns>
        public bool ObjectSetDouble(long chartId, string objectName, EnumObjectPropertyDouble propId, int propModifier, double propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            var namedParams = new Dictionary<string, object> {{nameof(propModifier), propModifier}};
            return SendCommand<bool>(MT4CommandType.ObjectSetDouble, commandParameters, namedParams);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the int type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyInteger enumeration.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully. Otherwise it returns false. 
        ///</returns>
        public bool ObjectSetInteger(long chartId, string objectName, EnumObjectPropertyInteger propId, long propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            return SendCommand<bool>(MT4CommandType.ObjectSetInteger, commandParameters);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the int type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyInteger enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. It denotes the number of the level in Fibonacci tools and in the graphical object Andrew's pitchfork. The numeration of levels starts from zero.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully. Otherwise it returns false. 
        ///</returns>
        public bool ObjectSetInteger(long chartId, string objectName, EnumObjectPropertyInteger propId, int propModifier, long propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            var namedParams = new Dictionary<string, object> {{nameof(propModifier), propModifier}};
            return SendCommand<bool>(MT4CommandType.ObjectSetInteger, commandParameters, namedParams);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the string type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyString enumeration.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully. Otherwise it returns false. 
        ///</returns>
        public bool ObjectSetString(long chartId, string objectName, EnumObjectPropertyString propId, string propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            return SendCommand<bool>(MT4CommandType.ObjectSetString, commandParameters);
        }

        ///<summary>
        ///The function sets the value of the corresponding object property. The object property must be of the string type. 
        ///</summary>
        ///<param name="chartId">Chart identifier. 0 means the current chart.</param>
        ///<param name="objectName">Name of the object.</param>
        ///<param name="propId">ID of the object property. The value can be one of the values of the EnumObjectPropertyString enumeration.</param>
        ///<param name="propModifier">Modifier of the specified property. It denotes the number of the level in Fibonacci tools and in the graphical object Andrew's pitchfork. The numeration of levels starts from zero.</param>
        ///<param name="propValue">The value of the property.</param>
        ///<returns>
        ///The function returns true only if the command to change properties of a graphical object has been sent to a chart successfully. Otherwise it returns false. 
        ///</returns>
        public bool ObjectSetString(long chartId, string objectName, EnumObjectPropertyString propId, int propModifier, string propValue)
        {
            var commandParameters = new ArrayList { chartId, objectName, (int)propId, propValue };
            var namedParams = new Dictionary<string, object> {{nameof(propModifier), propModifier}};
            return SendCommand<bool>(MT4CommandType.ObjectSetString, commandParameters, namedParams);
        }

        ///<summary>
        ///The function sets the font for displaying the text using drawing methods and returns the result of that operation. Arial font with the size -120 (12 pt) is used by default.
        ///</summary>
        ///<param name="name">Font name in the system or the name of the resource containing the font or the path to font file on the disk.</param>
        ///<param name="size">The font size that can be set using positive and negative values. In case of positive values, the size of a displayed text does not depend on the operating system's font size settings. In case of negative values, the value is set in tenths of a point and the text size depends on the operating system settings ("standard scale" or "large scale"). See the Note below for more information about the differences between the modes.</param>
        ///<param name="flags">Combination of flags describing font style.</param>
        ///<param name="orientation">Text's horizontal inclination to X axis, the unit of measurement is 0.1 degrees. It means that orientation=450 stands for inclination equal to 45 degrees.</param>
        ///<returns>
        ///Returns true if the current font is successfully installed, otherwise false.
        ///</returns>
        public bool TextSetFont(string name, int size, FlagFontStyle flags = 0, int orientation = 0)
        {
            var commandParameters = new ArrayList { name, size, (uint)flags, orientation };
            return SendCommand<bool>(MT4CommandType.TextSetFont, commandParameters);
        }

        ///<summary>
        ///Returns the object description.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<returns>
        ///Object description. For objects of OBJ_TEXT and OBJ_LABEL types, the text drawn by these objects will be returned.
        ///</returns>
        public string ObjectDescription(string objectName)
        {
            var commandParameters = new ArrayList { objectName };
            return SendCommand<string>(MT4CommandType.ObjectDescription, commandParameters);
        }

        ///<summary>
        ///Returns the level description of a Fibonacci object.
        ///</summary>
        ///<param name="objectName">Fibonacci object name.</param>
        ///<param name="index">Index of the Fibonacci level (0-31).</param>
        ///<returns>
        ///The level description of a Fibonacci object.
        ///</returns>
        public string ObjectGetFiboDescription(string objectName, int index)
        {
            var commandParameters = new ArrayList { objectName, index };
            return SendCommand<string>(MT4CommandType.ObjectGetFiboDescription, commandParameters);
        }

        ///<summary>
        ///The function calculates and returns bar index (shift related to the current bar) for the given price.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<param name="value">Price value.</param>
        ///<returns>
        ///The function calculates and returns bar index (shift related to the current bar) for the given price. The bar index is calculated by the first and second coordinates using a linear equation. Applied to trendlines and similar objects.
        ///</returns>
        public int ObjectGetShiftByValue(string objectName, double value)
        {
            var commandParameters = new ArrayList { objectName, value };
            return SendCommand<int>(MT4CommandType.ObjectGetShiftByValue, commandParameters);
        }

        ///<summary>
        ///The function calculates and returns the price value for the specified bar (shift related to the current bar).
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<param name="shift">Bar index.</param>
        ///<returns>
        ///The function calculates and returns the price value for the specified bar (shift related to the current bar). The price value is calculated by the first and second coordinates using a linear equation. Applied to trendlines and similar objects.
        ///</returns>
        public double ObjectGetValueByShift(string objectName, int shift)
        {
            var commandParameters = new ArrayList { objectName, shift };
            return SendCommand<double>(MT4CommandType.ObjectGetValueByShift, commandParameters);
        }

        ///<summary>
        ///Changes the value of the specified object property.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<param name="index">Object property index. It can be any of object properties enumeration values.</param>
        ///<param name="value">New value of the given property.</param>
        ///<returns>
        ///If the function succeeds, the returned value will be true, otherwise it returns false.
        ///</returns>
        public bool ObjectSet(string objectName, int index, double value)
        {
            var commandParameters = new ArrayList { objectName, index, value };
            return SendCommand<bool>(MT4CommandType.ObjectSet, commandParameters);
        }

        ///<summary>
        ///The function sets a new description to a level of a Fibonacci object.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<param name="index">Index of the Fibonacci level (0-31).</param>
        ///<param name="text">New description of the level.</param>
        ///<returns>
        ///The function returns true if successful, otherwise false.
        ///</returns>
        public bool ObjectSetFiboDescription(string objectName, int index, string text)
        {
            var commandParameters = new ArrayList { objectName, index, text };
            return SendCommand<bool>(MT4CommandType.ObjectSetFiboDescription, commandParameters);
        }

        ///<summary>
        ///The function changes the object description.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<param name="text">A text describing the object.</param>
        ///<param name="fontSize">Font size in points.</param>
        ///<param name="fontName">Font name.</param>
        ///<param name="textColor">Font color.</param>
        ///<returns>
        ///Changes the object description.  If the function succeeds, the returned value will be true, otherwise false.
        ///</returns>
        public bool ObjectSetText(string objectName, string text, int fontSize = 0, string fontName = null, Color? textColor = null)
        {
            var commandParameters = new ArrayList { objectName, text, fontSize, fontName, MT4APIColorConverter.ConvertToMT4Color(textColor) };
            return SendCommand<bool>(MT4CommandType.ObjectSetText, commandParameters);
        }

        ///<summary>
        ///The function returns the object type value.
        ///</summary>
        ///<param name="objectName">Object name.</param>
        ///<returns>
        ///The function returns the object type value. 
        ///</returns>
        public EnumObject ObjectType(string objectName)
        {
            var commandParameters = new ArrayList { objectName };
            return (EnumObject)SendCommand<int>(MT4CommandType.ObjectType, commandParameters);
        }

        #endregion

        #region Backtesting functions

        public void UnlockTicks()
        {
            SendCommand<object>(MT4CommandType.UnlockTicks, null);
        }

        #endregion

        #region Private Methods
        private MTClient Client
        {
            get
            {
                lock(_locker)
                {
                    return _client;
                }
            }
        }

        private void Connect(MTClient client)
        {
            lock (_locker)
            {
                if (_connectionState == MT4ConnectionState.Connected
                    || _connectionState == MT4ConnectionState.Connecting)
                {
                    return;
                }

                _connectionState = MT4ConnectionState.Connecting;
            }

            string message = string.IsNullOrEmpty(client.Host) ? $"Connecting to localhost:{client.Port}" : $"Connecting to {client.Host}:{client.Port}";
            ConnectionStateChanged?.Invoke(this, new MT4ConnectionEventArgs(MT4ConnectionState.Connecting, message));

            var state = MT4ConnectionState.Failed;

            lock (_locker)
            {
                try
                {
                    client.Connect();
                    state = MT4ConnectionState.Connected;
                }
                catch (Exception e)
                {
                    client.Dispose();
                    message = string.IsNullOrEmpty(client.Host) ? $"Failed connection to localhost:{client.Port}. {e.Message}" : $"Failed connection to {client.Host}:{client.Port}. {e.Message}";

                    Log.Warn(message);
                }

                if (state == MT4ConnectionState.Connected)
                {
                    _client = client;
                    _client.QuoteAdded += ClientQuoteAdded;
                    _client.QuoteRemoved += ClientQuoteRemoved;
                    _client.QuoteUpdated += ClientQuoteUpdated;
                    _client.ServerDisconnected += ClientServerDisconnected;
                    _client.ServerFailed += ClientServerFailed;
                    _client.MtEventReceived += ClientMTEventReceived;
                    message = string.IsNullOrEmpty(client.Host) ? $"Connected to localhost:{client.Port}" : $"Connected to  { client.Host}:{client.Port}";

                    Log.Info(message);
                }

                _connectionState = state;
            }

            ConnectionStateChanged?.Invoke(this, new MT4ConnectionEventArgs(state, message));

            if (state == MT4ConnectionState.Connected)
            {
                OnConnected();
            }
        }

        private void Connect(string host, int port)
        {
            var client = new MTClient(host, port);
            Connect(client);
        }

        private void Connect(int port)
        {
            var client = new MTClient(port);
            Connect(client);
        }

        private void OnConnected()
        {
            _isBacktestingMode = IsTesting();
            if (_isBacktestingMode)
            {
                BacktestingReady();
            }
        }

        private void Disconnect(bool failed)
        {
            var state =  failed ? MT4ConnectionState.Failed : MT4ConnectionState.Disconnected;
            var message = failed ? "Connection Failed" : "Disconnected";

            lock (_locker)
            {
                if (_connectionState == MT4ConnectionState.Disconnected
                    || _connectionState == MT4ConnectionState.Failed)
                    return;

                if (_client != null)
                {
                    _client.QuoteAdded -= ClientQuoteAdded;
                    _client.QuoteRemoved -= ClientQuoteRemoved;
                    _client.QuoteUpdated -= ClientQuoteUpdated;
                    _client.ServerDisconnected -= ClientServerDisconnected;
                    _client.ServerFailed -= ClientServerFailed;
                    _client.MtEventReceived -= ClientMTEventReceived;

                    if (!failed)
                    {
                        _client.Disconnect();
                    }

                    _client.Dispose();

                    _client = null;
                }

                _connectionState = state;
            }

            Log.Info(message);

            ConnectionStateChanged?.Invoke(this, new MT4ConnectionEventArgs(state, message));
        }

        private T SendCommand<T>(MT4CommandType commandType, ArrayList commandParameters, Dictionary<string, object> namedParams = null)
        {
            MTResponse response;

            var client = Client;
            if (client == null)
            {
                Log.Warn("SendCommand: No connection");
                throw new MT4ConnectionException("No connection");
            }

            try
            {
                response = client.SendCommand((int)commandType, commandParameters, namedParams, ExecutorHandle);
            }
            catch (CommunicationException ex)
            {
                Log.Warn($"SendCommand: {ex.Message}");
                throw new MT4ConnectionException(ex.Message, ex);
            }

            if (response == null)
            {
                Log.Warn("SendCommand: Response from MetaTrader is null");
                throw new MT4ExecutionException(MT4ErrorCode.MT4APICustomError, "Response from MetaTrader is null");
            }

            if (response.ErrorCode != 0)
            {
                Log.Warn($"SendCommand: ErrorCode = {response.ErrorCode}. {response}");
                throw new MT4ExecutionException((MT4ErrorCode)response.ErrorCode, response.ToString());
            }

            var responseValue = response.GetValue();
            return responseValue != null ? (T)responseValue : default;
        }

        private T SendRequest<T>(RequestBase request) where T : ResponseBase, new()
        {
            if (request == null)
                return default;

            var serializer = JsonConvert.SerializeObject(request, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var commandParameters = new ArrayList { serializer };

            var res = SendCommand<string>(MT4CommandType.MtRequest, commandParameters);

            if (res == null)
            {
                Log.Warn("SendRequest: Response from MetaTrader is null");
                throw new MT4ExecutionException(MT4ErrorCode.MT4APICustomError, "Response from MetaTrader is null");
            }

            var response = JsonConvert.DeserializeObject<T>(res);
            if (response.ErrorCode != 0)
            {
                Log.Warn($"SendRequest: ErrorCode = {response.ErrorCode}. {response}");
                throw new MT4ExecutionException((MT4ErrorCode)response.ErrorCode, response.ErrorMessage);
            }

            return response;
        }

        private void ClientQuoteUpdated(MTAPIService.MTQuote quote)
        {
            if (quote != null)
            {
                QuoteUpdate?.Invoke(this, new MT4QuoteEventArgs(new MT4Quote(quote)));
                QuoteUpdated?.Invoke(this, quote.Instrument, quote.Bid, quote.Ask);
            }
        }

        private void ClientServerDisconnected(object sender, EventArgs e)
        {
            Disconnect(false);
        }

        private void ClientServerFailed(object sender, EventArgs e)
        {
            Disconnect(true);
        }

        private void ClientQuoteRemoved(MTAPIService.MTQuote quote)
        {
            QuoteRemoved?.Invoke(this, new MT4QuoteEventArgs(new MT4Quote(quote)));
        }

        private void ClientQuoteAdded(MTAPIService.MTQuote quote)
        {
            QuoteAdded?.Invoke(this, new MT4QuoteEventArgs(new MT4Quote(quote)));
        }

        private void ClientMTEventReceived(MTEvent e)
        {
            var eventType = (MT4EventTypes) e.EventType;

            switch(eventType)
            {
                case MT4EventTypes.LastTimeBar:
                    FireOnLastTimeBar(e.ExpertHandle, JsonConvert.DeserializeObject<MT4TimeBar>(e.Payload));
                    break;
                case MT4EventTypes.ChartEvent:
                    FireOnChartEvent(e.ExpertHandle, JsonConvert.DeserializeObject<MT4ChartEvent>(e.Payload));
                    break;
                case MT4EventTypes.OnLockTicks:
                    FireOnLockTicks(e.ExpertHandle, JsonConvert.DeserializeObject<OnLockTicksEvent>(e.Payload));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FireOnLastTimeBar(int expertHandler, MT4TimeBar timeBar)
        {
            OnLastTimeBar?.Invoke(this, new TimeBarArgs(expertHandler, timeBar));
        }

        private void FireOnChartEvent(int expertHandler, MT4ChartEvent chartEvent)
        {
            OnChartEvent?.Invoke(this, new ChartEventArgs(expertHandler, chartEvent));
        }

        private void FireOnLockTicks(int expertHandler, OnLockTicksEvent lockTicksEvent)
        {
            OnLockTicks?.Invoke(this, new MT4LockTicksEventArgs(expertHandler, lockTicksEvent.Instrument));
        }

        private void BacktestingReady()
        {
            SendCommand<object>(MT4CommandType.BacktestingReady, null);
        }

        #endregion

        #region Events

        public event MTAPIQuoteHandler QuoteUpdated;
        public event EventHandler<MT4QuoteEventArgs> QuoteUpdate;
        public event EventHandler<MT4QuoteEventArgs> QuoteAdded;
        public event EventHandler<MT4QuoteEventArgs> QuoteRemoved;
        public event EventHandler<MT4ConnectionEventArgs> ConnectionStateChanged;
        public event EventHandler<TimeBarArgs> OnLastTimeBar;
        public event EventHandler<ChartEventArgs> OnChartEvent;
        public event EventHandler<MT4LockTicksEventArgs> OnLockTicks;

        #endregion
    }
}
