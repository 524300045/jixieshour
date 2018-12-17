using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Atms.Common
{
    public   class MesDeal
    {


        public static MesResponse Connect(String server, String message, int port)
        {
            MesResponse response = new MesResponse();
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                //   Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);
              

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                

                stream.Close();
                client.Close();

                if (responseData.IndexOf("FAIL") >= 0)
                {
                    response.Code = "FAIL";
                    response.Result = responseData;
                }
                else
                {
                    response.Code = "OK";
                    response.Result = responseData;
                    if (responseData.IndexOf("RETEST=1A")>=0)
                    {
                        response.Type = "1A";
                    }
                    if (responseData.IndexOf("RETEST=2A")>=0)
                    {
                        response.Type = "2A";
                    }
                }
               
                return response;
            }
            catch (ArgumentNullException e)
            {

                response.Code = "Exec";
                response.Result = e.Message;
            }
            catch (SocketException e)
            {
                response.Code = "Exec";
                response.Result = e.Message;
            }

            return response;
        }


        public static MesResponse ConnectByBarCode(String server, int port, string barCode)
        {
            string message = MesRequest.GetRequestStr(SystemInfo.FctCode, barCode, "7", SystemInfo.LoginCode);

            MesResponse response = new MesResponse();
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                //   Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);


                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);


                stream.Close();
                client.Close();

                if (responseData.IndexOf("FAIL") >= 0)
                {
                    response.Code = "FAIL";
                    response.Result = responseData;
                }
                else
                {
                    response.Code = "OK";
                    response.Result = responseData;
                }

                return response;
            }
            catch (ArgumentNullException e)
            {

                response.Code = "Exec";
                response.Result = e.Message;
            }
            catch (SocketException e)
            {
                response.Code = "Exec";
                response.Result = e.Message;
            }

            return response;
        }

    }
}
