using System;
using System.Collections.Generic;
using System.Text;

namespace StriveEngine.BinaryDemoCore
{
    /// <summary>
    /// 运算回复。
    /// </summary>
    [Serializable]
    public class ResponseContract
    {
        public ResponseContract() { }
        public ResponseContract(int num1, int num2 ,string opType,int res)
        {
            this.number1 = num1;
            this.number2 = num2;
            this.operationType = opType;
            this.result = res;
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

        private string operationType;
        /// <summary>
        /// 运算类型。
        /// </summary>
        public string OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }

        private int result;
        /// <summary>
        /// 运算结果。
        /// </summary>
        public int Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
