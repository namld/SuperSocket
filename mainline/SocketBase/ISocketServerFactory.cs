﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace SuperSocket.SocketBase
{
    public interface ISocketServerFactory
    {
        /// <summary>
        /// Creates the socket server.
        /// </summary>
        /// <typeparam name="TAppSession">The type of the app session.</typeparam>
        /// <typeparam name="TRequestInfo">The type of the request info.</typeparam>
        /// <param name="appServer">The app server.</param>
        /// <param name="localEndPoint">The local endpoint.</param>
        /// <param name="config">The config.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns></returns>
        ISocketServer CreateSocketServer<TAppSession, TRequestInfo>(IAppServer<TAppSession> appServer, IPEndPoint localEndPoint, IServerConfig config, ICustomProtocol<TRequestInfo> protocol)
            where TAppSession : IAppSession, IAppSession<TAppSession, TRequestInfo>, new()
            where TRequestInfo : IRequestInfo;
    }
}
