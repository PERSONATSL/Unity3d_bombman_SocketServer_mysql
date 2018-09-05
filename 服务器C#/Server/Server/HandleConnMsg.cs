using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public partial class HandleConnMsg
    {

        //处理心跳协议
        public void MsgHeatBeat(Conn conn, ProtocolBase protoBase)
        {
            conn.lastTickTime = Sys.GetTimeStamp();
        }

        //处理注册
        public void MsgRegister(Conn conn, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;

            string protoName = protocol.GetString(start, ref start);
            string username = protocol.GetString(start, ref start);
            string password = protocol.GetString(start, ref start);
            string strFormat = "[收到注册协议 ]" + conn.GetAddress();

            Console.WriteLine(strFormat + " 用户名： " + username + " 密码： " + password);

            //构建返回协议
            protocol = new ProtocolBytes();
            protocol.AddString("Register");
            //注册
            if (ServerSQL.instance.Register(username, password))
            {
                protocol.AddInt(0);
            }
            else
            {
                protocol.AddInt(-1);
            }
            //创建角色
            ServerSQL.instance.CreatePlayer(username);
            //返回协议给客户端
            conn.Send(protocol);
        }

        //处理登录
        public void MsgLogin(Conn conn, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            string username = protocol.GetString(start, ref start);
            string password = protocol.GetString(start, ref start);
            string strFormat = "[收到登录协议 ]" + conn.GetAddress();
            Console.WriteLine(strFormat + " 用户名： " + username + " 密码： " + password);
            //构建返回协议
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("Login");
            //验证
            if (!ServerSQL.instance.CheckPassWord(username, password))
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            //是否已经登录
            ProtocolBytes protocolLogout = new ProtocolBytes();
            protocolLogout.AddString("Logout");
            if (!Player.KickOff(username, protocolLogout))
            {

                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            //获取玩家数据
            PlayerData playerData = ServerSQL.instance.GetPlayerData(username);
            if (playerData == null)
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            conn.player = new Player(username, conn);
            conn.player.data = playerData;
            //事件触发
            ServerTCP.instance.handlePlayerEvent.OnLogin(conn.player);
            //返回
            protocolRet.AddInt(0);
            conn.Send(protocolRet);
            ServerTCP.instance.ShowOnlineSQL(conn.player);
            return;
        }

        //处理登出
        public void MsgLogout(Conn conn, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            Console.WriteLine("[收到登出协议 ]" + conn.GetAddress() + "\n");
            protocol.AddString("Logout");
            protocol.AddInt(0);
            if (conn.player == null)
            {
                conn.Send(protocol);
                conn.Close();
            }
            else
            {
                conn.Send(protocol);
                conn.player.Logout();
            }
        }

        //处理验证客户端是否已通过外网映射IP连接到服务器
        public void MsgIsServer(Conn conn, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            Console.WriteLine("[收到服务器验证协议 ]" + conn.GetAddress() + "\n");
            protocol.AddString("IsServer");
            conn.Send(protocol);
        }

    }


}
