using UnityEngine;
using System.Collections;

public class ReceiveTransformData {
    public int fd;
    public float posx;
    public float posy;
    public char direction;
    public char state;
    public static ReceiveTransformData readFromBytes(byte[] bytes)
    {
        ByteArray array = new ByteArray(bytes, (short)bytes.Length);
        ReceiveTransformData data = new ReceiveTransformData();
        data.fd = array.readInt();
        data.posx = array.readFloat();
        data.posy = array.readFloat();
        data.direction = array.readChar();
        data.state = array.readChar();
        return data;
    }
}

public class ReceiveActionData
{
    public int fd;
    public char action;
    public ReceiveActionBubble bubble;
    public enum ActionType
    {
        Bubble = 1,
        BeSave,
        BeKill,
    }
    public class ReceiveActionBubble
    {
        public char row;
        public char col;
        public char power;
    }

    public static ReceiveActionData readFromBytes(byte[] bytes)
    {
        ByteArray array = new ByteArray(bytes, (short)bytes.Length);
        ReceiveActionData data = new ReceiveActionData();
        data.fd = array.readInt();
        data.action = array.readChar();
        if(data.action == (char)ActionType.Bubble)
        {
            data.bubble = new ReceiveActionBubble();
            data.bubble.row = array.readChar();
            data.bubble.col = array.readChar();
            data.bubble.power = array.readChar();
        }
        return data;
    }
}

public class ReceiveStateData
{
    public int fd;
    public char state;
    public ReceiveTrapData trapData;
    public class ReceiveTrapData{
        public char trapLevel;
    }
    public static ReceiveStateData readFromBytes(byte[] bytes)
    {
        ByteArray array = new ByteArray(bytes, (short)bytes.Length);
        ReceiveStateData data = new ReceiveStateData();
        data.fd = array.readInt();
        data.state = array.readChar();
        if(data.state == (char)PlayerState.Trap)
        {
            data.trapData = new ReceiveTrapData();
            data.trapData.trapLevel = array.readChar();
        }
        return data;
    }
}

public class ReceiveData
{
    public ReceiveDataType cmd;
    public ReceiveTransformData transform;
    public ReceiveActionData action;
    public ReceiveStateData state;
}

public enum ReceiveDataType
{
    Response = 1,
    Transform,
    Action,
    State,
}