using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

	const int MAX_USER = 100;
	const int PORT = 26000;
	const int WEB_PORT = 26001;
	const string SERVER_IP = "127.0.0.1";

	byte reliableChannel;
	int hostId;
	byte error;
	bool isStarted = false;

	#region Monobehaviour
	void Start() {
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
		hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
		//web client
		NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
		Debug.Log(string.Format("Connecting from Web"));
#else
		//standalone Client
		NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
		Debug.Log(string.Format("Connecting from standalone"));
#endif
		
		Debug.Log(string.Format("연결 시도하는 중 {0}...", SERVER_IP));
		isStarted = true;
		Debug.Log(string.Format("연결 시도 완료"));
	}

	public void Shutdown() {
		isStarted = false;
		NetworkTransport.Shutdown();
	}

	// Update is called once per frame
	void Update() {

	}
}
