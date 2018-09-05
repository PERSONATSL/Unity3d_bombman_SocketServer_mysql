using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomPanel : MonoBehaviour {

    public static RoomPanel instance;

    public GameObject menu, level;

    public RoomPanel()
    {
        instance = this;
    }

    public void StartOnShowing()
    {
        OnShowing();
    }

    public List<Transform> prefabs = new List<Transform>();

    public void OnShowing()
    {
        //监听
        NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.AddListener("Fight", RecvFight);
                           
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);

    }
    public void OnClosing()
    {
        NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.DelListener("Fight", RecvFight);
    }
    public void RecvGetRoomInfo(ProtocolBase protocol)
    {
        //获取总数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //每个处理
        int i = 0;
        for (i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int win = proto.GetInt(start, ref start);
            int lose = proto.GetInt(start, ref start);
            int isOwner = proto.GetInt(start, ref start);
            //信息处理
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "名字：" + id + "\r\n";
            str += "胜利：" + win.ToString() + "   ";
            str += "失败：" + lose.ToString() + "\r\n";
            if (id == GameMgr.instance.username && isOwner == 1)
                str += "【房主】";
            else
                str += "【玩家】";
            text.text = str;
        }
    }

    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }


    public void OnCloseBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            Debug.Log("退出成功");
        }
        else
        {
            Debug.Log("退出失败");
        }
    }


    public void OnStartClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartFight");
        NetMgr.srvConn.Send(protocol, OnStartBack);
    }

    public void OnStartBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret != 0)
        {
            Debug.Log("开始游戏失败！只有房主可以开始战斗");
        }
    }


    public void RecvFight(ProtocolBase protocol)
    {
        menu.SetActive(false);
        level.SetActive(true);
        ProtocolBytes proto = (ProtocolBytes)protocol;
        MultiBattle.instance.StartBattle(proto);
    }
}
