using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using proto.serverproto;
using System;
using proto.clientproto;

public class SocketQueue{
    public Queue<ProtocolNetResponse> queue;
    private Action<ProtocolNetResponse> callback;
    private ConnectSocket socket;
    public static SocketQueue instance;
    public static SocketQueue getInstance()
    {
        if (instance == null)
        {
            instance = new SocketQueue();
            instance.socket = ConnectSocket.getSocketInstance();
            instance.socket.setReiviceCallback(instance.changeBytes);
            instance.queue = new Queue<ProtocolNetResponse>();
        }
        return instance;
    } 

    public void tick()
    {
        if(queue.Count > 0)
        {
            ProtocolNetResponse resp = queue.Dequeue();
            dealPbcMessage(resp);
        }
    }

    public void setCallback(Action<ProtocolNetResponse> callback)
    {
        this.callback = callback;
    }

    public void Push(ProtocolNetResponse resp)
    {
        queue.Enqueue(resp);
    }

    public void changeBytes(byte[] bytes)
    {
        ProtocolNetResponse resp = PBCSerialize.Deserialize<ProtocolNetResponse>(bytes);
        Push(resp);
    }

    void dealPbcMessage(ProtocolNetResponse resp)
    {
        if(resp.cmd == (int)NetRequestType.HEARTBEAT)
        {
            ProtocolNetRequest req = new ProtocolNetRequest();
            req.cmd = (int)NetRequestType.HEARTBEAT;
            socket.sendMSG(PBCSerialize.Serialize(req));
        }else if(resp.cmd == (int)NetRequestType.FORECEEXITGAME)
        {
            Debug.Log("服务器已断开连接,强制退出游戏");
            Application.Quit();
        }
        else
        {
            callback(resp);
        }
        
    }
}
