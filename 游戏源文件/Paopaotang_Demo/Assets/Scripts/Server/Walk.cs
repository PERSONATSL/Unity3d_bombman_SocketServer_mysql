using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
public class Walk : MonoBehaviour
{    //预设
    public GameObject prefab;
    //玩家初始位置
    public Transform pos1, pos2;
    //players    
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();    
    //self    
    string playerID = "";    
    //上一次移动的时间
    public float lastMoveTime;    
    //单例
    public static Walk instance;
    string abc = "abc";
    void Start()
    {
        instance = this;
        StartGame(abc);
    }

    ConnectToTheServer connectToTheServer = new ConnectToTheServer();
    MsgDistribution msgDistribytion = new MsgDistribution();
    //添加玩家
    void AddPlayer(string username, Vector3 pos, int score,int kill)
    {
        GameObject player = Instantiate(prefab, pos, Quaternion.identity);
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        //textMesh.text = username + ":" + score;
        players.Add(username, player);
    }   
    //删除玩家
    void DelPlayer(string username)
    {        
        //已经初始化该玩家
        if (players.ContainsKey(username))
        {
            Destroy(players[username]);
            players.Remove(username);
        }
    }

    //更新分数
    public void UpdateScore(string username, int score,int kill)
    {
        GameObject player = players[username];
        if (player == null)
            return;
        TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
        textMesh.text = username + ":" + score;
    } 
    //更新信息
    public void UpdateInfo(string username, Vector3 pos, int score,int kill)
    {    
        //只更新自己的分数
        if (username == playerID)
        {
            UpdateScore(username, score, kill);
            return;
        }    
        //其他人
                                                                  
        //已经初始化该玩家
        if (players.ContainsKey(username))
        {
            players[username].transform.position = pos;
            UpdateScore(username, score, kill);
        }    
        //尚未初始化该玩家
        else
        {
            AddPlayer(username, pos, score, kill);
        }
    }

    public void StartGame(string username)
    {
        playerID = username;   
        //产生自己      
        Vector3 pos = pos1.position;
        AddPlayer(playerID, pos, 0, 0);    
        //同步
        SendPos();    
        //获取列表
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("GetList");
        connectToTheServer.Send(proto, GetList);
        msgDistribytion.AddListener("UpdateInfo", UpdateInfo);
        msgDistribytion.AddListener("PlayerLeave", PlayerLeave);
    } 
    //发送位置
    public void SendPos()
    {
        GameObject player = players[playerID];
        Vector3 pos = player.transform.position;   
        //消息
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateInfo");
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        connectToTheServer.Send(proto);
    }

    //更新列表
    public void GetList(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //获取头部数值
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //遍历
        for (int i = 0; i < count; i++)
        {
            string username = proto.GetString(start, ref start);
            float x = proto.GetFloat(start, ref start);
            float y = proto.GetFloat(start, ref start);
            float z = proto.GetFloat(start, ref start);
            int score = proto.GetInt(start, ref start);
            int kill = proto.GetInt(start, ref start);
            Vector3 pos = new Vector3(x, y, z);
            UpdateInfo(username, pos, score, kill);
        }
    }

    //更新信息
    public void UpdateInfo(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string username = proto.GetString(start, ref start);
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);
        int score = proto.GetInt(start, ref start);
        int kill = proto.GetInt(start, ref start);
        Vector3 pos = new Vector3(x, y, z);
        UpdateInfo(username, pos, score, kill);

    }
    //玩家离开
    public void PlayerLeave(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //获取数值
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string username = proto.GetString(start, ref start);
        DelPlayer(username);
    }


}
