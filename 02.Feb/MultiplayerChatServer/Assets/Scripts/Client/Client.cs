using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public GameObject chatContainer;
    public GameObject messagePrefab;

    public string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    InputField hostInput;
    InputField portInput;
    InputField sendInput;

    private void Start() {
        hostInput = GameObject.Find("HostInput").GetComponent<InputField>();
        portInput = GameObject.Find("PortInput").GetComponent<InputField>();
        sendInput = GameObject.Find("SendInput").GetComponent<InputField>();
    }

    public void ConnectedToServer() {

        //이미 소켓에 연결되어 있으면 이 함수를 무시
        if (socketReady) {
            return;
        }

        //기본 호스트 및 포트 세팅
        string host = "127.0.0.1";
        int port = 6321;

        //호스트 및 포트 입력 후 세팅 (인풋필드 값 활용)
        string h;
        int p;

        h = hostInput.text;
        if (h != "") host = h;

        //int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        int.TryParse(portInput.text, out p);
        if (p != 0) port = p;

        //소켓 생성
        try {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();       //흐름 생성
            writer = new StreamWriter(stream); //패킹송신
            reader = new StreamReader(stream); //수신
            socketReady = true;
        }
        catch (System.Exception e) {

            Debug.Log($"Socket error : {e.Message}");
        }
    }

    private void Update() {
        if (socketReady) {
            if (stream.DataAvailable) {
                //스트림에서 데이터를 읽어내고 비어있는지 확인
                //그리고 콜백함수 실행
                string data = reader.ReadLine();
                if (data != null) OnIncomingData(data);
            }
        }
    }

    //데이터를 클라이언트가 받을때 콜백함수
    private void OnIncomingData(string data) {
        Debug.Log("Server : " + data);
        
        if(data == "%NAME") {
            Send("&NAME|" + clientName);
            return;
        }

        GameObject message = Instantiate(messagePrefab, chatContainer.transform);
        message.GetComponentInChildren<Text>().text = data;
    }

    private void Send(string data) {
        if (!socketReady) {
            return;
        }

        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton() {
        string message = sendInput.text;
        Send(message);
    }

    //소켓종료!
    void CloseSocket() {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit() {
        CloseSocket();
    }

    private void OnDisable() {
        CloseSocket();
    }
}
