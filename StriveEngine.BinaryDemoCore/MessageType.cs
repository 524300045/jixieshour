using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{
    public static class MessageType
    {
  
        //客户端发送read命令
        public const int ClientSendReady = 4;

        //服务端返回条码
        public const int ServerResponseBarCode = 5;

        //客户端发送收到请求
        public const int ClientSendReceive = 6;

        //客户端发送结果
        public const int ClientSendResult = 7;

        //服务端发送收到结果
        public const int SeverReceiveResult = 8;



    }
}
