using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{

       [Serializable]
   public   class MsgRequestContract
    {
           public string Code { get; set; }

           public string Msg { get; set; }

           public string Key { get; set; }
           public MsgRequestContract(string code, string msg) {
               this.Msg = msg;
               this.Code = code;
           }

           public string DeviceCode { get; set; }

    }
}
