using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using ReliablePubSub.Server;
using NetMQ;
using ReliablePubSub.Client;
using System;

public class RunPubSubExample: MonoBehaviour {
	string debug_name_ = "ReliablePubSub: ";
	void Log(string str)
	{
		Debug.Log (debug_name_ + str);
	}

	// Use this for initialization
	void Start () {
		AsyncIO.ForceDotNet.Force();
		NetMQConfig.ManualTerminationTakeOver();
		NetMQConfig.ContextCreate(true);
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
					_server.Publish(message);
					Log("publishing: " + message);
					Thread.Sleep(100);
					index++;
				}

			});
		serverTread.Start();

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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
