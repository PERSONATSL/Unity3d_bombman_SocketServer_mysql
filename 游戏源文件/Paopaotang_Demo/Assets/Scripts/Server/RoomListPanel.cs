using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : MonoBehaviour
{
    public static RoomListPanel instance;
    public Text usernameText;
    public Text winText;
    public Text loseText;
    public Text createRoomNameText;

    public Transform content;
    public GameObject roomCellPrefab;
    public InputField inputRoomName;

    private void Start()
    {
        instance = this;
    }

    public void StartOnshowing()
    {
        OnShowing();
    }

    public void OnShowing()
    {
        NetMgr.srvConn.msgDist.AddListener("GetAchieve", RecvGetAchieve);
        NetMgr.srvConn.msgDist.AddListener("GetRoomList", RecvGetRoomList);

        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.srvConn.Send(protocol);

        protocol = new ProtocolBytes();
        protocol.AddString("GetAchieve");
        NetMgr.srvConn.Send(protocol);
    }

    //关闭监听
    public void OnClosing()
    {
        NetMgr.srvConn.msgDist.DelListener("GetAchieve", RecvGetAchieve);
        NetMgr.srvConn.msgDist.DelListener("GetRoomList", RecvGetRoomList);
    }

    //收到GetAchieve协议
    public void RecvGetAchieve(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int win = proto.GetInt(start, ref start);
        int lose = proto.GetInt(start, ref start);
        //处理
        usernameText.text = "玩家昵称：" + GameMgr.instance.username;
        winText.text = "胜利场数：" + win.ToString();
        loseText.text = "失败场数：" + lose.ToString();
    }

    //收到GetRoomList协议
    public void RecvGetRoomList(ProtocolBase protocol)
    {
        //清理列表
        ClearRoomList();

        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        for (int i = 0; i < count; i++)
        {
            int num = proto.GetInt(start, ref start);
            int status = proto.GetInt(start, ref start);
            GenerateRoomUnit(i, num, status);
        }
    }
    public void ClearRoomList()
    {
        for (int i = 0; i < content.childCount; i++)
            if (content.GetChild(i).name.Contains("Clone"))
                Destroy(content.GetChild(i).gameObject);
    }

  
    public void GenerateRoomUnit(int i, int num, int status)
    {
        //添加房间
        GameObject c = Instantiate(roomCellPrefab);
        c.transform.SetParent(content);
        c.SetActive(true);
        //房间信息
        Transform ctransform = c.transform;
        Text roomNameText = ctransform.Find("NameText").GetComponent<Text>();
        Text roomCountText = ctransform.Find("CountText").GetComponent<Text>();
        Text roomStatusText = ctransform.Find("StatusText").GetComponent<Text>();

        roomNameText.text = "房间号：" + (i + 1).ToString();
        roomCountText.text = "人数：" + num.ToString();

        if (status == 1)
        {
            roomStatusText.color = Color.green;
            roomStatusText.text = "状态：准备中";
        }
        else
        {
            roomStatusText.color = Color.red;
            roomStatusText.text = "状态：游戏中";
        }

        //按钮事件

        Button btn = ctransform.Find("Join").GetComponent<Button>();
        btn.name = i.ToString();
        btn.onClick.AddListener(delegate ()
        {
            OnJoinBtnClick(btn.name);
        });

    }

    //刷新按钮
    public void OnReflashClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.srvConn.Send(protocol);
    }
    //加入按钮
    public void OnJoinBtnClick(string name)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("EnterRoom");

        protocol.AddInt(int.Parse(name));
        NetMgr.srvConn.Send(protocol, OnJoinBtnBack);
        Debug.Log("请求进入房间" + name);
    }

    //加入返回按钮
    public void OnJoinBtnBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if(ret == 0)
        {
            Debug.Log("进入房间成功");
            OnClosing();
            //RoomPanel.instance.OnShowing();
            CreateRoomFloatUI.instance.OnJoinClicked();
        }
        else
        {
            Debug.Log("进入房间失败");
        }
    }

    //新建按钮
    public void OnNewClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreateRoom");
        //protocol.AddString(inputRoomName.text);
        NetMgr.srvConn.Send(protocol, OnNewBack);
        Debug.Log(protocol.GetDesc());
    }

    //新建按钮返回
    public void OnNewBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if(ret == 0)
        {
            Debug.Log("创建成功");
            CreateRoomFloatUI.instance.OnClicked_Match();
        }
        else
        {
            Debug.Log("创建失败");
        }

    }

    //登出按钮
    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }

    //登出返回
    public void OnCloseBack(ProtocolBase protocol)
    {
        Debug.Log("登出成功");
    }
}
