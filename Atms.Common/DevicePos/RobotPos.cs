using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common
{

    /// <summary>
    /// Robot点位信息
    /// </summary>
    public  class RobotPos
    {

        public decimal X { get; set; }

        public decimal Y { get; set; }


        public decimal Z { get; set; }


        public decimal U { get; set; }

        public decimal V { get; set;}

        public decimal W { get; set; }

        /// <summary>
        /// POS
        /// </summary>
        public string Pos
        {
            get;
            set;
        }


    }
}
