using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atms.Common
{
    public  class MesRequest
    {
        /// <summary>
        /// 获取请求字符串
        /// </summary>
        /// <param name="fctCode"></param>
        /// <param name="barCode"></param>
        /// <param name="step"></param>
        /// <param name="loginCode"></param>
        /// <returns></returns>
        public static String GetRequestStr(string fctCode,string barCode,string step,string loginCode)
        {

            barCode = barCode.Trim('\0');
            barCode = barCode.Trim('\r');
            barCode = barCode.Trim('\n');

            return fctCode + "," + barCode.Trim('\0').Trim('\r').Trim('\n') + "," + step + "," + loginCode + ",SENTRY,,OK,,";
        }

        public static String Get2ARequestStr(string fctCode, string barCode, string step, string loginCode)
        {

            return fctCode + "," + barCode + "," + step + "," + loginCode + ",SENTRY,,OK,1A_STATION=???";
        }
    }
}
