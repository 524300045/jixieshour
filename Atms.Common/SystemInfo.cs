using Atms.Common.DevicePos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common
{
   public  class SystemInfo
    {
       /// <summary>
       /// Ip地址
       /// </summary>
       public static string MesServerIp = "10.5.10.18";

       /// <summary>
       /// 端口
       /// </summary>
       public static int Port = 21347;

       /// <summary>
       /// 当前几台信息
       /// </summary>
       public static string FctCode = "SENTRY-T2-5";


       public static string LoginCode = "M009287";

       public static List<FctPos> FctPosList { get; set; }

       public static List<IOInfo> IoInfoList { get; set; }

       public static List<ClientInfo> ClientInfoList { get; set; }


       public static ConcurrentQueue<ClientRquestInfo> clientRequestCQ { get; set; }
   }
}
