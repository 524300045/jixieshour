using Atms.Common.DevicePos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common
{
   public  class IOInfo
    {

       public string Code { get; set; }

     
       public string AreaCode { get; set; }


       public IOPos IoPosition { get; set; }

       /// <summary>
       /// 1表示有 0:表示无
       /// </summary>
       public int IoFlag { get; set; }

       public int i { get; set; }

       public int j { get; set; }
    }
}
