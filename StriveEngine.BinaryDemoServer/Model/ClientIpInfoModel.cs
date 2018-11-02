using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoServer.Model
{
    public  class ClientIpInfoModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }

        public int Timeouts { get; set; }

        public int Status { get; set; }

    }
}
