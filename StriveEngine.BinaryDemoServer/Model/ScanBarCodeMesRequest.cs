using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoServer.Model
{
   public    class ScanBarCodeMesRequest
    {
       /// <summary>
       /// 机台号
       /// </summary>
       public   string DeviceCode { get; set; }


       public string SN { get; set; }

       public string STEP { get; set; }

       /// <summary>
       /// 工号
       /// </summary>
       public string WorkCode { get; set; }

       /// <summary>
       /// 线别
       /// </summary>
       public string LineSort { get; set; }


       public string StatusCode { get; set; }


       public string ExtendOne { get; set; }


       public string ExtendTwo { get; set; }

    }
}
