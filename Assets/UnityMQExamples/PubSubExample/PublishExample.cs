using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ; // for NetMQConfig
using NetMQ.Sockets; 


public class PublishExample : MonoBehaviour {

	string debug_name_ = "Pub: ";

//	Thread client_thread_;
//	private Object thisLock_ = new Object();
	bool stop_thread_ = false;

	void Log(string str)
	{
		Debug.Log (debug_name_ + str);
	}

//	PublisherSocket socket;
//	NetMQPoller poller;

	// Use this for initialization
	void Start () {
////		Log(debug_name_ + "Start thread.");
////		client_thread_ = new Thread(NetMQClient);
////		client_thread_.Start();
//		Log(debug_name_ + "Start");
//		Log("Connect.");
//		var socket = new PublisherSocket("@tcp://localhost:5558");
//		var poller = new NetMQPoller { socket };
//
//		socket.SendReady+= (object sender, NetMQSocketEventArgs e) => {
//			socket.send
//		};
//
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
