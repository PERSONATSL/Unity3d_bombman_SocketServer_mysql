using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    [Serializable]
    public class PlayerData
    {
        public int score = 0;
        public int money = 0;
        public int level = 0;
        public float damage = 0;
        public int kill = 0;
        public int win = 0;
        public int lose = 0;

        public PlayerData()
        {
            score = 100;
            money = 100;
            level = 0;
            damage = 0f;
            kill = 0;
            win = 0;
            lose = 0;
        }
    }
}
