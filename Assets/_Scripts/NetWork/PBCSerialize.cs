using UnityEngine;
using System.Collections;
using proto.clientproto;
using proto.serverproto;
using ProtoBuf;
using System.IO;
using System.Text;
using System;

public class PBCSerialize
{

    public static byte[] Serialize(IExtensible msg)
    {
        byte[] result;
        using (var stream = new MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(stream, msg);
            result = stream.ToArray();
        }
        return result;
    }

    public static IExtensible Deserialize<IExtensible>(byte[] message)
    {
        IExtensible result;
        using (var stream = new MemoryStream(message))
        {
            result = ProtoBuf.Serializer.Deserialize<IExtensible>(stream);

        }
        return result;
    }

}
