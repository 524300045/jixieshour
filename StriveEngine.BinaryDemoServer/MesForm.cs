using Atms.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StriveEngine.BinaryDemoServer
{
    public partial class MesForm : Form
    {

        //   SerialPort serialPort1 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);      //初始化串口设置

        SerialPort serialPort1;
        public delegate void DisplaydelegateOne(byte[] InputBuf);

        public DisplaydelegateOne disp_delegate;

        public MesForm()
        {
            InitializeComponent();
        }



        private void MesForm_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            this.cbCom.Items.AddRange(ports);
            if (ports != null && ports.Count() > 0)
            {
                this.cbCom.SelectedIndex = 0;
            }

            disp_delegate = new DisplaydelegateOne(DispUI);
          
        }

        public void DispUI(byte[] InputBuf)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            string info = encoding.GetString(InputBuf);

            rtbReceive.Text += info + "\r\n";

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (serialPort1 == null)
            {
                serialPort1 = new SerialPort(this.cbCom.Text, int.Parse(tbRate.Text.Trim()), Parity.Even, 7, StopBits.One);
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(Comm_DataReceived);
            }
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }

                this.btnClose.Enabled = true;
                this.btnOpen.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;

            Byte[] InputBuf = new Byte[128];

            try
            {
                serialPort1.Read(InputBuf, 0, serialPort1.BytesToRead);                                //读取缓冲区的数据直到“}”即0x7D为结束符
                //InputBuf = UnicodeEncoding.Default.GetBytes(strRD);             //将得到的数据转换成byte的格式
                System.Threading.Thread.Sleep(50);
                this.Invoke(disp_delegate, InputBuf);

            }
            catch (TimeoutException ex)         //超时处理
            {
                LogHelper.Log("Comm_DataReceived:" + ex.Message);
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(tbContent.Text.Trim()))
            //{
            //    MessageBox.Show("请输入发送内容");
            //    return;
            //}
            try
            {
                string result = HexConvert.Hex2Ten(tbContent.Text.Trim());
                //serialPort1.Write(result);
                byte[] bData = new byte[3];
                //bData[0] = 0x16;
                //bData[1] = 0x54;
                //bData[2] = 0x0D;

                bData[0] = 22;
                bData[1] =84;
                bData[2] =13;

                serialPort1.Write(bData,0,3);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
           // string result = HexConvert.Hex2Ten(tbContent.Text.Trim());

         

         //   byte[] bb = strToHexByte(tbContent.Text.Trim());
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            this.btnClose.Enabled = false;
            this.btnOpen.Enabled = true;
        }

        private static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private bool isone = false;
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                while (true)
                {
                    if (isone)
                    {
                        return;
                    }
                    Console.WriteLine("11");
                    System.Threading.Thread.Sleep(2000);
                }

                Console.WriteLine("1");
            });

       
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isone = true;
        }

    }
}
