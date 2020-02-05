using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

	const int MAX_USER = 100;
	const int PORT = 26000;
	const int WEB_PORT = 26001;
	const int BYTE_SIZE = 1024;

	byte reliableChannel;
	int hostId;
	int webHostId;
	byte error;

	bool isStarted = false;

	#region Monobehaviour
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		Init();
	}

	private void Update() {
		UpdateMessagePump();
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

	public void UpdateMessagePump() {
		if (!isStarted) return;

		int recHostId; // 이게 웹에서 오냐 스탠드얼론이냐?
		int connectionId; // 유저가 이것을 나에게 보내는가
		int channelId; // 어디서부터 메세지를 보내고 있는가;.

		byte[] recBuffer = new byte[BYTE_SIZE];

		int dataSize;
		byte error;
		
		NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, recBuffer.Length, out dataSize, out error);

		switch (type) {
			case NetworkEventType.DataEvent:
				Debug.Log("데이터 발생");
				break;
			case NetworkEventType.ConnectEvent:
				Debug.Log(string.Format("유저 [{0}]이 로그인 했습니다.", connectionId));
				break;
			case NetworkEventType.DisconnectEvent:
				Debug.Log(string.Format("유저 [{0}]이 종료 했습니다.", connectionId));
				break;
			case NetworkEventType.Nothing:
				break;
			case NetworkEventType.BroadcastEvent:
				Debug.Log("예상되지 않은 타입입니다");
				break;
			default:
				break;
		}
	}
}
