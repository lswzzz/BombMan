using UnityEngine;
using System.Collections;
using System;

public class CustomEventArgs : EventArgs
{

    private byte[] bytes;

    public CustomEventArgs(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public byte[] Bytes {
       get { return bytes; }
    }
}
