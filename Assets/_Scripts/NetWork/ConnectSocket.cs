using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;

public class ConnectSocket{
    private Socket socket;
    private static ConnectSocket instance;
    //public string IP = "203.195.193.136";
    public string IP = "192.168.2.13";
    public int port = 21365;
    private Action<byte[]> ReceiveCallback;
    public static ConnectSocket getSocketInstance()
    {
        if(instance == null)
        {
            instance = new ConnectSocket();
        }
        return instance;
    }

    ConnectSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse(IP);
        IPEndPoint ipe = new IPEndPoint(ip, port);
        IAsyncResult result = socket.BeginConnect(ipe, new AsyncCallback(connectCallBack), socket);
        bool connectSucces = result.AsyncWaitHandle.WaitOne(1000, true);
        if (connectSucces)
        {
            Thread thread = new Thread(new ThreadStart(getMSG));
            thread.IsBackground = true;
            thread.Start();
        }
        else
        {
            Debug.Log("Time Out");
        }
    }

    public bool isConnect()
    {
        if (socket != null && socket.Connected) return true;
        return false;
    }

    private void connectCallBack(IAsyncResult ast)
    {
        Debug.Log("Connect Success");
    }

    private void getMSG()
    {
        while (true)
        {
            if (!socket.Connected)
            {
                Debug.Log("bread connect");
                socket.Close();
                break;
            }
            try
            {
                byte[] byteLength = new byte[2];
                socket.Receive(byteLength);
                int length = ByteUtil.byteArray2IntN2H(byteLength, 0);
                byte[] bytes = new byte[length];
                int count = 0;
                while(count < length)
                {
                    int tempLength = socket.Receive(bytes);
                    count += tempLength;
                }
                
                ReceiveCallback(bytes);
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
                break;
            }
        }
    }

    public void setReiviceCallback(Action<byte[]> callback)
    {
        ReceiveCallback = callback;
    }

    public void sendMSG(byte[] bytes)
    {
        if (!socket.Connected)
        {
            Debug.Log("bread connect");
            socket.Close();
        }
        try
        {
            short length = (short)bytes.Length;
            byte[] byteLen = ByteUtil.short2ByteArray(length);
            Array.Reverse(byteLen);
            byte[] newByte = ByteUtil.ArrayAdd(byteLen, bytes);
            socket.Send(newByte);
            
        }catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Close()
    {
        socket.Close();
    }

}
