using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace ReliablePubSub.Client
{
	class SimpleClient
	{
		private NetMQPoller m_poller;
		private NetMQTimer m_timeoutTimer;

		public SimpleClient()
		{

		}

		public void Cancel()
		{
			m_poller.Dispose();
		}

		public void Run(params string[] addresses)
		{
			var subscriber = Connect(addresses);

			if (subscriber == null)
				throw new Exception("cannot connect to eny of the endpoints");

			// timeout timer, when heartbeat was not arrived for 5 seconds
			m_timeoutTimer = new NetMQTimer(TimeSpan.FromSeconds(5));
			m_timeoutTimer.Elapsed += (sender, args) =>
			{
				// timeout happend, first dispose existing subscriber
				subscriber.Dispose();
				m_poller.Remove(subscriber);

				// connect again
				subscriber = Connect(addresses);

				if (subscriber == null)
					throw new Exception("cannot connect to eny of the endpoints");

				m_poller.Add(subscriber);
			};

			m_poller = new NetMQPoller{subscriber};
			m_poller.Add(m_timeoutTimer);

			m_poller.Run();

		}

		private void OnSubscriberMessage(object sender, NetMQSocketEventArgs e)
		{
			var topic = e.Socket.ReceiveFrameString();

			switch (topic)
			{
			case "WM":
				// welcome message, print and reset timeout timer
				Console.WriteLine("Connection drop recongnized");
				m_timeoutTimer.Enable = false;
				m_timeoutTimer.Enable = true;
				Console.WriteLine("Message received. Topic: {0}", topic);
				break;
			case "HB":
				// heartbeat, we reset timeout timer
				m_timeoutTimer.Enable = false;
				m_timeoutTimer.Enable = true;
				Console.WriteLine("Message received. Topic: {0}", topic);
				break;
			default:
				// its a message, reset timeout timer, notify the client, for the example we just print it
				m_timeoutTimer.Enable = false;
				m_timeoutTimer.Enable = true;
				string message = e.Socket.ReceiveFrameString();
				Console.WriteLine("Message received. Topic: {0}, Message: {1}", topic, message);
				break;
			}
		}

		private SubscriberSocket Connect(string[] addresses)
		{
			Console.WriteLine("a1");
			List<SubscriberSocket> sockets = new List<SubscriberSocket>();
			NetMQPoller poller = new NetMQPoller();

			SubscriberSocket connectedSocket = null;

			// event handler to handle message from socket
			EventHandler<NetMQSocketEventArgs> handleMessage = (sender, args) =>
			{
				Console.WriteLine("a5");
				//var topic = args.Socket.ReceiveFrameString();
				//Console.WriteLine("handleMessage Topic: {0}", topic);

				if (connectedSocket == null)
				{
					Console.WriteLine("a6");
					connectedSocket = (SubscriberSocket) args.Socket;
					poller.Stop();
				}
			};

			// If timeout elapsed just cancel the poller without seting the connected socket
			NetMQTimer timeoutTimer = new NetMQTimer(TimeSpan.FromSeconds(5));
			timeoutTimer.Elapsed += (sender, args) => poller.Stop();
			poller.Add(timeoutTimer);

			foreach (var address in addresses)
			{
				var socket = new SubscriberSocket(address);
				sockets.Add(socket);

				socket.ReceiveReady += handleMessage;
				poller.Add(socket);

				// Subscribe to welcome message
				socket.Subscribe("WM");
				socket.Connect(address);
			}

			Console.WriteLine("a2");
			poller.Run();
			Console.WriteLine("a9");

			// if we a connected socket the connection attempt succeed
			if (connectedSocket != null)
			{
				Console.WriteLine("a10");
				// remove the connected socket form the list
				sockets.Remove(connectedSocket);

				// close all existing sockets
				CloseSockets(sockets);

				// drop the welcome message
				connectedSocket.SkipMultipartMessage();

				// subscribe to heartbeat
				connectedSocket.Subscribe("HB");

				// subscribe to our only topic
				connectedSocket.Subscribe("A");

				connectedSocket.ReceiveReady -= handleMessage;
				connectedSocket.ReceiveReady += OnSubscriberMessage;

				return connectedSocket;
			}
			else
			{
				// close all existing sockets
				CloseSockets(sockets);

				return null;
			}
		}

		public void CloseSockets(IList<SubscriberSocket> sockets)
		{
			// close all exsiting connections
			foreach (var socket in sockets)
			{
				// to close them immediatly we set the linger to zero
				socket.Options.Linger = TimeSpan.Zero;
				socket.Dispose();
			}
		}
	}
}