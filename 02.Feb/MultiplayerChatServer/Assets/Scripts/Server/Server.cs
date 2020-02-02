using System.Collections;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class Server : MonoBehaviour
{
    public int port = 6321;

    List<ServerClient> clients = new List<ServerClient>();
    List<ServerClient> disconnectLists = new List<ServerClient>();

    TcpListener server;
    bool serverStarted;

    private void Start() {
        try {
            server = new TcpListener(IPAddress.Any, port);
            server.Start(); //서버오픈

            StartListening(); //서버대기시작
            serverStarted = true;

            Debug.Log("Server has been started on port : " + port.ToString());
        }
        catch (Exception e) {

            Debug.Log("Socket error : " + e.Message);
        }
    }

    private void Update() {
        if (!serverStarted) {
            return;
        }

        foreach (ServerClient c in clients) {
            //클라이언트가 연결돼있는지 판별
            //아니라면 계속 연결종료 클라이언트에 추가하고 패스
            if (!isConnected(c.tcp)) {
                c.tcp.Close();
                disconnectLists.Add(c);
                continue;
            }
            //클라이언트로부터 메세지(패킷(?)) 확인
            else {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable) {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if(data != null) {
                        OnIncomingData(c, data);
                    }
                }
            }
        }

        for (int i = 0; i < disconnectLists.Count - 1; i++) {
            Broadcast(disconnectLists[i].clientName + " has disconnected", clients);
            clients.Remove(disconnectLists[i]);
            disconnectLists.RemoveAt(i);
        }
    }
    private bool isConnected(TcpClient tcp) {
        try {
            if (tcp != null & tcp.Client != null & tcp.Client.Connected) {
                if (tcp.Client.Poll(0, SelectMode.SelectRead)) {
                    return !(tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else {
                return false;
            }
        }
        catch  {

            return false;
        }
    }

    private void StartListening() {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    
    //클라이언트의 연결을 받아내는 함수
    private void AcceptTcpClient(IAsyncResult ar) {
        TcpListener listener = (TcpListener)ar.AsyncState;

        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        // 메세지를 모두에게 보낸다, 연결된 누군가를 말한다
        // 연결된 클라이언트 스트림에 메세지를 보냄
        //Broadcast("[" + clients[clients.Count - 1].clientName + "] 님이 연결됐습니다.", clients);
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
    }

    //데이터를 서버가 받게될때 콜백함수
    private void OnIncomingData(ServerClient c, string data) {

        //Debug.Log(c.clientName + " has sent the following message : " + data);
        if (data.Contains("&NAME")) {
            c.clientName = data.Split('|')[1];
            Broadcast("[" + c.clientName + "] 님이 연결됐습니다.", clients);
            return;
        }

        Broadcast(c.clientName + " : " + data, clients);
    }

    private void Broadcast(string data, List<ServerClient> c1) {
        foreach (ServerClient c in c1) {
            try {
                //서버 클라이언트 스트림을 받아오고 data를 입력한 후 바로 스트림에 입력
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e) {
                Debug.Log($"Write error : {e.Message} to client {c.clientName}");
                
            }
        }
    }
}

public class ServerClient {
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket) {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
