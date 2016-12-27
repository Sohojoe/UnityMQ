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

	Thread _server_thread;
	Thread _client_thread;
	object _threadLock = new object();
	bool _endThread;

	// Use this for initialization
	void Start () {
		AsyncIO.ForceDotNet.Force();
		NetMQConfig.ManualTerminationTakeOver();
		NetMQConfig.ContextCreate(true);
		var _server_thread = new Thread(() =>
			{
				Log("Start server.");
				var _server = new ReliableServer("tcp://*:6669");

				var index = 0;

				while (true)
				{
					lock (_threadLock) {
						if (_endThread)
							break;
					}
					var message = new NetMQMessage();
					message.Append("A");
					message.Append("hello: " + index);
					_server.Publish(message);
					Log("publishing: " + message);
					Thread.Sleep(100);
					index++;
				}
				Log("close server.");
				_server.Dispose();
			});
		_server_thread.Start();

		var _client_thread = new Thread(() =>
			{
				Log("Start client.");
				var _client = new ReliableClient("tcp://localhost:6669");
				_client.Subscribe("A");

				while (true)
				{
					lock (_threadLock) {
						if (_endThread)
							break;
					}

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
				Log("close client.");
				_client.Dispose();
			});
		_client_thread.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit()
	{
		lock (_threadLock)
			_endThread = true;
		Thread.Sleep(200);	
		if (_server_thread != null) {
			_server_thread.Join();
			_server_thread = null;
		}
		if (_client_thread != null) {
			_client_thread.Join();
			_client_thread = null;
		}

		Log("Quit the thread.");
	}
}
