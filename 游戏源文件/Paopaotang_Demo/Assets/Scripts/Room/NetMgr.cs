using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr
{

    public static ConnectToTheServer srvConn = new ConnectToTheServer();

    public static void Update()
    {
        srvConn.Update();
    }

    //心跳
    public static ProtocolBase GetHeatBeatProtocol()
    {
        //具体的发送内容根据服务端设定改动
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeatBeat");
        return protocol;
    }
}
