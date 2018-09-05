using System;

namespace Server
{
    class MainClsaa
    {

        //主程序开启服务
        public static void Main(string[] args)
        {
            ServerTCP serverTCP = new ServerTCP();
            serverTCP.proto = new ProtocolBytes();
            serverTCP.Start(8762);

            ServerSQL serverSQL = new ServerSQL();
            serverSQL.Connect();

            Scene scene = new Scene();
            RoomMgr roomMgr = new RoomMgr();

            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        serverTCP.Close();
                        return;
                    case "print":
                        serverTCP.Print();
                        break;
                    case "help":
                        Console.WriteLine("帮助信息:\n");
                        Console.WriteLine("-1.输入quit 关闭服务器.");
                        Console.WriteLine("-2.输入print 打印服务端登录信息.");
                        break;

                }
            }
          
        }
    }
}
