// based on: https://github.com/zeromq/netmq/issues/526#issuecomment-230193595

using UnityEngine;
using System.Threading;
using System.Collections;
using System.Timers;
using NetMQ; // for NetMQConfig
using NetMQ.Sockets; 

public class RequestExample: MonoBehaviour
{
	string debug_name_ = "Request: ";

	Thread client_thread_;
	private Object thisLock_ = new Object();
	bool stop_thread_ = false;

	void Log(string str)
	{
		Debug.Log (debug_name_ + str);
	}

	void Start()
	{
		Log(debug_name_ + "Start thread.");
		client_thread_ = new Thread(NetMQClient);
		client_thread_.Start();
	}

	// Client thread which does not block Update()
	void NetMQClient()
	{
		AsyncIO.ForceDotNet.Force();
		NetMQConfig.ManualTerminationTakeOver();
		NetMQConfig.ContextCreate(true);

		string msg;
		var timeout = new System.TimeSpan(0, 0, 1); //1sec

		Log("Connect to the server.");
		var socket = new RequestSocket(">tcp://localhost:5557");

		while (true)
		{
			lock (thisLock_) {
				if (stop_thread_)
					break;
			}
//			Log("Send Request.");
			try {
				socket.SendFrame("hello");
				if (socket.TryReceiveFrameString (timeout, out msg)) {
					Log ("recived: " + msg);
				} else {
					Log ("Timed out, sleep");
					Thread.Sleep (1000);
				}
			} catch (System.Exception ex) {
				Log (ex.Message);
				throw ex;
			}
		}

		socket.Close();
		Log("ContextTerminate.");
		NetMQConfig.ContextTerminate();
	}

	void Update()
	{
		/// Do normal Unity stuff
	}

	void OnApplicationQuit()
	{
		lock (thisLock_)stop_thread_ = true;
		if (client_thread_ != null)
			client_thread_.Join();
		client_thread_ = null;
		Log("Quit the thread.");
	}

}
