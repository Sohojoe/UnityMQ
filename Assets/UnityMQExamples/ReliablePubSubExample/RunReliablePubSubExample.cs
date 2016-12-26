using UnityEngine;
using System.Threading;
using System.Collections;
using System.Timers;
using NetMQ; // for NetMQConfig
using NetMQ.Sockets;
using ReliablePubSub.Server; 


public class RunReliablePubSubExample: MonoBehaviour {
	string debug_name_ = "Request: ";

	Thread client_thread_;
	private Object thisLock_ = new Object();
	bool stop_thread_ = false;

	ReliableServer _server;
	System.Random _random = new System.Random();

	void Log(string str)
	{
		Debug.Log (debug_name_ + str);
	}


	void Start()
	{
//		Log(debug_name_ + "Start thread.");
//		client_thread_ = new Thread(NetMQServer);
//		client_thread_.Start();

		Log(debug_name_ + "Start server.");
		_server = new ReliableServer("tcp://*:6669");
//		Thread.Sleep(1000);
	}

	void Update()
	{
		/// Do normal Unity stuff
		/// 
		/// 
		var message = new NetMQMessage();
//		message.Append("A");
//		message.Append(_random.Next().ToString());
		message.Append("hello");
		_server.Publish(message);
		Log("publishing: " + message);
		}

	void OnApplicationQuit()
	{
//		lock (thisLock_)stop_thread_ = true;
//		if (client_thread_ != null)
//			client_thread_.Join();
//		client_thread_ = null;
//		Log("Quit the thread.");
		if (_server != null) {
			_server.Dispose ();
			Log("server disposed.");
		}
		_server = null;

	}

}


