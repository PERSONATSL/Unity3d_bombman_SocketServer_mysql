using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class ProtocolBase
    {
        public virtual ProtocolBase Decode(byte[] readbuff,int start,int length)
        {
            return new ProtocolBase();
        }
        //编码器
        public virtual byte[] Encode()
        {
            return new byte[] { };
        }
        //协议名称，用于消息分发
        public virtual string GetName()
        {
            return "";
        }
        //描述
        public virtual string GetDesc()
        {
            return "";
        }
    }
}
