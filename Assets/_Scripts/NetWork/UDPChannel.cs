using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//两种机制，一种是如果延迟超过一定时间的话那么就在此发送
//第二种如果在延迟时间内，当收到一个回应，而这个回应的顺序并不是按照队列中的顺序来的时候也要重发
public class UDPChannel{

    public class UdpConfirm {
        public byte[] data;
        public float delayTime;
        public int hashCode;
        public SendData saveData;
        public long sendTime;
        public long ReceiveTime;
    }
    private Dictionary<int, UdpConfirm> confirmMap;
    private Action<ReceiveData> callback;
    private Action<SendData> sendCallback;

    private int maxHashCode;
    private float maxDelayTime;
    public static UDPChannel Instance = new UDPChannel();
    private UDPSocket socket;
    private System.Object thisLock = new System.Object();

    public bool isInit = false;

    public void init()
    {
        maxHashCode = 0;
        maxDelayTime = 0.25f;
        isInit = true;
        confirmMap = new Dictionary<int, UdpConfirm>();
        socket = UDPSocket.Instance;
        socket.InitSocket();
        socket.setReceiveCallback(ReceiveData);
    }

    void ReceiveData(byte[] data)
    {
        ByteArray byteArray = new ByteArray(data, (short)data.Length);
        ReceiveDataType type = (ReceiveDataType)byteArray.readChar();
        ReceiveData receiveData = new ReceiveData();
        switch (type) {
            case ReceiveDataType.Response:
                checkConfirm(byteArray.readInt(), byteArray.readChar());
                break;
            case ReceiveDataType.Transform:
                receiveData.cmd = type;
                receiveData.transform = ReceiveTransformData.readFromBytes(byteArray.readBytes());
                callback(receiveData);
                break;
            case ReceiveDataType.Action:
                receiveData.cmd = type;
                SendResponse(byteArray.readInt());
                receiveData.action = ReceiveActionData.readFromBytes(byteArray.readBytes());
                callback(receiveData);
                break;
            case ReceiveDataType.State:
                receiveData.cmd = type;
                SendResponse(byteArray.readInt());
                receiveData.state = ReceiveStateData.readFromBytes(byteArray.readBytes());
                callback(receiveData);
                break;
        }
    }

    void SendResponse(int hashCode)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar((char)SendDataType.Response);
        array.writeInt(hashCode);
        array.writeInt(GameGlobalData.fd);
        socket.SendBytes(array.getByteData());
    }

    public void checkConfirm(int hashCode, char result)
    {
        foreach(UdpConfirm confirm in confirmMap.Values)
        {
            if(hashCode == confirm.hashCode)
            {
                if (result == 1)
                {
                    AcceptConfirm(confirm, true);
                }
                else
                {
                    AcceptConfirm(confirm, false);
                }
                confirmMap.Remove(confirm.hashCode);
                return;
            }
        }
        //暂时将丢包问题隐藏
        //Debug.Log("Error don't has any confirmData HashCode " + hashCode + "   currentCount:" + confirmMap.Count);
    }
    
    public void AcceptConfirm(UdpConfirm confirm, bool result)
    {
        confirm.ReceiveTime = TimeUtils.GetTimeStampNow();
        if (result)
        {
            sendCallback(confirm.saveData);
        }
        float delay = confirm.ReceiveTime - confirm.sendTime;
        delay /= 1000f;
        TimeUtils.GenerateUdpDelayTime(delay);
    }

    public void setCallback(Action<ReceiveData> callback)
    {
        this.callback = callback;
    }

    public void setSendCallback(Action<SendData> callback)
    {
        this.sendCallback = callback;
    }

    public UdpConfirm getConfirm(int hashCode)
    {
        foreach(UdpConfirm confirm in confirmMap.Values)
        {
            if (confirm.hashCode == hashCode) return confirm;
        }
        return null;
    }

    public void tick()
    {
        socket.tick();
        foreach (UdpConfirm confirm in confirmMap.Values)
        {
            confirm.delayTime += Time.deltaTime;
            if(confirm.delayTime >= maxDelayTime)
            {
                socket.SendBytes(confirm.data);
                confirm.delayTime = 0f;
                confirm.sendTime = TimeUtils.GetTimeStampNow();
            }
        }
    }

    
    public void Send(SendData data)
    {
        byte[] byteData = null;
        int hashCode = 0;
        switch (data.cmd)
        {
            case SendDataType.StartInfo:
                hashCode = newHashCode();
                byteData = GenerateStartInfo(data, hashCode);
                break;
            case SendDataType.Transform:
                byteData = GenerateTransform(data);
                break;
            case SendDataType.Action:
                hashCode = newHashCode();
                byteData = GenerateAction(data, hashCode);
                break;
            case SendDataType.State:
                hashCode = newHashCode();
                byteData = GenerateState(data, hashCode);
                break;
        }

        if(data.cmd != SendDataType.Transform)
        {
            UdpConfirm confirm = new UdpConfirm();
            confirm.data = byteData;
            confirm.hashCode = hashCode;
            confirm.delayTime = 0f;
            confirm.saveData = data;
            confirm.sendTime = TimeUtils.GetTimeStampNow();
            confirmMap.Add(hashCode, confirm);
        }
        socket.SendBytes(byteData);
    }

    public int newHashCode()
    {
        int hashCode;
        lock (thisLock)
        {
            hashCode = ++maxHashCode;
        }
        return hashCode;
    }

    public byte[] GenerateStartInfo(SendData data, int hashCode)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar((char)data.cmd);
        array.writeInt(hashCode);
        array.writeInt(GameGlobalData.fd);
        array.writeInt(socket.getPort());
        return array.getByteData();
    }

    public byte[] GenerateTransform(SendData data)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar((char)data.cmd);
        array.writeInt(GameGlobalData.fd);
        array.writeByteArray(SendTransformData.ToBytes(data.transform));
        return array.getByteData();
    }

    public byte[] GenerateAction(SendData data, int hashCode)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar((char)data.cmd);
        array.writeInt(hashCode);
        array.writeInt(GameGlobalData.fd);
        array.writeByteArray(SendActionData.ToBytes(data.action));
        return array.getByteData();
    }

    public byte[] GenerateState(SendData data, int hashCode)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar((char)data.cmd);
        array.writeInt(hashCode);
        array.writeInt(GameGlobalData.fd);
        array.writeByteArray(SendStateData.ToBytes(data.state));
        return array.getByteData();
    }

}


