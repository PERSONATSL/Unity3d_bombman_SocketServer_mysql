using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public class ServerSQL
    {

        //static string host = "192.168.2.120";
        //static string host = "192.168.43.233";
        static string host = "127.0.0.1";
        static string database = "onlinegame";
        static string user = "root";
        static string password = "root";

        public MySqlConnection con = null;

        private string connectStr = "Server=" + host + ";Database=" + database + ";User=" + user + ";Password=" + password;

        //单例模式
        public static ServerSQL instance;
        public ServerSQL()
        {
            instance = this;
        }

        //防止SQL注入
        public bool IsSafeStr(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\'|\<|\>]");
        }

        public void Connect()
        {
            try
            {
                con = new MySqlConnection(connectStr);
                con.Open();
                Console.WriteLine("[数据库] 连接成功\n");
                                            
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库] 连接失败" + e.Message);
            }
        }

        //是否存在该用户
        private bool CanRegister(string username)
        {
            //防sql注入
            if (!IsSafeStr(username))
                return false;
            //查询username是否存在
            string cmdStr = string.Format("SELECT * FROM usermsg WHERE username ='{0}';", username);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                bool hasRows = rdr.HasRows;
                rdr.Close();
                return !hasRows;
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]CanRegister 失败" + e.Message);
                return false;
            }
        }

        //注册
        public bool Register(string username,string password)
        {
            //防sql注入
            if(!IsSafeStr(username) || !IsSafeStr(password))
            {
                Console.WriteLine("[ServerSQL]Register 使用非法字符");
                return false;
            }
            //能否注册
            if (!CanRegister(username))
            {
                Console.WriteLine("[ServerSQL]Register 已存在该用户名");
                return false;
            }
            //写入数据库usermsg表中
            string cmdStr = string.Format("INSERT INTO usermsg SET username ='{0}',password='{1}';", username, password);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]Register " + e.Message);
                return false;
            }
        }

        //创建角色
        public bool CreatePlayer(string username)
        {
            //防sql注入
            if (!IsSafeStr(username))
                return false;
            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            PlayerData playerData = new PlayerData();
            try
            {
                formatter.Serialize(stream, playerData);
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]CreatePlayer" + e.Message);
                return false;
            }
            byte[] byteArr = stream.ToArray();

            //写入数据库

            string cmdStr = string.Format("INSERT INTO userdatamsg SET username ='{0}',userdata=@data;", username);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byteArr;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]CreatePlayer 写入" + e.Message);
                return false;
            }
        }

        //检测用户名和密码
        public bool CheckPassWord(string username,string password)
        {
            //防sql注入
            if (!IsSafeStr(username) || !IsSafeStr(password))
                return false;
            //查询
            string cmdStr = string.Format("SELECT * FROM usermsg WHERE username = '{0}' AND password = '{1}';", username, password);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                bool hasRows = rdr.HasRows;
                rdr.Close();
                return hasRows;
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]CheckPaaaWord" + e.Message);
                return false;
            }
        }

        //获取LoadMsg
        public void LoadMsg()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetLoadMsg");
            //查询
            string cmdStr = string.Format("SELECT notice FROM noticemsg");
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                {
                    string c= rdr[0].ToString();
                    protocol.AddString(c);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]LoadMsg 错误" + e.Message);
            }
            rdr.Close();
            ServerTCP.instance.Broadcast(protocol);

        }
        //获取LoadItemMsg
        public void LoadItemMsg()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetLoadItemMsg");

            //查询所有物品名字
            string cmdStr = string.Format("SELECT itemname FROM itemmsg");
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                while (rdr.Read())
                {
                    string c = rdr[0].ToString();
                    protocol.AddString(c);
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]LoadMsg 错误" + e.Message);
            }
            ////查询所有物品名字
            //string cmdStr1 = string.Format("SELECT itemprice FROM itemmsg");
            //MySqlCommand cmd1 = new MySqlCommand(cmdStr1, con);
            //MySqlDataReader rdr1 = cmd.ExecuteReader();
            //try
            //{
            //    while (rdr1.Read())
            //    {
            //        string c = rdr1[0].ToString();
            //        protocol.AddString(c);
            //    }
            //    rdr1.Close();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("[ServerSQL]LoadMsg 错误" + e.Message);
            //}
            //查询所有物品名字
            string cmdStr2 = string.Format("SELECT itemdesc FROM itemmsg");
            MySqlCommand cmd2 = new MySqlCommand(cmdStr2, con);
            MySqlDataReader rdr2 = cmd.ExecuteReader();
            try
            {
                while (rdr2.Read())
                {
                    string d = rdr2[0].ToString();
                    protocol.AddString(d);
                }
                rdr2.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]LoadMsg 错误" + e.Message);
            }
            ////查询所有物品名字
            //string cmdStr3 = string.Format("SELECT itemlevel FROM itemmsg");
            //MySqlCommand cmd3 = new MySqlCommand(cmdStr3, con);
            //MySqlDataReader rdr3 = cmd.ExecuteReader();
            //try
            //{
            //    while (rdr3.Read())
            //    {
            //        string c = rdr3[0].ToString();
            //        //protocol.AddString(c);
            //    }
            //    rdr3.Close();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("[ServerSQL]LoadMsg 错误" + e.Message);
            //}

            ServerTCP.instance.Broadcast(protocol);
        }

        //获取玩家数据
        public PlayerData GetPlayerData(string username)
        {
            PlayerData playerData = null;
            //防sql注入
            if (!IsSafeStr(username))
                return playerData;
            //查询
            string cmdStr = string.Format("SELECT * FROM userdatamsg WHERE username = '{0}';", username);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            byte[] buffer = new byte[1];
            try
            {
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (!rdr.HasRows)
                {
                    rdr.Close();
                    return playerData;
                }
                rdr.Read();
                long len = rdr.GetBytes(1, 0, null, 0, 0);
                buffer = new byte[len];
                rdr.GetBytes(1, 0, buffer, 0, (int)len);
                rdr.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]GetPlayerData 查询" + e.Message);
                return playerData;
            }

            //反序列化

            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                playerData = (PlayerData)formatter.Deserialize(stream);
                return playerData;
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]GetPlayerData 反序列化" + e.Message);
                return playerData;
            }
        }

        //保存角色
        public bool SavePlayer(Player player)
        {
            string username = player.username;
            PlayerData playerData = player.data;

            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, playerData);
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]SavePlayer序列化" + e.Message);
                return false;
            }
            byte[] byteArr = stream.ToArray();
            //写入数据库

            string formatStr = "UPDATE userdatamsg SET userdata =@data WHERE username = '{0}';";
            string cmdStr = string.Format(formatStr, player.username);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byteArr;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("[ServerSQL]SavePlayer 写入" + e.Message);
                return false;
            }
        }

        public void OnlineMsgShow(string str)
        {
            //写入数据库usermsg表中
            string cmdStr = string.Format("INSERT INTO onlinemsg SET username ='{0}'", str);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]Register " + e.Message);

            }
        }
        public void OfflineMsgShow(string str)
        {
            //写入数据库usermsg表中
            string cmdStr = string.Format("DELETE FROM onlinemsg WHERE username ='{0}'", str);
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ServerSQL]Register " + e.Message);

            }
        }
    }
}
