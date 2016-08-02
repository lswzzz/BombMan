using UnityEngine;
using System.Collections;
using System;


//写入的byteArray大小用short
public class ByteArray{
    //根据BitConverter.IsLittleEndian判断当前机器是大端还是小段
    //写入的数据是大端模式，这里保证存储的数据是大端模式，那么读取就能是正常的了
    private byte[] byteBuffer;
    private short read_index;
    private short write_index;
    private short capacity;
    //当写的时候Big_Endian的话就是转成大端模式
    //当读的时候指明当前byte[]数据的模式
    private bool Big_Endian;
    public ByteArray(short size)
    {
        byteBuffer = new byte[size];
        write_index = read_index = 0;
        capacity = size;
        Big_Endian = true;
    }

    public ByteArray(byte[] data, short len)
    {
        byteBuffer = data;
        write_index = read_index = 0;
        capacity = len;
        Big_Endian = true;
    }

    public void setEndian(bool endian)
    {
        Big_Endian = endian;
    }

    public void writeChar(char data)
    {
        judgeCapacityWriteSize(1);
        byteBuffer[write_index++] = (byte)data;
    }

    public void WriteReverse(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            if (Big_Endian) Array.Reverse(data);
        }
        else
        {
            if (!Big_Endian) Array.Reverse(data);
        }
    }

    public void ReadReverse(byte[] data)
    {
        if (Big_Endian)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
        }
        else
        {
            if (!BitConverter.IsLittleEndian) Array.Reverse(data);
        }
    }

    public void writeShort(short data)
    {
        judgeCapacityWriteSize(2);
        byte[] bytes = System.BitConverter.GetBytes(data);
        WriteReverse(bytes);
        writeLoop(bytes, sizeof(short));
    }

    public void writeInt(int data)
    {
        judgeCapacityWriteSize(sizeof(int));
        byte[] bytes = System.BitConverter.GetBytes(data);
        WriteReverse(bytes);
        writeLoop(bytes, sizeof(int));
    }

    public void writeString(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        short len = (short)bytes.Length;
        judgeCapacityWriteSize((short)(bytes.Length + 2));
        writeShort(len);
        writeLoop(bytes, (short)bytes.Length);
    }

    public void writeByteArray(byte[] array)
    {
        short len = (short)array.Length;
        short totalLen = len;
        totalLen += 2;
        judgeCapacityWriteSize(totalLen);
        writeShort(len);
        writeLoop(array, len);
    }

    public void writeLoop(byte[] data, short size)
    {
        for (short i = 0; i < size; i++)
        {
            byteBuffer[write_index++] = data[i];
        }
    }

    public void writeFloat(float data)
    {
        judgeCapacityWriteSize(sizeof(float));
        byte[] bytes = System.BitConverter.GetBytes(data);
        WriteReverse(bytes);
        writeLoop(bytes, sizeof(float));
    }

    public void writeDouble(double data)
    {
        judgeCapacityWriteSize(sizeof(double));
        byte[] bytes = System.BitConverter.GetBytes(data);
        WriteReverse(bytes);
        writeLoop(bytes, sizeof(double));
    }

    public void writeLong(long data)
    {
        judgeCapacityWriteSize(sizeof(long));
        byte[] bytes = System.BitConverter.GetBytes(data);
        WriteReverse(bytes);
        writeLoop(bytes, sizeof(long));
    }

    public void Expansion()
    {
        byte[] newByte = new byte[capacity * 2];
        byteBuffer.CopyTo(newByte, 0);
        byteBuffer = newByte;
        capacity *= 2;
    }

    public void judgeCapacityWriteSize(short size)
    {
        while (capacity < write_index + size)
        {
            Expansion();
        }
    }

    public char readChar()
    {
        return (char)byteBuffer[read_index++];
    }

    public short readShort()
    {
        byte[] copy = new byte[sizeof(short)];
        Buffer.BlockCopy(byteBuffer, read_index, copy, 0, sizeof(short));
        ReadReverse(copy);
        short data = System.BitConverter.ToInt16(copy, 0);
        read_index += sizeof(short);
        return data;
    }

    public int readInt()
    {
        byte[] copy = new byte[sizeof(int)];
        Buffer.BlockCopy(byteBuffer, read_index, copy, 0, sizeof(int));
        ReadReverse(copy);
        int data = System.BitConverter.ToInt32(copy, 0);
        read_index += sizeof(int);
        return data;
    }

    public long readLong()
    {
        byte[] copy = new byte[sizeof(long)];
        Buffer.BlockCopy(byteBuffer, read_index, copy, 0, sizeof(long));
        ReadReverse(copy);
        long data = System.BitConverter.ToInt64(copy, 0);
        read_index += sizeof(long);
        return data;
    }

    public float readFloat()
    {
        byte[] copy = new byte[sizeof(float)];
        Buffer.BlockCopy(byteBuffer, read_index, copy, 0, sizeof(float));
        ReadReverse(copy);
        float data = System.BitConverter.ToSingle(copy, 0);
        read_index += sizeof(float);
        return data;
    }

    public double readDouble()
    {
        byte[] copy = new byte[sizeof(double)];
        Buffer.BlockCopy(byteBuffer, read_index, copy, 0, sizeof(double));
        ReadReverse(copy);
        double data = System.BitConverter.ToDouble(copy, 0);
        read_index += sizeof(double);
        return data;
    }

    public string readString()
    {
        byte[] data = readBytes();
        return System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
    }

    public byte[] readBytes()
    {
        short len = readShort();
        byte[] newByte = new byte[len];
        Array.Copy(byteBuffer, read_index, newByte, 0, len);
        read_index += len;
        return newByte;
    }

    public byte[] getByteData()
    {
        byte[] data = new byte[write_index];
        Array.Copy(byteBuffer, data, write_index);
        return data;
    }
}
