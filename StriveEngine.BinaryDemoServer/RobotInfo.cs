using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoServer
{
   public   class RobotInfo
    {
       public static string DeviceCode { get; set; }

       /// <summary>
       /// 0未使用,3 工作中
       /// </summary>
       public static int WorkStatus
       { get; set; }
    }
}
