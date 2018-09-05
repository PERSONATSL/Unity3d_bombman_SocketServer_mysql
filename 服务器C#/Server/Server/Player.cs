using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Player
    {
        public string username;
        public Conn conn;
        public PlayerData data;
        //临时数据
        public PlayerTempData tempData;

        public Player(string username,Conn conn)
        {
            this.username = username;
            this.conn = conn;
            tempData = new PlayerTempData();
        }

        //发送
        public void Send(ProtocolBase proto)
        {
            if (conn == null)
                return;
            ServerTCP.instance.Send(conn, proto);
        }
        //踢下线
        public static bool KickOff(string username,ProtocolBase proto)
        {
            Conn[] conns = ServerTCP.instance.conns;
            for(int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                if(conns[i].player.username == username)
                {
                    lock (conns[i].player)
                    {
                        if (proto != null)
                            conns[i].player.Send(proto);
                        return conns[i].player.Logout();
                    }
                }
            }
            return true;
        }

        //下线
        public bool Logout()
        {
            ServerTCP.instance.handlePlayerEvent.OnLogout(this);
            //保存
            if (!ServerSQL.instance.SavePlayer(this))
                return false;
            //下线
            conn.player = null;
            conn.Close();
            return true;
        }
    }
}
