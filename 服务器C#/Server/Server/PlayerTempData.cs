using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class PlayerTempData
    {
        public Status status;

        public enum Status
        {
            None,
            Room,
            Fight,
        }
        public PlayerTempData()
        {
            status = Status.None;
        }

        //room状态
        public Room room;
        public int team = 1;
        public bool isOwner = false;
        //战场相关
        public long lastUpdateTime;
        public float posX;
        public float posY;
        public float posZ;
    }
}
