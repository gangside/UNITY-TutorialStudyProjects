using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream ms = new MemoryStream(recBuffer);
				NetMsg msg = (NetMsg)formatter.Deserialize(ms);
				OnData(connectionId, channelId, recHostId, msg);
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

    #region OnData
    private void OnData(int connectionId, int channelId, int recHostId, NetMsg msg) {
		Debug.Log("메세지를 받았습니다.");
		switch (msg.OP) {
			case NetOP.None:
				Debug.Log("잘못된 네트워크 메세지");
				break;
			case NetOP.CreateAccount:
				CreateAccount(connectionId, channelId, recHostId, (Net_CreateAccount)msg);
				break;
			case NetOP.LoginRequest:
				LoginRequest(connectionId, channelId, recHostId, (Net_LoginRequest)msg);
				break;
		}
	}

	private void LoginRequest(int connectionId, int channelId, int recHostId, Net_LoginRequest loginInfo) {
		Debug.Log(string.Format("로그인 정보 : {0},{1}", loginInfo.UsernameOrEmail, loginInfo.Password));

		Net_OnLoginRequest olr = new Net_OnLoginRequest();
		olr.Sucess = 0;
		olr.Information = "로그인에 성공했습니다";
		olr.Username = "morm";
		olr.Discriminator = "0000";
		olr.Token = "TOKEN";

		SendClient(recHostId, connectionId, olr);
	}

	private void CreateAccount(int connectionId, int channelId, int recHostId, Net_CreateAccount ca) {
		Debug.Log(string.Format("회원가입 정보 : {0},{1},{2}", ca.Username, ca.Password, ca.Email));

		Net_OnCreateAccount oca = new Net_OnCreateAccount();
		oca.Sucess = 0;
		oca.Information = "계정이 생성됐습니다";

		SendClient(recHostId, connectionId, oca);
	}
	#endregion

	#region Send
	public void SendClient(int recHost,int cnnId, NetMsg msg) {
		//데이터를 모으는 변수(버퍼)
		byte[] buffer = new byte[BYTE_SIZE];

		//this is where you would crush your data into a byte[]
		//데이터를 바이트로 압축
		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(buffer);
		formatter.Serialize(ms, msg);

		if(recHost == 0 )
			NetworkTransport.Send(hostId, cnnId, reliableChannel, buffer, BYTE_SIZE, out error);
		else
			NetworkTransport.Send(webHostId, cnnId, reliableChannel, buffer, BYTE_SIZE, out error);

	}
	#endregion
}
