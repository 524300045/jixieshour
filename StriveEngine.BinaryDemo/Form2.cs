using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StriveEngine.Tcp.Passive;
using StriveEngine.BinaryDemoCore;

namespace StriveEngine.BinaryDemo
{
    public partial class Form2 : Form
    {
        private ITcpPassiveEngine tcpPassiveEngine;





        public Form2()
        {
            InitializeComponent();
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //初始化并启动客户端引擎（TCP、文本协议）
                this.tcpPassiveEngine = NetworkEngineFactory.CreateStreamTcpPassivEngine(this.textBox_IP.Text, int.Parse(this.textBox_port.Text), new StreamContractHelper());
                this.tcpPassiveEngine.MessageReceived += new CbDelegate<System.Net.IPEndPoint, byte[]>(tcpPassiveEngine_MessageReceived);
                this.tcpPassiveEngine.AutoReconnect = true;//启动掉线自动重连                
                this.tcpPassiveEngine.ConnectionInterrupted += new CbDelegate(tcpPassiveEngine_ConnectionInterrupted);
                this.tcpPassiveEngine.ConnectionRebuildSucceed += new CbDelegate(tcpPassiveEngine_ConnectionRebuildSucceed);
                this.tcpPassiveEngine.Initialize();

                this.textBox_IP.ReadOnly = true;
                this.textBox_port.ReadOnly = true;
                this.button3.Enabled = false;
                     
                MessageBox.Show("连接成功！");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }


        void tcpPassiveEngine_ConnectionRebuildSucceed()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate(this.tcpPassiveEngine_ConnectionInterrupted));
            }
            else
            {
               // this.button1.Enabled = true;
                MessageBox.Show("重连成功。");
            }
        }

        void tcpPassiveEngine_ConnectionInterrupted()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate(this.tcpPassiveEngine_ConnectionInterrupted));
            }
            else
            {
               // this.button1.Enabled = false;
                MessageBox.Show("您已经掉线。");
            }
        }

        void tcpPassiveEngine_MessageReceived(System.Net.IPEndPoint serverIPE, byte[] bMsg)
        {
            //获取消息类型
            int msgType = BitConverter.ToInt32(bMsg, 4);//消息类型是 从offset=4处开始 的一个整数

          
            if (msgType == NewMessageType.SeverReceiveResult)
            {
                #region 收到服务端返回的收到结果请求

                string result = "客户端收到服务端返回结果" ;
                this.ShowResult(result);


                #endregion
            }

            if (msgType==NewMessageType.ServerPlaceDevice)
            {
                //收到服务端已经放置好设备
                #region 服务端收到设备已放置好
                
               
                MsgResponseContract response = (MsgResponseContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);
                string result = "收到服务端条码:" + response.Msg;
                this.ShowResult(result);

                //客户端发送给服务端收到请求

                result = "发送收到请求给服务端";
                this.ShowResult(result);
                msgType = MessageType.ClientSendReceive;
                MsgRequestContract contract = new MsgRequestContract("", "");
                contract.Key = Guid.NewGuid().ToString();
                byte[] bBody = SerializeHelper.SerializeObject(contract);

                //消息头
                MessageHead head = new MessageHead(bBody.Length, msgType);
                byte[] bHead = head.ToStream();

                //构建请求消息
                byte[] reqMessage = new byte[bHead.Length + bBody.Length];
                Buffer.BlockCopy(bHead, 0, reqMessage, 0, bHead.Length);
                Buffer.BlockCopy(bBody, 0, reqMessage, bHead.Length, bBody.Length);

                //发送请求消息
                this.tcpPassiveEngine.PostMessageToServer(reqMessage);

                #endregion

            }

            if (msgType == NewMessageType.ServerTakeDevice)
            { 
                //服务区已取走设备，不放置新设备
            }

            if (msgType == NewMessageType.ServerTakeDeviceAndPlaceDevice)
            {
                //服务区取走，并放置新设备

            }
            
        }

        private void ShowResult(string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<string>(this.ShowResult), result);
            }
            else
            {
               // this.label_result.Text = result;
                rtbInfo.AppendText(result+"\r\n");
            }
        }


        /// <summary>
        /// 发送ready命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReady_Click(object sender, EventArgs e)
        {
            string code =tbCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("请输入编码");
                return;
            }
            ShowResult("发送Ready请求");
            //客户端发送ready命令
            int msgType = NewMessageType.ClientSendReady;
            MsgRequestContract contract = new MsgRequestContract("", "");
            contract.Key = Guid.NewGuid().ToString();
            contract.DeviceCode = code;//设备编码

            byte[] bBody = SerializeHelper.SerializeObject(contract);

            //消息头
            MessageHead head = new MessageHead(bBody.Length, msgType);
            byte[] bHead = head.ToStream();

            //构建请求消息
            byte[] reqMessage = new byte[bHead.Length + bBody.Length];
            Buffer.BlockCopy(bHead, 0, reqMessage, 0, bHead.Length);
            Buffer.BlockCopy(bBody, 0, reqMessage, bHead.Length, bBody.Length);

            //发送请求消息
            this.tcpPassiveEngine.PostMessageToServer(reqMessage);

        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            #region 客户端发送结果

            ShowResult("发送结果");
            //发送ready
            int msgType = NewMessageType.ClientSendResult;
            MsgRequestContract contract = new MsgRequestContract("", "");
            contract.Key = Guid.NewGuid().ToString();
            byte[] bBody = SerializeHelper.SerializeObject(contract);

            //消息头
            MessageHead head = new MessageHead(bBody.Length, msgType);
            byte[] bHead = head.ToStream();

            //构建请求消息
            byte[] reqMessage = new byte[bHead.Length + bBody.Length];
            Buffer.BlockCopy(bHead, 0, reqMessage, 0, bHead.Length);
            Buffer.BlockCopy(bBody, 0, reqMessage, bHead.Length, bBody.Length);

            //发送请求消息
            this.tcpPassiveEngine.PostMessageToServer(reqMessage);

            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ShowResult("发送Ready请求");
            //发送ready
            int msgType = MessageType.ClientSendReady;
            MsgRequestContract contract = new MsgRequestContract("", "MC9K-TR-FCT-1,M2MB999999990,1,M009287,MC9K-TR,,OK,");
            contract.Key = Guid.NewGuid().ToString();
            byte[] bBody = SerializeHelper.SerializeObject(contract);

            //消息头
            MessageHead head = new MessageHead(bBody.Length, msgType);
            byte[] bHead = head.ToStream();

            //构建请求消息
            byte[] reqMessage = new byte[bHead.Length + bBody.Length];
            Buffer.BlockCopy(bHead, 0, reqMessage, 0, bHead.Length);
            Buffer.BlockCopy(bBody, 0, reqMessage, bHead.Length, bBody.Length);

            //发送请求消息
            

        }

        private void btnReadOne_Click(object sender, EventArgs e)
        {

            //发送readdy请求

            string code = tbCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("请输入编码");
                return;
            }
            ShowResult("发送Ready请求");
            //客户端发送ready命令
            int msgType = NewMessageType.ClientSendReady;
            MsgRequestContract contract = new MsgRequestContract("", "");
            contract.Key = Guid.NewGuid().ToString();
            contract.DeviceCode = code;//设备编码

            byte[] bBody = SerializeHelper.SerializeObject(contract);

            //消息头
            MessageHead head = new MessageHead(bBody.Length, msgType);
            byte[] bHead = head.ToStream();

            //构建请求消息
            byte[] reqMessage = new byte[bHead.Length + bBody.Length];
            Buffer.BlockCopy(bHead, 0, reqMessage, 0, bHead.Length);
            Buffer.BlockCopy(bBody, 0, reqMessage, bHead.Length, bBody.Length);

            //发送请求消息
            this.tcpPassiveEngine.PostMessageToServer(reqMessage);
        }

        
    }
}
