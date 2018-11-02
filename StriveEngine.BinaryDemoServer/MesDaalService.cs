using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace StriveEngine.BinaryDemoServer
{
    public  class MesDaalService
    {

        string messerver = ConfigurationManager.AppSettings["mesip"];
        string port = ConfigurationManager.AppSettings["mesport"];

   

     public      void Connect(String server, int port, String message)
        {
            try
            {

                LogHelper.Log("mes:" + server+":"+port+":"+message);

                TcpClient client = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                NetworkStream stream = client.GetStream();
               // stream.DataAvailable
               
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);
                LogHelper.Log("Connect:" + message);
                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                LogHelper.Log("Received:" + responseData);
                // Close everything.
                stream.Close();
                client.Close();


                LogHelper.Log("MES返回结果:" + responseData);
                if (responseData.IndexOf("T2")>0)
                {
                    #region good处理
                    DealGood();
                    #endregion
                }

                if (responseData.IndexOf("GOTO")>0)
                {
                    #region NG处理
                    DealNG();
                    #endregion
                }

                if (responseData.IndexOf("ASM5")>0)
                {
                    #region 跳站处理
                    DealStep();
                    #endregion
                }

                if (responseData.IndexOf("RETEST=1A")>0)
                {
                    #region 待测处理
                    DealTest();
                    #endregion
                }

                if (responseData.IndexOf("RETEST=2A") > 0)
                {
                    #region 1A处理
                    Deal1A();
                    #endregion
                }


            }
            catch (ArgumentNullException e)
            {
                LogHelper.Log("connect" + e);
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                LogHelper.Log("SocketException" + e);
            }
        }

        #region 处理各种情况
        
  
        /// <summary>
        /// goods处理
        /// </summary>
        private void DealGood()
        { 
            
        }

        /// <summary>
        /// NG处理
        /// </summary>
        private void DealNG()
        { 
        
        }

        /// <summary>
        /// 跳转处理
        /// </summary>

        private void DealStep()
        { 
        
        }

        /// <summary>
        /// 待测处理
        /// </summary>
        public void DealTest()
        { 
        
        }

        /// <summary>
        /// 1A处理
        /// </summary>
        public void Deal1A()
        {


        }

        #endregion

    }
}
