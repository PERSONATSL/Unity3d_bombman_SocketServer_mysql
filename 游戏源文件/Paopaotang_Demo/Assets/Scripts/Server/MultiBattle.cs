using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBattle : MonoBehaviour {

    //单例
    public static MultiBattle instance;
    //玩家预设
    public GameObject[] playerPrefabs;

    public Dictionary<string, BattleGround> list = new Dictionary<string, BattleGround>();
    void Start()
    {
        //单例模式
        instance = this;
    }

    //清理战场
    public void ClearBattle()
    {
        list.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
            Destroy(players[i]);
    }

    //开始战斗
    public void StartBattle(ProtocolBytes proto)
    {
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
            return;
        //玩家总数
        int count = proto.GetInt(start, ref start);
        //清理场景
        ClearBattle();
        //每一个玩家
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int swopID = proto.GetInt(start, ref start);
            GeneratePlayer(id, team, swopID);
        }
        NetMgr.srvConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
       // NetMgr.srvConn.msgDist.AddListener("Shooting", RecvShooting);
       // NetMgr.srvConn.msgDist.AddListener("Hit", RecvHit);
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
    }                  


    //产生玩家
    public void GeneratePlayer(string username, int team, int swopID)
    {
        //获取出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;

        if(team == 1)
        {
            Transform teamSwop = sp.GetChild(0);
            swopTrans = teamSwop;
        }
        else
        {
            Transform teamSwop = sp.GetChild(1);
            swopTrans = teamSwop;
        }
        if (swopTrans == null)
        {
            Debug.Log("出生点错误");
            return;
        }

        if (playerPrefabs.Length < 2)
        {
            Debug.Log("预设玩家prefabs不足");
            return;
        }

        //产生玩家
        GameObject playerObj = Instantiate(playerPrefabs[team]);
        playerObj.name = username;
        playerObj.transform.position = swopTrans.position;
        playerObj.transform.rotation = swopTrans.rotation;

        //列表处理
        BattleGround bg = new BattleGround();
        bg.player = playerObj.GetComponent<Player>();
        list.Add(username, bg);

        //玩家处理
        if(username == GameMgr.instance.username)
        {
            bg.player.ctrlType = Player.CtrlType.player;
        }
        else
        {
            bg.player.ctrlType = Player.CtrlType.net;
            bg.player.InitNetCtrl();
        }
    }


    public void RecvUpdateUnitInfo(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        Vector3 nPos;
        Vector3 nRot;
        nPos.x = proto.GetFloat(start, ref start);
        nPos.y = proto.GetFloat(start, ref start);
        nPos.z = proto.GetFloat(start, ref start);
        nRot.x = proto.GetFloat(start, ref start);
        nRot.y = proto.GetFloat(start, ref start);
        nRot.z = proto.GetFloat(start, ref start);
        Debug.Log("RecvUpdateUnitInfo" + id);
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvUpdateUnitInfo bg == null");
            return;
        }
        BattleGround bg = list[id];
        if (id == GameMgr.instance.username)
            return;
        bg.player.NetForecastInfo(nPos, nRot);

    }



    public void RecvResult(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        int winTeam = proto.GetInt(start, ref start);
        //取消监听
        NetMgr.srvConn.msgDist.DelListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        //NetMgr.srvConn.msgDist.DelListener("Shooting", RecvShooting);
        //NetMgr.srvConn.msgDist.DelListener("Hit", RecvHit);
        NetMgr.srvConn.msgDist.DelListener("Result", RecvResult);
    }
}
