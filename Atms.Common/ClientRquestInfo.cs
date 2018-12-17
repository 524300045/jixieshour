
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Atms.Common
{
    public  class ClientRquestInfo
    {
        /// <summary>
        /// 终结点
        /// </summary>
        public IPEndPoint endPoint;

        /// <summary>
        /// 客户端请求类型
        /// </summary>
        public int messageType;
    }
}
