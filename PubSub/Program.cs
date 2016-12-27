using System;
using ReliablePubSub.Server;
using ReliablePubSub.Client;
using System.Threading;
using NetMQ;

namespace PubSub
{
	class MainClass
	{
		static void Log(object msg)
		{
			Console.WriteLine(msg);
		}
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var serverTread = new Thread(() =>
			{
				Log("Start server.");
				var _server = new ReliableServer("tcp://*:6669");

				var index = 0;

				while (true)
				{
					var message = new NetMQMessage();
					message.Append("A");
					message.Append("hello: " + index);
					message.Append("world");
					_server.Publish(message);
					Console.WriteLine("publishing: " + message);
					Thread.Sleep(100);
					index++;
				}

			});
			serverTread.Start();
			//		client_thread_.Start();

			//new SimpleClient().Run("tcp://localhost:6669");
			var clientTread = new Thread(() =>
			{
				Log("Start client.");
				var _client = new ReliableClient("tcp://localhost:6669");
				_client.Subscribe("A");

				while (true)
				{
					NetMQMessage msg = null;
					msg = _client.ReceiveMessage();
					if (msg != null)
					{
						var topic = msg.Pop().ConvertToString();
						var message = msg.Pop().ConvertToString();
						var output = String.Format("reviced topic: {0}, message: {1}", topic, message);
						Log(output);
					}
					Thread.Sleep(10);
				}
			});
			clientTread.Start();

			Console.ReadLine();
		}
	}
}
