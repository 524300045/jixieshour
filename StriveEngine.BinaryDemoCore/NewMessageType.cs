using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{
    public  class NewMessageType
    {
        //客户端发送read命令
        public const int ClientSendReady = 4;
        /// <summary>
        /// 服务端收到ready请求,发送给客户端
        /// </summary>
        public const int ReceiveClientReady = 6;


        //服务端放置好设备，发送条码信息给客户端
        public const int ServerPlaceDevice = 8;

        //客户端发送结果
        public const int ClientSendResult =9;

        //服务端发送收到结果
        public const int SeverReceiveResult = 10;


        /// <summary>
        /// 服务端取走设备,不放置新设备
        /// </summary>
        public const int ServerTakeDevice = 11;
        /// <summary>
        /// 服务端取走设备，并放置新设备
        /// </summary>
        public const int ServerTakeDeviceAndPlaceDevice = 12;


        /// <summary>
        /// 客户端测试完成
        /// </summary>

        public const int ClientSendTestFinished = 13;

        /// <summary>
        /// 服务端收到测试完成
        /// </summary>
        public const int ServerReceiveTestFinished = 14;

    }
}
