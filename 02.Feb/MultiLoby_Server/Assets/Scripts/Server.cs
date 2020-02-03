using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

	const int MAX_USER = 100;
	const int PORT = 26000;
	const int WEB_PORT = 26001;

	byte reliableChannel;
	int hostId;
	int webHostId;

	bool isStarted = false;

	#region Monobehaviour
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		Init();
	}
	#endregion

	public void Init() {
		NetworkTransport.Init();

		ConnectionConfig cc = new ConnectionConfig();
		reliableChannel = cc.AddChannel(QosType.Reliable);

		HostTopology topo = new HostTopology(cc, MAX_USER);

		//server only code
		hostId = NetworkTransport.AddHost(topo, PORT, null);
		webHostId = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);
		Debug.Log(string.Format("호스트연결 {0}... Web {1}", PORT, WEB_PORT));
		isStarted = true;
	}

	public void Shutdown() {
		isStarted = false;
		NetworkTransport.Shutdown();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
