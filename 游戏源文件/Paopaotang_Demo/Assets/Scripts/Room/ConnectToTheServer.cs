using UnityEngine;
using System;
using System.Net.Sockets;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ConnectToTheServer
{


    public Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];


    private string serverHost = "121.42.201.245";
    private int serverPost = 14286;

    private string serverHost0 = "127.0.0.1";
    private int serverPost0 = 8762;

    public static ConnectToTheServer instance;
    public ConnectToTheServer()
    {
        instance = this;
    }

    //处理粘包分包
    int buffCount = 0;
    byte[] lenBytes = new byte[sizeof(UInt32)];
    Int32 msgLength = 0;

    //协议
    ProtocolBase proto = new ProtocolBytes();

    //消息分发
    public MsgDistribution msgDist = new MsgDistribution();

    //心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 30;

    public void Update()
    {
        //消息
        msgDist.Update();
            if (Time.time - lastTickTime > heartBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeatBeatProtocol();
                Send(protocol);
                lastTickTime = Time.time;
            }
    }

    public void ConnentToTheServer()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(serverHost0, serverPost0);
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            SendToTestTheServer();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ConnectLogUI.instance.str = " 错误，请检查网络连接.";
            ConnectLogUI.instance.isTexted = true;
            ConnectLogUI.instance.isFailed = true;
            socket.Close();
        }

    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount += count;
            ProcessData();

            //继续接收
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void ProcessData()
    {
        //小于长度字节
        if (buffCount < sizeof(Int32))
        {
            return;
        }
        //消息长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount < msgLength + sizeof(Int32))
        {
            return;
        }
        //处理消息
        ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);

        lock (msgDist.msgList)
        {
            msgDist.msgList.Add(protocol);
        }
        //清除处理过的消息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
        {
            ProcessData();
        }
    }
    public bool Send(ProtocolBase protocol)
    {
        byte[] b = protocol.Encode();
        byte[] length = BitConverter.GetBytes(b.Length);
        byte[] sendbuff = length.Concat(b).ToArray();
        socket.Send(sendbuff);
        return true;
    }

    public bool Send(ProtocolBase protocol, string cbName, MsgDistribution.Delegate cb)
    {
        msgDist.AddOnceListener(cbName, cb);
        return Send(protocol);
    }

    public bool Send(ProtocolBase protocol, MsgDistribution.Delegate cb)
    {
        string cbName = protocol.GetName();
        return Send(protocol, cbName, cb);
    }

    //验证客户端是否已通过外网映射IP连接到服务器
    private void SendToTestTheServer()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("IsServer");
        Send(protocol, BackToTestTheServer);
    }
    public void BackToTestTheServer(ProtocolBase protocol)
    {

        ProtocolBytes proto = (ProtocolBytes)protocol;
        string str = proto.GetName();
        if (str == "IsServer")
        {
            ConnectLogUI.instance.str = " 连接成功，请登录.";
            ConnectLogUI.instance.isTexted = true;
            ConnectLogUI.instance.isSucced = true;
        }

    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitTheGame()
    {
        Application.Quit();
    }


}
