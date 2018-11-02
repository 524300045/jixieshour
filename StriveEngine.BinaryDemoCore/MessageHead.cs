using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{
    /// <summary>
    /// 消息头定义
    /// </summary>
    public class MessageHead
    {
        public const int HeadLength = 8;

        public MessageHead() { }
        public MessageHead(int bodyLen, int msgType)
        {
            this.bodyLength = bodyLen;
            this.messageType = msgType;
        }

        private int bodyLength;
        /// <summary>
        /// 消息体长度
        /// </summary>
        public int BodyLength
        {
            get { return bodyLength; }
            set { bodyLength = value; }
        }

        private int messageType;
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public byte[] ToStream()
        {
            byte[] buff = new byte[MessageHead.HeadLength];
            byte[] bodyLenBuff = BitConverter.GetBytes(this.bodyLength) ;
            byte[] msgTypeBuff = BitConverter.GetBytes(this.messageType) ;
            Buffer.BlockCopy(bodyLenBuff,0,buff,0,bodyLenBuff.Length) ;
            Buffer.BlockCopy(msgTypeBuff,0,buff,4,msgTypeBuff.Length) ;
            return buff;
        }
    }
}
