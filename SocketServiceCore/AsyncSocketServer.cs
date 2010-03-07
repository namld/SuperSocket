﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SuperSocket.Common;
using System.Threading;
using System.Collections;
using System.Net;

namespace SuperSocket.SocketServiceCore
{
    public class AsyncSocketServer<TSocketSession, TAppSession> : SocketServerBase<TSocketSession, TAppSession>
        where TAppSession : IAppSession, new()
        where TSocketSession : ISocketSession<TAppSession>, new()     
	{
        public AsyncSocketServer(IAppServer<TAppSession> appServer, IPEndPoint localEndPoint)
            : base(appServer, localEndPoint)
		{
			
		}

		public static ManualResetEvent m_TcpClientConnected = new ManualResetEvent(false);

		private TcpListener m_Listener = null;

        private Thread m_ListenThread = null;

		public override bool Start()
		{
			try
			{
                if (!base.Start())
                    return false;

                if (m_Listener == null)
                {
                    m_ListenThread = new Thread(StartListen);
                    m_ListenThread.IsBackground = true;
                    m_ListenThread.Start();
                }

                return true;				
			}
			catch (Exception e)
			{
				LogUtil.LogError(e);
				return false;
			}			
		}

        private void StartListen()
        {
            m_Listener = new TcpListener(EndPoint);
            m_Listener.Start();
            m_Listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            while (true)
            {
                m_TcpClientConnected.Reset();
                m_Listener.BeginAcceptTcpClient(OnClientConnect, null);
                m_TcpClientConnected.WaitOne();
            }
        }

		public void OnClientConnect(IAsyncResult result)
		{
			TcpClient client = m_Listener.EndAcceptTcpClient(result);
            TSocketSession session = RegisterSession(client);
            m_TcpClientConnected.Set();
			session.Start();
		}

		public override void Stop()
		{
			base.Stop();

            if (m_ListenThread != null)
            {
                m_ListenThread.Abort();
                m_ListenThread = null;
            }

			if (m_Listener != null)
			{
				m_Listener.Stop();
				m_Listener = null;
			}
		}
	}
}
