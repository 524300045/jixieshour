using Atms.Common;
using Atms.Common.DevicePos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atms.Common
{
    public   class ClientInfo
    {

        public System.Net.IPEndPoint IpPoint;

        /// <summary>
        /// 默认为0,发送ready请求后为1
        /// </summary>
        public int State { get; set; }

        public string StateDes { get; set; }


        public string Name { get; set; }

        public string Code { get; set; }

        public string Ip { get; set; }

        /// <summary>
        /// 是否在线 0:离线 1：在线
        /// </summary>
        public int IsOnLine { get; set; }


        public string BarCode { get; set; }

        public MessageResult MsgResult { get; set; }



        //public IOPos Position { get; set; }


        public FctPos FctPos { get; set; }

    }
}
