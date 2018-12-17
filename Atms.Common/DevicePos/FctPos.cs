using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common.DevicePos
{
    public class FctPos
    {

        public string IP { get; set; }
       /// <summary>
       /// 低端位置
       /// </summary>
       public  String LowPos { get; set; }

       public  string HighPos { get; set; }


       /// <summary>
       /// 马达位置
       /// </summary>
       public int Pos { get; set; }

    }
}
