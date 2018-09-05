using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Reflection;

namespace Server
{
    class ServerTCP
    {
        //监听嵌套字
        public Socket listenfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 100;
        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        //心跳时间
        public long heartBeatTime = 120;
        //协议
        public ProtocolBase proto;

        //玩家登录过程的事件类的实例化
        public HandleConnMsg handleConnMsg = new HandleConnMsg();
        //玩家登陆成功后的事件类的实例化
        public HandleplayerMsg handleplayerMsg = new HandleplayerMsg();
        //玩家事件处理类的实例化
        public HandlePlayerEvent handlePlayerEvent = new HandlePlayerEvent();

        //单例模式
        public static ServerTCP instance;
        public ServerTCP()
        {
            instance = this;
        }


        //获取连接池索引,负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务
        public void Start(int port)
        {
            //连接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }

            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Bind
            IPAddress ipAdr = IPAddress.Any;
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);

            //Listen
            listenfd.Listen(maxConn);

            //Accept（开启异步等待接收客户端连接，接收到客户端连接后返回）
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器] 已启动.");

            //定时器
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;

        }

        //主定时器
        public void HandleMainTimer(object sender,System.Timers.ElapsedEventArgs e)
        {
            //处理心跳
            HeartBeat();
            timer.Start();

        }
        //心跳
        public void HeartBeat()
        {
            long timeNow = Sys.GetTimeStamp();

            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;

                if (conn.lastTickTime < timeNow - heartBeatTime)
                {
                    lock (conn)
                        conn.Close();
                }

            }
        }


        //Accept回调
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("[警告] 连接已达到上限");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAddress();
                    Console.WriteLine("新客户端连接 [" + adr + "]");
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.buffRemain(), SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb 失败:" + e.Message);
            }
        }

        //接收回调
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.socket.EndReceive(ar);
                    //关闭信号
                    if (count <= 0)
                    {
                        Console.WriteLine("收到 [" + conn.GetAddress() + "] 断开连接");
                        ServerSQL.instance.OfflineMsgShow(conn.player.username);
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;                  
                    ProcessData(conn);      
                    //继续接收
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.buffRemain(), SocketFlags.None, ReceiveCb, conn);

                }
                catch (Exception e)
                {
                    Console.WriteLine("收到 [" + conn.GetAddress() + "] 断开连接" + e.Message);
                    conn.Close();
                }
            }
        }
        public void ProcessData(Conn conn)
        {
            //小于长度字节
            if (conn.buffCount < sizeof(Int32))
            {
                return;
            }
            //消息长度

            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
            if(conn.buffCount<conn.msgLength+ sizeof(Int32))
            {
                return;
            }
            //处理消息
            ProtocolBase protocol = proto.Decode(conn.readBuff, sizeof(Int32), conn.msgLength);
            HandleMsg(conn, protocol);

            //清除处理过的消息
            int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            conn.buffCount = count;
            if(conn.buffCount > 0)
            {
                ProcessData(conn);
            }
        }

        public void HandleMsg(Conn conn,ProtocolBase protoBase)
        {
            string name = protoBase.GetName();
            string methodName = "Msg" + name;

            //处理玩家连接
            if(conn.player == null || name == "HeatBeat" || name == "Logout" || name == "Login" || name == "isserver")
            {
                MethodInfo mm = handleConnMsg.GetType().GetMethod(methodName);
                if(mm == null)
                {
                    string str = "[警告]HandleMsg没有处理连接方法";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Object[] obj = new object[] { conn, protoBase };
                //Console.WriteLine("[处理连接的消息]" + conn.GetAddress() + "[" + name + "]");
                mm.Invoke(handleConnMsg, obj);
            }
            //处理玩家方法
            else
            {
                MethodInfo mm = handleplayerMsg.GetType().GetMethod(methodName);
                if(mm == null)
                {
                    string str = "[警告]handleMsg没有处理玩家方法";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Object[] obj = new object[] { conn.player, protoBase };
                Console.WriteLine("[处理玩家的消息]" + conn.player.username + "[" + name + "]");
                mm.Invoke(handleplayerMsg, obj);
            }

        }

        //发送
        public void Send(Conn conn,ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息]" + conn.GetAddress() + ":" + e.Message);
            }
        }

        //广播
        public void Broadcast(ProtocolBase protocol)
        {
            for(int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                Send(conns[i], protocol);

            }
        }

        public void Close()
        {
            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock (conn)
                {
                    conn.Close();
                }
            }
        }
        public void Print()
        {
            Console.WriteLine("===服务器登录信息===");
            for(int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;

                string str = "连接[" + conns[i].GetAddress() + "]";
                if (conns[i].player != null)
                    str += "玩家id" + conns[i].player.username;

                Console.WriteLine(str);
            }
        }
        public void ShowOnlineSQL(Player player)
        {
                string str = player.username;
                ServerSQL.instance.OnlineMsgShow(str);
         
        }

    }
}
