# Bombman

*TCP/IP
*Socket
*Server/Client
*Mysql
*Unity3d

** Part of code **
```
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
```

## Screen Shots




