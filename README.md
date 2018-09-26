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

![16](https://user-images.githubusercontent.com/42737061/46079633-4893cb00-c1ca-11e8-83d4-a770e913305e.PNG)
![17](https://user-images.githubusercontent.com/42737061/46079634-4893cb00-c1ca-11e8-978c-466effb165a5.PNG)
![18](https://user-images.githubusercontent.com/42737061/46079635-4893cb00-c1ca-11e8-9f44-9aafb47b54fd.PNG)
![11](https://user-images.githubusercontent.com/42737061/46079638-492c6180-c1ca-11e8-820f-9ad7ec229906.PNG)
![12](https://user-images.githubusercontent.com/42737061/46079639-492c6180-c1ca-11e8-8da2-1bf488012913.PNG)
![13](https://user-images.githubusercontent.com/42737061/46079640-492c6180-c1ca-11e8-9bd2-2a9a4328367b.PNG)
![14](https://user-images.githubusercontent.com/42737061/46079641-49c4f800-c1ca-11e8-9bea-0e1642e6b2e7.PNG)
![15](https://user-images.githubusercontent.com/42737061/46079642-49c4f800-c1ca-11e8-9d4f-1e5111ad2bae.PNG)


