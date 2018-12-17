using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common.DevicePos
{
   public  class ScanerPos
    {
        public decimal X { get; set; }

        public decimal Y { get; set; }


        public decimal Z { get; set; }


        public decimal U { get; set; }

        public decimal V { get; set; }

        public decimal W { get; set; }

        /// <summary>
        /// 放料位置
        /// </summary>
        public static string Pos
        {
            get;
            set;
        }


        public static string TakePos { get; set; }

        public static string MidPos { get; set; }

    }
}
