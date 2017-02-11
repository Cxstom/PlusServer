﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using log4net;
using Plus.Core.ConnectionManager.Socket_Exceptions;

namespace Plus.Core.ConnectionManager
{
    public class SocketManager
    {
        private static readonly ILog log = LogManager.GetLogger("ConnectionManager");

        #region declares

        /// <summary>
        ///     This method is called if an connection event occurs
        /// </summary>
        /// <param name="connection">The new Game connection which was generated by the code</param>
        public delegate void ConnectionEvent(ConnectionInformation connection);

        /// <summary>
        ///     Indicates if connections should be accepted or not
        /// </summary>
        private bool _acceptConnections;

        /// <summary>
        ///     Indicates the amount of accepted connections.
        /// </summary>
        private int _acceptedConnections;

        /// <summary>
        ///     The Socket used for incoming data requests.
        /// </summary>
        private Socket connectionListener;

        private bool disableNagleAlgorithm;

        /// <summary>
        ///     Contains the max conenctions per ip count
        /// </summary>
        private int maxIpConnectionCount;

        /// <summary>
        ///     The maximum amount of connections the server should be allowed to have
        /// </summary>
        private int maximumConnections;

        private IDataParser parser;

        /// <summary>
        ///     The port information, contains the nummeric value the socket should listen on.
        /// </summary>
        private int portInformation;

        /// <summary>
        ///     Occurs when a new connection was established
        /// </summary>
        public event ConnectionEvent connectionEvent;

        /// <summary>
        /// Contains the ip's and their connection counts
        /// </summary>
        //private Dictionary<string, int> ipConnectionCount;
        private ConcurrentDictionary<string, int> _ipConnectionsCount;
        #endregion

        #region initializer

        /// <summary>
        ///     Initializes the connection instance
        /// </summary>
        /// <param name="portID">The ID of the port this item should listen on</param>
        /// <param name="maxConnections">The maximum amount of connections</param>
        public void init(int portID, int maxConnections, int connectionsPerIP, IDataParser parser,  bool disableNaglesAlgorithm)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();

            this.parser = parser;
            disableNagleAlgorithm = disableNaglesAlgorithm;
            maximumConnections = maxConnections;
            portInformation = portID;
            maxIpConnectionCount = connectionsPerIP;
            prepareConnectionDetails();
            _acceptedConnections = 0;
            log.Info("Successfully setup GameSocketManager on port (" + portID + ")!");
            log.Info("Maximum connections per IP has been set to [" + connectionsPerIP + "]!");
        }

        /// <summary>
        ///     Prepares the socket for connections
        /// </summary>
        private void prepareConnectionDetails()
        {
            connectionListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectionListener.NoDelay = disableNagleAlgorithm;
            try
            {
                connectionListener.Bind(new IPEndPoint(IPAddress.Any, portInformation));
            }
            catch (SocketException ex)
            {
                throw new SocketInitializationException(ex.Message);
            }
        }

        /// <summary>
        ///     Initializes the incoming data requests
        /// </summary>
        public void initializeConnectionRequests()
        {
            //Out.writeLine("Starting to listen to connection requests", Out.logFlags.ImportantLogLevel);
            connectionListener.Listen(100);
            _acceptConnections = true;

            try
            {
                connectionListener.BeginAccept(newConnectionRequest, connectionListener);
            }
            catch
            {
                destroy();
            }
        }

        #endregion

        #region destructor

        /// <summary>
        ///     Destroys the current connection manager and disconnects all users
        /// </summary>
        public void destroy()
        {
            _acceptConnections = false;
            try { connectionListener.Close(); }
            catch { }
            connectionListener = null;
        }

        #endregion

        #region connection request

        /// <summary>
        ///     Handels a new incoming data request from some computer from arround the world
        /// </summary>
        /// <param name="iAr">the IAsyncResult of the connection</param>
        private void newConnectionRequest(IAsyncResult iAr)
        {
            if (connectionListener != null)
            {
                if (_acceptConnections)
                {
                    try
                    {
                        Socket replyFromComputer = ((Socket)iAr.AsyncState).EndAccept(iAr);
                        replyFromComputer.NoDelay = disableNagleAlgorithm;

                        string Ip = replyFromComputer.RemoteEndPoint.ToString().Split(':')[0];

                        int ConnectionCount = getAmountOfConnectionFromIp(Ip);
                        if (ConnectionCount < maxIpConnectionCount)
                        {
                            _acceptedConnections++;
                            ConnectionInformation c = new ConnectionInformation(_acceptedConnections, replyFromComputer, this, parser.Clone() as IDataParser, Ip);
                            reportUserLogin(Ip);
                            c.connectionChanged += c_connectionChanged;

                            if (connectionEvent != null)
                                connectionEvent(c);
                        }
                        else
                        {
                            log.Info("Connection denied from [" + replyFromComputer.RemoteEndPoint.ToString().Split(':')[0] + "]. Too many connections (" + ConnectionCount + ").");
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        connectionListener.BeginAccept(newConnectionRequest, connectionListener);
                    }
                }
                else
                {
                }
            }
        }

        private void c_connectionChanged(ConnectionInformation information, ConnectionState state)
        {
            if (state == ConnectionState.CLOSED)
            {
                reportDisconnect(information);
            }
        }

        #endregion

        #region connection disconnected

        /// <summary>
        ///     Reports a gameconnection as disconnected
        /// </summary>
        /// <param name="gameConnection">The connection which is logging out</param>
        public void reportDisconnect(ConnectionInformation gameConnection)
        {
            gameConnection.connectionChanged -= c_connectionChanged;
            reportUserLogout(gameConnection.getIp());
            //activeConnections.Remove(gameConnection.getConnectionID());
        }

        #endregion

        #region ip connection management

        /// <summary>
        ///     reports the user with an ip as "logged in"
        /// </summary>
        /// <param name="ip">The ip of the user</param>
        private void reportUserLogin(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) + 1));
        }

        /// <summary>
        ///     reports the user with an ip as "logged out"
        /// </summary>
        /// <param name="ip">The ip of the user</param>
        private void reportUserLogout(string ip)
        {
            alterIpConnectionCount(ip, (getAmountOfConnectionFromIp(ip) - 1));
        }

        /// <summary>
        ///     Alters the ip connection count
        /// </summary>
        /// <param name="ip">The ip of the user</param>
        /// <param name="amount">The amount of connections</param>
        private void alterIpConnectionCount(string ip, int amount)
        {
            if (_ipConnectionsCount.ContainsKey(ip))
            {
                int am;
                _ipConnectionsCount.TryRemove(ip, out am);
            }
            _ipConnectionsCount.TryAdd(ip, amount);
        }

        /// <summary>
        ///     Gets the amount of connections from 1 ip
        /// </summary>
        /// <param name="ip">The ip of the user</param>
        /// <returns>The amount of connections from the given ip address</returns>
        private int getAmountOfConnectionFromIp(string ip)
        {
            if (_ipConnectionsCount.ContainsKey(ip))
            {
                return _ipConnectionsCount[ip];
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}