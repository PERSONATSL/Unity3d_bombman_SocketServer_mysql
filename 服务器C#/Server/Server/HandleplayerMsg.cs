using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public partial class HandleplayerMsg
    {

        public void MsgGetScore(Player player, ProtocolBase protoBase)
        {
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("GetScore");
            protocolRet.AddInt(player.data.score);
            player.Send(protocolRet);
            Console.WriteLine("MsgGetScore " + player.username + player.data.score);
        }


        public void MsgAddScore(Player player, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            //处理
            player.data.score += 1;
            Console.WriteLine("MsgAddScore " + player.username + " " + player.data.score.ToString());
        }

        //获取玩家列表
        public void MsgGetList(Player player, ProtocolBase protoBase)
        {
            Scene.instance.SendPlayerList(player);
        }
        //更新信息
        public void MsgUpdateInfo(Player player, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);

            float x = protocol.GetFloat(start, ref start);
            float y = protocol.GetFloat(start, ref start);
            float z = protocol.GetFloat(start, ref start);
            int score = player.data.score;
            int kill = player.data.kill;

            Scene.instance.UpdateInfo(player.username, x, y, z, score, kill);
            //广播
            ProtocolBytes protocolRet = new ProtocolBytes();

            protocolRet.AddString("UpdateInfo");
            protocolRet.AddString(player.username);
            protocolRet.AddFloat(x);
            protocolRet.AddFloat(y);
            protocolRet.AddFloat(z);
            protocolRet.AddInt(score);
            protocolRet.AddInt(kill);

            ServerTCP.instance.Broadcast(protocolRet);
        }
        //获取玩家信息
        public void MsgGetAchieve(Player player, ProtocolBase protoBase)
        {
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("GetAchieve");
            protocolRet.AddInt(player.data.win);
            protocolRet.AddInt(player.data.lose);
            player.Send(protocolRet);
        }

        //获取房间列表
        public void MsgGetRoomList(Player player, ProtocolBase protoBase)
        {
            player.Send(RoomMgr.instance.GetRoomList());
        }

        //创建房间
        public void MsgCreateRoom(Player player, ProtocolBase protoBase)
        {
            int start = 0;
            ProtocolBytes protocol = new ProtocolBytes();
            string protoName = protocol.GetString(start, ref start);
            //Room.instance.roomname = protocol.GetString(start, ref start);
            protocol.AddString("CreateRoom");
            //条件检测
            if (player.tempData.status != PlayerTempData.Status.None)
            {
                Console.WriteLine("MsgCreateRoom Fail " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }
            RoomMgr.instance.CreateRoom(player);
            protocol.AddInt(0);
            player.Send(protocol);
            player.tempData.team = 1;
            Console.WriteLine("MsgCreateRoom Ok " + player.username);
        }

        //加入房间
        public void MsgEnterRoom(Player player, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            int index = protocol.GetInt(start, ref start);
            Console.WriteLine("[收到MsgEnterRoom]" + player.username + " " + index);
            
            protocol = new ProtocolBytes();
            protocol.AddString("EnterRoom");
            //判断房间是否存在
            if (index < 0 || index >= RoomMgr.instance.list.Count)
            {
                Console.WriteLine("MsgEnterRoom index error " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }
            Room room = RoomMgr.instance.list[index];
            //判断房间是状态
            if (room.status != Room.Status.Prepare)
            {
                Console.WriteLine("MsgEnterRoom status error " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }
            //添加玩家
            if (room.AddPlayer(player))
            {
                room.Broadcast(room.GetRoomInfo());
                protocol.AddInt(0);
                player.tempData.team = 2;
                player.Send(protocol);
            }
            else
            {
                Console.WriteLine("MsgEnterRoom maxPlayer err " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
            }
        }

        //获取房间信息
        public void MsgGetRoomInfo(Player player, ProtocolBase protoBase)
        {

            if (player.tempData.status != PlayerTempData.Status.Room)
            {
                Console.WriteLine("MsgGetRoomInfo status err " + player.username);
                return;
            }
            Room room = player.tempData.room;
            player.Send(room.GetRoomInfo());
        }

        //离开房间
        public void MsgLeaveRoom(Player player, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("LeaveRoom");

            //条件检测
            if (player.tempData.status != PlayerTempData.Status.Room)
            {
                Console.WriteLine("MsgLeaveRoom status err " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }
            //处理
            protocol.AddInt(0);
            player.Send(protocol);
            Room room = player.tempData.room;
            RoomMgr.instance.LeaveRoom(player);
            //广播
            if (room != null)
                room.Broadcast(room.GetRoomInfo());
        }

        //开始战斗
        public void MsgStartFight(Player player, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("StartFight");
            //条件判断
            if (player.tempData.status != PlayerTempData.Status.Room)
            {
                Console.WriteLine("MsgStartFight status err " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }

            if (!player.tempData.isOwner)
            {
                Console.WriteLine("MsgStartFight owner err " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }

            Room room = player.tempData.room;

            if (!room.CanStart())
            {
                Console.WriteLine("MsgStartFight CanStart err " + player.username);
                protocol.AddInt(-1);
                player.Send(protocol);
                return;
            }
            //开始战斗
            protocol.AddInt(0);
            player.Send(protocol);
            room.StartFight();
        }

        //同步玩家单元
        public void MsgUpdateUnitInfo(Player player, ProtocolBase protoBase)
        {
            //获取数值
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            float posX = protocol.GetFloat(start, ref start);
            float posY = protocol.GetFloat(start, ref start);
            float posZ = protocol.GetFloat(start, ref start);
            float rotX = protocol.GetFloat(start, ref start);
            float rotY = protocol.GetFloat(start, ref start);
            float rotZ = protocol.GetFloat(start, ref start);
            //获取房间
            if (player.tempData.status != PlayerTempData.Status.Fight)
                return;
            Room room = player.tempData.room;
            //作弊校验 略
            player.tempData.posX = posX;
            player.tempData.posY = posY;
            player.tempData.posZ = posZ;
            player.tempData.lastUpdateTime = Sys.GetTimeStamp();
            //广播
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("UpdateUnitInfo");
            protocolRet.AddString(player.username);
            protocolRet.AddFloat(posX);
            protocolRet.AddFloat(posY);
            protocolRet.AddFloat(posZ);
            protocolRet.AddFloat(rotX);
            protocolRet.AddFloat(rotY);
            protocolRet.AddFloat(rotZ);
            room.Broadcast(protocolRet);

        }

        public void MsgGetLoadMsg(Player player, ProtocolBase protoBase)
        {
            ServerSQL.instance.LoadMsg();
        }
        public void MsgGetLoadItemMsg(Player player, ProtocolBase protoBase)
        {
            ServerSQL.instance.LoadItemMsg();
        }
    }
}
