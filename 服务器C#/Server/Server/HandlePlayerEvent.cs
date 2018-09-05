using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class HandlePlayerEvent
    {
        //上线
        public void OnLogin(Player player)
        {
            
        }
        //下线
        public void OnLogout(Player player)
        {
            //房间中
            if (player.tempData.status == PlayerTempData.Status.Room)
            {
                Room room = player.tempData.room;
                RoomMgr.instance.LeaveRoom(player);
                if (room != null)
                    room.Broadcast(room.GetRoomInfo());
            }
            //战斗中
            if (player.tempData.status == PlayerTempData.Status.Fight)
            {
                Room room = player.tempData.room;
                
                RoomMgr.instance.LeaveRoom(player);
            }
        }
    }
}
