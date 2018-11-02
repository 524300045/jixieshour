using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace StriveEngine.BinaryDemoServer
{
     public class ClientSendInfo
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string IP { get; set; }


        public int ClientSendReceiveStatus;

        public DateTime SendBarCodeTime { get; set; }


        public string BarCode { get; set; }


        public int TotalSendCount { get; set; }

        public IPEndPoint IpPoint { get; set; }


        public DateTime? ReceiveTime { get; set; }


    }
}
