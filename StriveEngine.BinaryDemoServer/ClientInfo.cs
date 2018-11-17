using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoServer
{
    public   class ClientInfo
    {

        public System.Net.IPEndPoint IpPoint;

        public int State { get; set; }

        public string StateDes { get; set; }


        public string Name { get; set; }

        public string Code { get; set; }

        public string Ip { get; set; }

        /// <summary>
        /// 是否在线 0:离线 1：在线
        /// </summary>
        public int IsOnLine { get; set; }
    }
}
