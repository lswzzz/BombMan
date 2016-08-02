using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class UDPSocket
{
    UdpClient sendClient;
    IPEndPoint sendIpEnd;
    Thread connectThread;
    Queue<byte[]> recvQueue;
    public static UDPSocket Instance = new UDPSocket();
    private Action<byte[]> callback;
    //加队列是因为
    Queue<byte[]> sendQueue;
    public int port = 21344;
    //public string ip = "203.195.193.136";
    public string ip = "192.168.2.13";
    int localPort;
    bool runThread;
    public bool isInit = false;
    public void InitSocket()
    {
        isInit = true;
        sendIpEnd = new IPEndPoint(IPAddress.Parse(ip), port);
        sendClient = new UdpClient();
        localPort = SocketUtils.GetFirstAvailablePort() + GameGlobalData.fd;
        recvQueue = new Queue<byte[]>();
        sendQueue = new Queue<byte[]>();
        runThread = true;
        connectThread = new Thread(
            new ThreadStart(ReceiveData));
        connectThread.IsBackground = true;
        connectThread.Start();
    }

    public int getPort()
    {
        return localPort;
    }

    void ReceiveData()
    {
        while (runThread)
        {
            try
            {
                IPEndPoint inEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = sendClient.Receive(ref inEndPoint);
                recvQueue.Enqueue(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

        }
    }
    void Send(byte[] bytes)
    {
        sendClient.Send(bytes, bytes.Length, sendIpEnd);
    }

    public void SendBytes(byte[] bytes)
    {
        sendQueue.Enqueue(bytes);
    }

    public void tick()
    {
        while(recvQueue.Count > 0)
        {
            dealReceive(recvQueue.Dequeue());
        }

        while(sendQueue.Count > 0)
        {
            Send(sendQueue.Dequeue());
        }
    }

    public void Close()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if (sendClient != null) sendClient.Close();
        runThread = false;
    }

    public void dealReceive(byte[] data)
    {
        callback(data);
    }

    public void setReceiveCallback(Action<byte[]> callback)
    {
        this.callback = callback;
    }
}
