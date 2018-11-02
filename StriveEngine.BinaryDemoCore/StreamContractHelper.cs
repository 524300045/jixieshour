using System;
using System.Collections.Generic;
using System.Text;
using StriveEngine.Core;

namespace StriveEngine.BinaryDemoCore
{
    public class StreamContractHelper : IStreamContractHelper
    {
        public int MessageHeaderLength
        {
            get { return 8; }
        }

        public int ParseMessageBodyLength(byte[] head)
        {
            return BitConverter.ToInt32(head, 0);
        }
    }
}
