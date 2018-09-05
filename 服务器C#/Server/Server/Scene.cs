using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Scene
    {
        public static Scene instance;
        public Scene()
        {
            instance = this;
        }

        //场景中的角色列表
        List<ScenePlayer> list = new List<ScenePlayer>();

        //根据名字获取ScenePlayer
        private ScenePlayer GetScenePlayer(string username)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].username == username)
                    return list[i];
            }
            return null;
        }

        //添加玩家
        public void AddPlayer(string username)
        {
            //加锁防止多线程同时操作
            lock (list)
            {
                ScenePlayer p = new ScenePlayer();
                p.username = username;
                list.Add(p);
            }
        }
        //删除玩家
        public void DelPlayer(string username)
        {
            lock (list)
            {
                ScenePlayer p = GetScenePlayer(username);
                if (p != null)
                    list.Remove(p);
            }
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("PlayerLeave");
            protocol.AddString(username);
            ServerTCP.instance.Broadcast(protocol);
        }

        //发送列表
        public void SendPlayerList(Player player)
        {
            int count = list.Count;
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetList");
            protocol.AddInt(count);
            for(int i = 0; i < count; i++)
            {
                ScenePlayer p = list[i];
                protocol.AddString(p.username);
                protocol.AddFloat(p.x);
                protocol.AddFloat(p.y);
                protocol.AddFloat(p.z);
                protocol.AddInt(p.score);
                protocol.AddInt(p.kill);
            }
            player.Send(protocol);
        }

        //更新信息
        public void UpdateInfo(string username, float x, float y, float z, int score, int kill)
        {
            int count = list.Count;
            ProtocolBytes protocol = new ProtocolBytes();
            ScenePlayer p = GetScenePlayer(username);
            if (p == null)
                return;
            p.x = x;
            p.y = y;
            p.z = z;
            p.score = score;
            p.kill = kill;
        }

    }
}
