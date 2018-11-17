using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace StriveEngine.BinaryDemo
{
    public partial class TcpClientFormTwo : Form
    {
        public TcpClientFormTwo()
        {
            InitializeComponent();
        }

        private void TcpClientForm_Load(object sender, EventArgs e)
        {
            this.tbIp.Text = "10.5.10.18";
            tbPort.Text = "21347";

        //   this.tbIp.Text = "127.0.0.1";
        //  tbPort.Text = "9000";
            IPAddress ipAddress = IPAddress.Any; //IPAddress.Parse("172.16.102.11");

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(tbIp.Text), Int32.Parse(tbPort.Text));

            NetworkStream ns = tcpClient.GetStream();
            
            if (ns.CanWrite)
            {
                Byte[] sendBytes = Encoding.UTF8.GetBytes(tbMsg.Text);
                ns.Write(sendBytes, 0, sendBytes.Length);
                LogHelper.Log("正常");
                Thread thread = new Thread(new ThreadStart(SocketListen));

                thread.Start();

            }
            else
            {
                MessageBox.Show("不能写入数据流", "终止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Console.WriteLine("You cannot write data to this stream.");
                tcpClient.Close();
                LogHelper.Log("不能写入数据流");
                // Closing the tcpClient instance does not close the network stream.
                ns.Close();
                return;
            }
        }

        public void SocketListen()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(new IPEndPoint(IPAddress.Any, 9999));

            listener.Listen(0);

            Socket socket = listener.Accept();
            Stream netStream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(netStream);

            string result = reader.ReadToEnd();
            Console.WriteLine(result);
            LogHelper.Log("SocketListen:" + result);
            socket.Close();
            listener.Close();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connect(tbIp.Text.Trim(),tbMsg.Text.Trim(),int.Parse(tbPort.Text.Trim()));
        }

         void Connect(String server, String message,int port)
        {
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
                LogHelper.Log("Connect:" + message);
                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                LogHelper.Log("Received:" + responseData);
                // Close everything.
                
                stream.Close();
                client.Close();

                this.richTextBox1.Text += responseData + "\r\n";
            }
            catch (ArgumentNullException e)
            {
                LogHelper.Log("connect"+e);
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                LogHelper.Log("SocketException" + e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

    }
}
