using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{
    /// <summary>
    /// 运算请求。
    /// </summary>
    [Serializable]
    public class RequestContract
    {
        public RequestContract() { }
        public RequestContract(int num1, int num2)
        {
            this.number1 = num1;
            this.number2 = num2;
        }

        private int number1;
        /// <summary>
        /// 运算的第一个数。
        /// </summary>
        public int Number1
        {
            get { return number1; }
            set { number1 = value; }
        }

        private int number2;
        /// <summary>
        /// 运算的第二个数。
        /// </summary>
        public int Number2
        {
            get { return number2; }
            set { number2 = value; }
        }
    }
}
