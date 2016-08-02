using UnityEngine;
using System.Collections;
using System;
using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("UdpChannel")]
//客户端发送到服务器转换为java的字节序，java字节序直接发送过来客户端，客户端接受服务器的字节序要进行转化
public class SendTransformData {
    public float posx;
    public float posy;
    public char direction;
    public char state;

    public static byte[] ToBytes(SendTransformData data)
    {
        ByteArray array = new ByteArray(10);
        array.writeFloat(data.posx);
        array.writeFloat(data.posy);
        array.writeChar(data.direction);
        array.writeChar(data.state);
        return array.getByteData();
    }
}

public class SendActionData {
    public char action;
    public SendActionBubble bubble;
    public SaveOne saveOne;
    public KillOne killOne;
    public enum ActionType
    {
        Bubble = 1,
        SaveOne,
        KillOne,
    }

    public class SendActionBubble {
        public char row;
        public char col;
        public char power;
    }

    public class SaveOne
    {
        public int fd;
    }

    public class KillOne
    {
        public int fd;
    }

    public static byte[] ToBytes(SendActionData data)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar(data.action);
        if((ActionType)data.action == ActionType.Bubble)
        {
            array.writeChar(data.bubble.row);
            array.writeChar(data.bubble.col);
            array.writeChar(data.bubble.power);
        }else if((ActionType)data.action == ActionType.SaveOne)
        {
            array.writeInt(data.saveOne.fd);
        }else if((ActionType)data.action == ActionType.KillOne)
        {
            array.writeInt(data.killOne.fd);
        }
        return array.getByteData();
    }
}

public class SendStateData
{

    public char state;
    public SendTrapData trapData;

    public class SendTrapData
    {
        public char trapLevel;
    }

    public static byte[] ToBytes(SendStateData data)
    {
        ByteArray array = new ByteArray(10);
        array.writeChar(data.state);
        if(data.state == (char)PlayerState.Trap)
        {
            array.writeChar(data.trapData.trapLevel);
        }
        return array.getByteData();
    }
}

public class SendData
{
    public SendDataType cmd;
    public int fd;
    public SendTransformData transform;
    public SendActionData action;
    public SendStateData state;
}

public enum SendDataType
{
    StartInfo = 1,
    Response,
    Transform,
    Action,
    State,
    CheckTime,
}