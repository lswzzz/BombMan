using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class ByteUtil {
    
    public static short byteArray2IntN2H(byte[] bt, int starIndex)//byte转int
    {
        Array.Reverse(bt);
        short i = System.BitConverter.ToInt16(bt, starIndex);
        return i;
    }
    public static short byteArray2Short(byte[] bt, int startIndex)
    {
        return System.BitConverter.ToInt16(bt, startIndex);
    }

    public static byte[] short2ByteArray(short num)//int转byte
    {
        return System.BitConverter.GetBytes(num);
    }


    public static byte[] char2ByteArray(char num)
    {
        return System.BitConverter.GetBytes(num);
    }

    public static float byteArray2Float(byte[] bt, int starIndex)//byte转float
    {
        if (System.BitConverter.IsLittleEndian) Array.Reverse(bt);
        float f = System.BitConverter.ToSingle(bt, starIndex);
        return f;
    }
    public static byte[] float2ByteArray(float f)//float转byte
    {
        byte[] bt = System.BitConverter.GetBytes(f);
        if (System.BitConverter.IsLittleEndian) Array.Reverse(bt);
        return bt;
    }

    public static byte[] Serialize<T>(T t)
    {
        MemoryStream mStream = new MemoryStream();
        BinaryFormatter bFormatter = new BinaryFormatter();
        bFormatter.Serialize(mStream, t);
        return mStream.GetBuffer();
    }

    public static T Deserialize<T>(byte[] b)
    {
        BinaryFormatter bFormatter = new BinaryFormatter();
        return (T)bFormatter.Deserialize(new MemoryStream(b));
    }

    public static byte[] ArrayAdd(byte[] main, byte[] sub)
    {
        byte[] ret = new byte[main.Length + sub.Length];
        main.CopyTo(ret, 0);
        sub.CopyTo(ret, main.Length);
        return ret;
    }

    public static byte[] ArrayAdd4(byte[] bytes1, byte[] bytes2, byte[] bytes3, byte[] bytes4)
    {
        byte[] ret = new byte[bytes1.Length + bytes2.Length + bytes3.Length + bytes4.Length];
        bytes1.CopyTo(ret, 0);
        bytes2.CopyTo(ret, bytes1.Length);
        bytes3.CopyTo(ret, bytes1.Length + bytes2.Length);
        bytes4.CopyTo(ret, bytes1.Length + bytes2.Length + bytes3.Length);
        return ret;
    }
}
