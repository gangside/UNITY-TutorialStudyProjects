using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

	public static Client Instance { get; private set; }

	const int MAX_USER = 100;
	const int PORT = 26000;
	const int WEB_PORT = 26001;
	const string SERVER_IP = "127.0.0.1";
	const int BYTE_SIZE = 1024;

	byte reliableChannel;
	int hostId;
	int connectionId;
	byte error;
	bool isStarted = false;

	#region Monobehaviour
	void Start() {
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
		Init();
	}
	
	void Update() {
		UpdateMessagePump();
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
		connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);
		Debug.Log(string.Format("Connecting from Web"));
#else
		//standalone Client
		connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
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
				Debug.Log("로그인 했습니다");
				break;
			case NetworkEventType.DisconnectEvent:
				Debug.Log("접속을 종료했습니다");
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
			case NetOP.OnCreateAccount:
				OnCreateAccount((Net_OnCreateAccount)msg);
				break;
			case NetOP.OnLoginRequest:
				OnLoginRequest((Net_OnLoginRequest)msg);
				break;
		}
	}

	void OnCreateAccount(Net_OnCreateAccount oca) {
		LobbyScene.Instance.EnableInputs();
		LobbyScene.Instance.ChangeWelcomeMessage(oca.Information);
	}

	void OnLoginRequest(Net_OnLoginRequest olr) {

		LobbyScene.Instance.ChangeAuthenticationMessage(olr.Information);

		if(olr.Sucess != 0) {
			//로그인 실패
			LobbyScene.Instance.EnableInputs();
		}
		else {
			//로그인 성공
		}
	}
	#endregion

	#region Send
	public void SendServer(NetMsg msg) {
		//데이터를 모으는 변수(버퍼)
		byte[] buffer = new byte[BYTE_SIZE];

		//this is where you would crush your data into a byte[]
		//데이터를 바이트로 압축
		BinaryFormatter formatter = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(buffer);
		formatter.Serialize(ms, msg);

		NetworkTransport.Send(hostId, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
	}

	public void SendCreateAccount(string username, string password, string email) {
		Net_CreateAccount ca = new Net_CreateAccount();

		ca.Username = username;
		ca.Password = password;
		ca.Email = email;

		SendServer(ca);
	}

	public void SendLoginRequest(string usernameOrEmail, string password) {
		Net_LoginRequest lr = new Net_LoginRequest();

		lr.UsernameOrEmail = usernameOrEmail;
		lr.Password = password;

		SendServer(lr);
	}
	#endregion

}
