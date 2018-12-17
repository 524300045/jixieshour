using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StriveEngine;
using StriveEngine.Core;
using StriveEngine.Tcp.Server;
using System.Net;
using StriveEngine.BinaryDemoCore;
using System.Linq;
using StriveEngine.BinaryDemoServer.Dal;
using StriveEngine.BinaryDemoServer.Model;
using System.Timers;
using System.IO.Ports;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PdaTester;
using System.Threading;
using FwCore.Tasks;
using Atms.Common;
using Atms.Common.DevicePos;
namespace StriveEngine.BinaryDemoServer
{

    public partial class Form2 : Form
    {
        /// <summary>
        /// LOG日志队列
        /// </summary>
        public  ConcurrentQueue<ClientLogModel> logCQ = new ConcurrentQueue<ClientLogModel>();

  
        private ITcpServerEngine tcpServerEngine;
        private bool hasTcpServerEngineInitialized;


        List<IOInfo> ioInfoList = new List<IOInfo>();
        List<FctPos> fctPosList = new List<FctPos>();

       HomeControl homeControl;

        //MES接口
        string messerver = ConfigurationManager.AppSettings["mesip"];
        string mesport = ConfigurationManager.AppSettings["mesport"];


        UcFlow u1;
      
        public delegate void Displaydelegate(byte[] InputBuf);
        public Displaydelegate disp_delegate;

        public Form2()
        {
            InitializeComponent();

            disp_delegate = new Displaydelegate(DispUI);
           

        }


        #region 定时器

        /// <summary>
        /// 程序执行时间测试
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(秒)单位，比如: 0.00239秒</returns>
        public  int ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            //你想转的格式
            return ts3.Minutes*60+ts3.Seconds;
        }

       

        #endregion

        #region 客户端监听界面
        
     
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
           
                if (this.tcpServerEngine == null)
                {
                    this.tcpServerEngine = NetworkEngineFactory.CreateStreamTcpServerEngine(int.Parse(this.textBox_port.Text), new  StreamContractHelper());
                }
              
                if (this.hasTcpServerEngineInitialized)
                {
                    this.tcpServerEngine.ChangeListenerState(true);
                }
                else
                {
                    this.InitializeTcpServerEngine();
                }

                this.ShowListenStatus();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                LogHelper.Log("启动:"+ee.Message);
            }
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_StopListen_Click(object sender, EventArgs e)
        {
            if (!this.tcpServerEngine.IsListening)
            {
                return;
            }
            this.tcpServerEngine.ChangeListenerState(false);
            this.ShowListenStatus();
        }

        /// <summary>
        /// 关闭监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Close_Click(object sender, EventArgs e)
        {
            if (tcpServerEngine!=null)
            {
                this.tcpServerEngine.CloseAllClient();
                this.tcpServerEngine.Dispose();
                this.hasTcpServerEngineInitialized = false;
                this.ShowListenStatus();
                this.textBox_port.ReadOnly = false;
                this.textBox_port.SelectAll();
                this.textBox_port.Focus();
                this.tcpServerEngine = null;
            }
        
        }

        #endregion

        #region tcpip
        

        private void InitializeTcpServerEngine()
        {
            this.tcpServerEngine.ClientCountChanged += new CbDelegate<int>(tcpServerEngine_ClientCountChanged);
            this.tcpServerEngine.ClientConnected += new CbDelegate<System.Net.IPEndPoint>(tcpServerEngine_ClientConnected);
            this.tcpServerEngine.ClientDisconnected += new CbDelegate<System.Net.IPEndPoint>(tcpServerEngine_ClientDisconnected);
            this.tcpServerEngine.MessageReceived += new CbDelegate<IPEndPoint, byte[]>(tcpServerEngine_MessageReceived);
            this.tcpServerEngine.Initialize();
            this.hasTcpServerEngineInitialized = true;
        }

        void tcpServerEngine_MessageReceived(IPEndPoint client, byte[] bMsg)
        {
            //获取消息类型
            int msgType = BitConverter.ToInt32(bMsg, 4);//消息类型是 从offset=4处开始 的一个整数

            if (msgType == NewMessageType.ClientSendReady)
            {
                #region 客服端发送ready请求，服务端收到ready请求

                //改变客户端状态
                var clientResult = SystemInfo.ClientInfoList.Where(p => p.Ip == client.Address.ToString());
              
                foreach (var item in SystemInfo.ClientInfoList)
                {
                    if (item.Ip==client.Address.ToString())
                    {
                        item.State = 1;
                    }
                }
             
                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);



                ShowMsghomeControl("收到" + client.Address.ToString() + "ClientSendReady的请求");
           
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = request.Key;
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ReceiveClientReady);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

                //发送回复消息
                this.tcpServerEngine.PostMessageToClient(client, resMessage);

                ShowMsghomeControl("向客户端回复" + client.Address.ToString() + "ReceiveClientReady的请求");


                #endregion
            }

            if (msgType==NewMessageType.ClientSendTestFinished)
            {
                #region 客户端发送测试完成

                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);

         
                ShowMsghomeControl("收到" + client.Address.ToString() + "ClientSendTestFinished的请求," + request.Msg);

                foreach (var item in SystemInfo.ClientInfoList)
                {
                    if (item.Ip == client.Address.ToString())
                    {
                        item.State =3;
                    }
                }

                ClientRquestInfo requestInfo = new ClientRquestInfo();
                requestInfo.endPoint = client;
                requestInfo.messageType = msgType;
                SystemInfo.clientRequestCQ.Enqueue(requestInfo);

                //服务端收到客户端测试完成请求
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = request.Key;
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerReceiveTestFinished);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

                //发送回复消息
                this.tcpServerEngine.PostMessageToClient(client, resMessage);

                   MessageResult messageResult = new MessageResult();
                if (request.Msg.Split(',')[1].ToUpper() == "PASS")
                {
                    //放置到good区
                    messageResult.BarCode = request.Msg.Split(',')[0];
                    messageResult.Code = request.Msg.Split(',')[1].ToUpper();
                }
                else
                {
                    //放置到Ng区
                    messageResult.BarCode = request.Msg.Split(',')[0];
                    messageResult.Code = request.Msg.Split(',')[1].ToUpper();
                }

                var clientResult = SystemInfo.ClientInfoList.Where(p => p.Ip == client.Address.ToString());
                if (clientResult!=null&&clientResult.FirstOrDefault()!=null)
                {
                    clientResult.FirstOrDefault().MsgResult = messageResult;
                }

                ShowMsghomeControl("向客户端回复" + client.Address.ToString() + "ServerReceiveTestFinished的请求");

             

                #endregion
            }

            if (msgType == NewMessageType.ClientSendResult)
            {
                #region 服务端收到客户端发送结果

                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);

                ShowMsghomeControl("收到" + client.Address.ToString() + "ClientSendResult的请求," + request.Msg);
                ShowMsghomeControl("收到" + client.Address.ToString() + "ClientSendResult的请求," + request.Msg);
                //回复消息体
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = request.Key;
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.SeverReceiveResult);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

                //发送回复消息
                this.tcpServerEngine.PostMessageToClient(client, resMessage);

               // ShowMsg("收到客户端测试结果:ClientSendResult:" + request.Msg);

                MessageResult messageResult = new MessageResult();
                if (request.Msg.Split(',')[1].ToUpper() == "PASS")
                {
                    //放置到good区
                    messageResult.BarCode = request.Msg.Split(',')[0];
                    messageResult.Code = request.Msg.Split(',')[1].ToUpper();
                }
                else
                {
                    //放置到Ng区
                    messageResult.BarCode = request.Msg.Split(',')[0];
                    messageResult.Code = request.Msg.Split(',')[1].ToUpper();
                }

                var clientResult = SystemInfo.ClientInfoList.Where(p => p.Ip == client.Address.ToString());
                if (clientResult != null && clientResult.FirstOrDefault() != null)
                {
                    clientResult.FirstOrDefault().MsgResult = messageResult;
                }

                ShowMsghomeControl("向客户端回复:" + client.Address.ToString() + "SeverReceiveResult的请求,");
                #endregion
            }


        }

        void tcpServerEngine_ClientDisconnected(System.Net.IPEndPoint ipe)
        {
            string msg = "下线";
            this.ShowEvent(msg,ipe);
        }

        /// <summary>
        /// 客户端连接处理
        /// </summary>
        /// <param name="ipe"></param>
        void tcpServerEngine_ClientConnected(System.Net.IPEndPoint ipe)
        {
            
            string msg = "上线";
            this.ShowEvent(msg,ipe);
        }        

        void tcpServerEngine_ClientCountChanged(int count)
        {
            this.ShowConnectionCount(count);
        }

        #endregion

        #region 公用方法


        private void ShowListenStatus()
        {
            this.button_StartListen.Enabled = this.tcpServerEngine.IsListening ? false : true;
            this.button_StopListen.Enabled = this.tcpServerEngine.IsListening ? true : false;
            this.button_Close.Enabled = this.tcpServerEngine.IsListening ? true : false;
            this.textBox_port.ReadOnly = true;
            this.toolStripLabel_Port.Text = this.tcpServerEngine.IsListening ? this.tcpServerEngine.Port.ToString() : null;
        }

        private void ShowEvent(string msg, System.Net.IPEndPoint ipe)
        {
            var curResult = AppInfo.ClientInfoList.Where(p => p.IP == ipe.Address.ToString());
            if (curResult==null||curResult.FirstOrDefault()==null)
            {

                return;
            }
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<string,System.Net.IPEndPoint>(this.ShowEvent), msg,ipe);
            }
            else
            {
                if (msg=="上线")
                {
                    #region 上线处理

                    var result = SystemInfo.ClientInfoList.Where(p => p.Ip == ipe.Address.ToString());
                    if (result == null || result.FirstOrDefault() == null)
                    {
                        ShowMsghomeControl("设备" + ipe.Address.ToString() + "上线");
                        ClientInfo info = new ClientInfo();
                        info.IpPoint = ipe;
                        info.StateDes = "在线";
                        info.IsOnLine = 1;
                        if (curResult!=null)
                        {
                            info.Code = curResult.FirstOrDefault().Code;
                            info.Name = curResult.FirstOrDefault().Name;
                        }
                        info.Ip = ipe.Address.ToString();
                        info.FctPos = SystemInfo.FctPosList.Where(p => p.IP == ipe.Address.ToString()).FirstOrDefault();
                        SystemInfo.ClientInfoList.Add(info);
                    }
                    else
                    {
                        
                        ShowMsghomeControl("设备" + ipe.Address.ToString() + "上线....");
                        result.FirstOrDefault().IsOnLine = 1;
                        result.FirstOrDefault().IpPoint = ipe;
                        result.FirstOrDefault().StateDes = "在线";
                        result.FirstOrDefault().Ip = ipe.Address.ToString();
                        result.FirstOrDefault().FctPos= SystemInfo.FctPosList.Where(p => p.IP == ipe.Address.ToString()).FirstOrDefault();
                    }

                    #endregion
                }
                if (msg=="下线")
                {
                    #region 下线处理

                    ShowMsghomeControl("设备" + ipe.Address.ToString() + "下线....");
                    var result = SystemInfo.ClientInfoList.Where(p => p.Ip == ipe.Address.ToString());
                    if (result != null && result.FirstOrDefault()!=null)
                    {
                        result.FirstOrDefault().State = 0;
                        result.FirstOrDefault().StateDes = "下线";
                        result.FirstOrDefault().IsOnLine = 0;
                        result.FirstOrDefault().IpPoint = ipe;
                    }
                    #endregion
                }
                this.toolStripLabel_event.Text = ipe.ToString()+msg;

            }
        }

        public void InsertLog(ClientLogModel model)
        {
            try
            {
                var curInfo = AppInfo.ClientInfoList.Where(p => p.IP == model.IP);
                if (curInfo != null && curInfo.FirstOrDefault() != null)
                {
                    model.Name = curInfo.FirstOrDefault().Name;
                    model.Code = curInfo.FirstOrDefault().Code;
                }

                logCQ.Enqueue(model);

                // new ClientLogDal().Insert(model);
            }
            catch (Exception ex)
            {
                LogHelper.Log("InsertLog" + ex.Message);
            }
        }

    
        private void ShowConnectionCount(int clientCount)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<int>(this.ShowConnectionCount), clientCount);
            }
            else
            {
                this.toolStripLabel_clientCount.Text = "在线数量： " + clientCount.ToString();
            }
        }

        #endregion

        #region 设备管理

        private void btnAddIp_Click(object sender, EventArgs e)
        {
            AddIPForm form = new AddIPForm();
            if (form.ShowDialog()==DialogResult.OK)
            {
                BindClientIPInfo();
            }
        }

        public void BindClientIPInfo()
        {
          DataTable dt=new ClientInfoDal().GetDS();
          dgvClientInfo.DataSource = dt;
        }

        private void dgvClientInfo_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex>=0)
            {
                if (e.ColumnIndex==7)
                {
                    int id = Convert.ToInt32(this.dgvClientInfo.CurrentRow.Cells["id"].Value.ToString());
                    EditIPForm form = new EditIPForm(id);
                    if (form.ShowDialog()==DialogResult.OK)
                    {
                        BindClientIPInfo();
                    }
                }
                if (e.ColumnIndex ==8)
                {
                    if (MessageBox.Show("确定要删除当前设备吗?","提示",MessageBoxButtons.OKCancel)==DialogResult.OK)
                    {
                        int id = Convert.ToInt32(this.dgvClientInfo.CurrentRow.Cells["id"].Value.ToString());
                        new ClientInfoDal().Delete(id);
                        BindClientIPInfo();
                    }
                }
            }
          
        }

        private void dgvClientInfo_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
             Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dgvClientInfo.RowHeadersWidth - 4,
                e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvClientInfo.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgvClientInfo.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }

        #endregion

        #region 日志管理
        
      
 



  
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvLog.AutoGenerateColumns = false;

            if (this.tabControl1.SelectedIndex==2)
            {
                dtBegin.Value = DateTime.Now;
                
            }
            if (this.tabControl1.SelectedIndex ==1)
            {
                this.dgvClientInfo.AutoGenerateColumns = false;
                BindClientIPInfo();
            }
        }

  
 

 

        #endregion

        #region 页面加载

        IoCard ioCard;
        public bool bRefresh = true;
        private void Form1_Load(object sender, EventArgs e)
        {
            //当前设备编码
            RobotInfo.DeviceCode = ConfigurationManager.AppSettings["devicecode"];
            this.Text = this.Text + RobotInfo.DeviceCode;//当前编码
            SystemInfo.ClientInfoList = new List<ClientInfo>();
            SystemInfo.clientRequestCQ=new  ConcurrentQueue<ClientRquestInfo>();
            List<ClientIpInfoModel> list = new List<ClientIpInfoModel>();
            DataTable dt = new ClientInfoDal().GetDS();
            if (dt!=null&&dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ClientIpInfoModel model = new ClientIpInfoModel();
                    model.Id =Convert.ToInt32(dt.Rows[i]["id"].ToString());
                    model.Code = dt.Rows[i]["code"].ToString();
                    model.Name = dt.Rows[i]["name"].ToString();
                    model.IP = dt.Rows[i]["ip"].ToString();
                    model.Port = dt.Rows[i]["port"].ToString();
                    model.Timeouts = Convert.ToInt32(dt.Rows[i]["timeouts"].ToString());
                    model.Status = Convert.ToInt32(dt.Rows[i]["status"].ToString());
                    list.Add(model);

                    ClientInfo info = new ClientInfo();
                    info.Code = dt.Rows[i]["code"].ToString();
                    info.Name = dt.Rows[i]["name"].ToString();
                    info.Ip = dt.Rows[i]["ip"].ToString();
                    SystemInfo.ClientInfoList.Add(info);
                
                }
            }
            AppInfo.ClientInfoList = list;

            LoadFctPos();
            LoadIOPos();
            SystemInfo.FctPosList = fctPosList;
            SystemInfo.IoInfoList = ioInfoList;


            try
            {
                button1_Click(null,null);
            }
            catch (Exception ex)
            {
                LogHelper.Log("服务端启动失败" + ex.Message);
                MessageBox.Show("服务端启动失败"+ex.Message);
                return;
            }


            u1 = new UcFlow();
            tabPage5.Controls.Add(u1);
            ioCard = RobotApp.IoCard;

             homeControl = new HomeControl();
             homeControl.Dock = DockStyle.Fill;
             panel2.Controls.Add(homeControl);

            TaskContinue.Start(delegate
            {
                if (!RobotApp.BRun)
                {
                    return false;
                }
                bool b = true;
                if (bRefresh)
                    b = ioCard.RefreshDI();
                return b;
      
            }, 100, "IoCard");
			
            u1.ConsoleMsgEvent+=u1_ConsoleMsgEvent;
            u1.SendMessageEvent+=u1_SendMessageEvent;
            u1.RobotStatusEvent +=u1_RobotStatusEvent;

            homeControl.SendMessageEvent += u1_SendMessageEvent;


            Task.Factory.StartNew(()=>{
          WriteLog();
        });
            //日志处理
          
        }

        private void u1_ConsoleMsgEvent(string msg)
        {
            ShowMsghomeControl(msg);
        }



        private string u1_SendMessageEvent(int type,string barCode,string ip)
        {
            if (SystemInfo.ClientInfoList.Count <= 0)
            {
                return "";
            }

            IPEndPoint client = SystemInfo.ClientInfoList.Where(p => p.Ip == ip).FirstOrDefault().IpPoint;
         //   ShowMsghomeControl("向client" + client.Address.ToString() + "发送:" + type + "--" + barCode);
         
            if (type==8)
            {
                #region 机械手放置设备

                ShowMsghomeControl("向client" + client.Address.ToString() + "发送放置:ServerPlaceDevice" + type + "--" + barCode);

                //回复消息体
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = "";
                response.Msg = barCode;
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerPlaceDevice);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

                //发送回复消息
                this.tcpServerEngine.PostMessageToClient(client, resMessage);

                foreach (var item in SystemInfo.ClientInfoList)
                {
                    if (item.Ip==client.Address.ToString())
                    {
                        item.State = 2;
                    }
                }

                #endregion
            }

            if (type==11)
            {
                #region 取走设备


                ShowMsghomeControl("向client" + client.Address.ToString() + "发送取走:ServerTakeDevice" + type + "--" + barCode);

                //回复消息体
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = "";
                response.Msg = "";
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerTakeDevice);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);
                //发送回复消息
                this.tcpServerEngine.SendMessageToClient(client, resMessage);

             

                #endregion
            }

            if (type == 12)
            {
                #region 取走设备

                ShowMsghomeControl("向client" + client.Address.ToString() + "发送取走并放置:ServerTakeDeviceAndPlaceDevice" + type + "--" + barCode);
                //回复消息体
                MsgResponseContract response = new MsgResponseContract("", "");
                response.Key = "";
                response.Msg = barCode;
                byte[] bReponse = SerializeHelper.SerializeObject(response);
                //回复消息头
                MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerTakeDeviceAndPlaceDevice);
                byte[] bHead = head.ToStream();

                //构建回复消息
                byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);
                //发送回复消息
                this.tcpServerEngine.SendMessageToClient(client, resMessage);



                #endregion
            }
           



            return "";
        
        }


        private void u1_RobotStatusEvent(int status)
        {
            RobotInfo.WorkStatus = status;
        }
 

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
    
            button_Close_Click(null,null);

        }


        #endregion

        private void dgvLog_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y,
             dgvLog.RowHeadersWidth - 4,
             e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvLog.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgvLog.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

  

        #region 串口

        public void DispUI(byte[] InputBuf)
        {
            //textBox1.Text = Convert.ToString(InputBuf);

            ASCIIEncoding encoding = new ASCIIEncoding();
            string info= encoding.GetString(InputBuf);
            LogHelper.Log("串口读取条码:" + info);
          //  richTextBox1.Text = encoding.GetString(InputBuf);
            rtbInfo.Text += info+"\r\n";

            string paraInfo = "" ;
            if (!string.IsNullOrEmpty(info))
            {
                #region 调取MES接口
                new MesDaalService().Connect(messerver, int.Parse(mesport), paraInfo);
                #endregion
            }
            
        }
        #endregion
      


        #region 日志消息队列处理

        private void WriteLog()
        {
            try
            {
                while (true)
                {
                    if (logCQ.Count > 0)
                    {
                        ClientLogModel model = new ClientLogModel();
                        if (logCQ.TryDequeue(out model))
                        {
                            new ClientLogDal().Insert(model);
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message);
            }
        
        }

        #endregion


        #region 抓走操作实际
        
      
        private void button1_Click_2(object sender, EventArgs e)
        {
            if (lvIps.SelectedItems.Count == 0) return;

            string ipInfo = lvIps.SelectedItems[0].SubItems[2].Text;

            string ip = ipInfo.Split(':')[0];
            string curPort = ipInfo.Split(':')[1];

            IPEndPoint client = new IPEndPoint(IPAddress.Parse(ip), int.Parse(curPort)); ;
            ClientSendInfo sendInfo = new ClientSendInfo();
            sendInfo.IP = ip;
            sendInfo.SendBarCodeTime = DateTime.Now;
            sendInfo.IpPoint = client;
            //var curDevInfoResult = AppInfo.ClientInfoList.Where(p => p.IP == sendInfo.IP);
            //if (curDevInfoResult != null && curDevInfoResult.FirstOrDefault() != null)
            //{
            //    sendInfo.Name = curDevInfoResult.FirstOrDefault().Name;
            //    sendInfo.Code = curDevInfoResult.FirstOrDefault().Code;
            //}
            //sendInfo.BarCode = "12345678";
            //clientSendList.Add(sendInfo);

            //回复消息体
            MsgResponseContract response = new MsgResponseContract("", "");
            response.Key = "";
            response.Msg = "12345678";
            byte[] bReponse = SerializeHelper.SerializeObject(response);
            //回复消息头
            MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerPlaceDevice);
            byte[] bHead = head.ToStream();

            //构建回复消息
            byte[] resMessage = new byte[bHead.Length + bReponse.Length];
            Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
            Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

            //发送回复消息
            this.tcpServerEngine.PostMessageToClient(client, resMessage);
     
        }

        /// <summary>
        /// 抓走
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZhuaZou_Click(object sender, EventArgs e)
        {
            if (lvIps.SelectedItems.Count == 0) return;

            string ipInfo = lvIps.SelectedItems[0].SubItems[2].Text;

            string ip = ipInfo.Split(':')[0];
            string curPort = ipInfo.Split(':')[1];

            IPEndPoint client = new IPEndPoint(IPAddress.Parse(ip), int.Parse(curPort)); ;
            ClientSendInfo sendInfo = new ClientSendInfo();
            sendInfo.IP = ip;
            sendInfo.SendBarCodeTime = DateTime.Now;
            sendInfo.IpPoint = client;
          

            //回复消息体
            MsgResponseContract response = new MsgResponseContract("", "");
            response.Key = "";
            response.Msg = "";
            byte[] bReponse = SerializeHelper.SerializeObject(response);
            //回复消息头
            MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerTakeDevice);
            byte[] bHead = head.ToStream();

            //构建回复消息
            byte[] resMessage = new byte[bHead.Length + bReponse.Length];
            Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
            Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

            //发送回复消息
            this.tcpServerEngine.PostMessageToClient(client, resMessage);
        }

        /// <summary>
        /// 抓走并放置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZhuanAndPlace_Click(object sender, EventArgs e)
        {

            if (lvIps.SelectedItems.Count == 0) return;

            string ipInfo = lvIps.SelectedItems[0].SubItems[2].Text;

            string ip = ipInfo.Split(':')[0];
            string curPort = ipInfo.Split(':')[1];

            IPEndPoint client = new IPEndPoint(IPAddress.Parse(ip), int.Parse(curPort)); ;
            ClientSendInfo sendInfo = new ClientSendInfo();
            sendInfo.IP = ip;
            sendInfo.SendBarCodeTime = DateTime.Now;
            sendInfo.IpPoint = client;


            //回复消息体
            MsgResponseContract response = new MsgResponseContract("", "");
            response.Key = "";
            response.Msg = "";
            byte[] bReponse = SerializeHelper.SerializeObject(response);
            //回复消息头
            MessageHead head = new MessageHead(bReponse.Length, NewMessageType.ServerTakeDeviceAndPlaceDevice);
            byte[] bHead = head.ToStream();

            //构建回复消息
            byte[] resMessage = new byte[bHead.Length + bReponse.Length];
            Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
            Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

            //发送回复消息
            this.tcpServerEngine.PostMessageToClient(client, resMessage);
        }

        #endregion


  

        private void ShowMsghomeControl(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<string>(this.ShowMsghomeControl), msg);
            }
            else
            {
                homeControl.rtbInfo.AppendText(DateTime.Now.ToString()+":"+msg + "\r\n");
                LogHelper.Log(DateTime.Now.ToString() + ":" + msg);
                Application.DoEvents();
            }
        }


        /// <summary>
        /// 加载位置信息
        /// </summary>
        private void LoadIOPos()
        { 
            //入料去区
           

            #region 加载入料区信息
            

            //加载1入料区
            IOInfo infoOne = new IOInfo();
            infoOne.AreaCode = "LIN";
            infoOne.Code = "1";
            //位置
            IOPos  ioposOne=new IOPos();
            ioposOne.LowPos = "X=284.275,Y=495.800,Z=323.899,U=179.721,V=-0.599,W=-179.446";//低端位置
            ioposOne.HighPos="";//高端位置
            infoOne.IoPosition = ioposOne;
            infoOne.IoFlag = 1;
            infoOne.i =2;
            infoOne.j =0;
            ioInfoList.Add(infoOne);

 




            //加载2
            IOInfo infoTwo = new IOInfo();
            infoTwo.AreaCode = "LIN";
            infoTwo.Code = "2";
            //位置
            IOPos ioposTwo = new IOPos();
            ioposTwo.LowPos = "X=158.804,Y=495.800,Z=323.344,U=179.721,V=-0.599,W=-179.446";//低端位置
            ioposTwo.HighPos = "";//高端位置
            infoTwo.IoPosition = ioposTwo;
            infoTwo.IoFlag = 1;

            infoTwo.i = 1;
            infoTwo.j = 3;

            ioInfoList.Add(infoTwo);

            //加载3
            IOInfo infoThree= new IOInfo();
            infoThree.AreaCode = "LIN";
            infoThree.Code = "3";
            //位置
            IOPos ioposThree = new IOPos();
            ioposThree.LowPos = "X=157.809,Y=722.095,Z=328.533,U=179.721,V=-0.600,W=-179.446";//低端位置
            ioposThree.HighPos = "";//高端位置
            infoThree.IoPosition = ioposThree;
            infoThree.IoFlag = 1;

            infoThree.i = 1;
            infoThree.j = 2;

            ioInfoList.Add(infoThree);


            //加载4
            IOInfo infoFour = new IOInfo();
            infoFour.AreaCode = "LIN";
            infoFour.Code = "4";
            //位置
            IOPos ioposFour = new IOPos();
            ioposFour.LowPos = "X=282.637,Y=723.612,Z=329.961,U=179.720,V=-0.600,W=-179.446";//低端位置
            ioposFour.HighPos = "";//高端位置
            infoFour.IoPosition = ioposFour;
            infoFour.IoFlag = 1;

            infoFour.i = 1;
            infoFour.j = 0;

            ioInfoList.Add(infoFour);

            #endregion

            #region 加载1A区信息
            //加载1A第一个格子信息
            IOInfo info1AOne = new IOInfo();
            info1AOne.AreaCode = "1A";
            info1AOne.Code = "1";
            //位置
            IOPos iopos1AOne = new IOPos();
            iopos1AOne.LowPos = "X=516.601,Y=55.497,Z=398.994,U=90.857,V=0.247,W=-178.960";//低端位置
            iopos1AOne.HighPos = "X=516.601,Y=55.497,Z=398.994,U=90.857,V=0.247,W=-178.960";//高端位置
            info1AOne.IoPosition = iopos1AOne;

            info1AOne.i = 1;
            info1AOne.j = 0;


            ioInfoList.Add(info1AOne);

            //加载1ATWO信息
            IOInfo info1ATwo= new IOInfo();
            info1ATwo.AreaCode = "1A";
            info1ATwo.Code = "2";
            //位置
            IOPos iopos1ATwo = new IOPos();
            iopos1ATwo.LowPos = "X=516.601,Y=-70.497,Z=398.994,U=90.857,V=0.247,W=-178.960";//低端位置
            iopos1ATwo.HighPos = "X=516.601,Y=-70.497,Z=398.994,U=90.857,V=0.247,W=-178.960";//高端位置
            info1ATwo.IoPosition = iopos1ATwo;
            info1ATwo.i = 1;
            info1ATwo.j =1;

            ioInfoList.Add(info1ATwo);


            #endregion

            #region 加载2A区信息

            IOInfo info2AOne = new IOInfo();
            info2AOne.AreaCode = "2A";
            info2AOne.Code = "1";
            //位置
            IOPos iopos2AOne = new IOPos();
            iopos2AOne.LowPos = "X=505.032,Y=84.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//低端位置
            iopos2AOne.HighPos = "X=505.032,Y=84.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//高端位置
            info2AOne.IoPosition = iopos2AOne;

            info2AOne.i = 0;
            info2AOne.j = 0;

            ioInfoList.Add(info2AOne);


            IOInfo info2ATwo = new IOInfo();
            info2ATwo.AreaCode = "2A";
            info2ATwo.Code = "2";
            //位置
            IOPos iopos2ATwo = new IOPos();
            iopos2ATwo.LowPos = "X=505.032,Y=-41.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//低端位置
            iopos2ATwo.HighPos = "X=505.032,Y=-41.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//高端位置
            info2ATwo.IoPosition = iopos2ATwo;
            info2ATwo.i = 3;
            info2ATwo.j =4;

            ioInfoList.Add(info2ATwo);


            IOInfo info2AThree= new IOInfo();
            info2AThree.AreaCode = "2A";
            info2AThree.Code = "3";
            //位置
            IOPos iopos2AThree = new IOPos();
            iopos2AThree.LowPos = "X=505.032,Y=-166.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//低端位置
            iopos2AThree.HighPos = "X=505.032,Y=-166.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//高端位置
            info2AThree.IoPosition = iopos2AThree;

            info2AThree.i = 0;
            info2AThree.j = 2;

            ioInfoList.Add(info2AThree);


            IOInfo info2AFour = new IOInfo();
            info2AFour.AreaCode = "2A";
            info2AFour.Code = "4";
            //位置
            IOPos iopos2AFour= new IOPos();
            iopos2AFour.LowPos = "X=505.032,Y=-291.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//低端位置
            iopos2AFour.HighPos = "X=505.032,Y=-291.099,Z=257.127,U=91.143,V=0.247,W=-178.960";//高端位置
            info2AFour.IoPosition = iopos2AFour;
            info2AFour.i = 0;
            info2AFour.j = 3;

            ioInfoList.Add(info2AFour);

            #endregion

            #region 加载出料区信息

            IOInfo infoOutOne = new IOInfo();
            infoOutOne.AreaCode = "LOUT";
            infoOutOne.Code = "1";
            //位置
            IOPos ioposoutOne = new IOPos();
            ioposoutOne.LowPos = "X=292.560,Y=-725.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//低端位置
            ioposoutOne.HighPos = "X=292.560,Y=-725.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//高端位置
            infoOutOne.IoPosition = ioposoutOne;

            infoOutOne.i = 4;
            infoOutOne.j = 0;

            ioInfoList.Add(infoOutOne);

            IOInfo infoOutTwo= new IOInfo();
            infoOutTwo.AreaCode = "LOUT";
            infoOutTwo.Code = "2";
            //位置
            IOPos ioposoutTwo = new IOPos();
            ioposoutTwo.LowPos = "X=167.560,Y=-725.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//低端位置
            ioposoutTwo.HighPos = "X=167.560,Y=-725.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//高端位置
            infoOutTwo.IoPosition = ioposoutTwo;

            infoOutTwo.i = 3;
            infoOutTwo.j = 3;

            ioInfoList.Add(infoOutTwo);


            IOInfo infoOutThree = new IOInfo();
            infoOutThree.AreaCode = "LOUT";
            infoOutThree.Code = "3";
            //位置
            IOPos ioposoutThree = new IOPos();
            ioposoutThree.LowPos = "X=167.560,Y=-499.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//低端位置
            ioposoutThree.HighPos = "X=167.560,Y=-499.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//高端位置
            infoOutThree.IoPosition = ioposoutThree;

            infoOutThree.i = 4;
            infoOutThree.j = 2;

            ioInfoList.Add(infoOutThree);

            IOInfo infoOutFour = new IOInfo();
            infoOutFour.AreaCode = "LOUT";
            infoOutFour.Code = "4";
            //位置
            IOPos ioposoutFour = new IOPos();
            ioposoutFour.LowPos = "X=292.560,Y=-499.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//低端位置
            ioposoutFour.HighPos = "X=292.560,Y=-499.862,Z=334.467,U=1.469,V=0.525,W=-179.598";//高端位置
            infoOutFour.IoPosition = ioposoutFour;

            infoOutFour.i = 4;
            infoOutFour.j = 3;


            ioInfoList.Add(infoOutFour);



            #endregion

        }

        private void LoadFctPos()
        {


            ScanerPos.Pos = "X=23.770,Y=496.962,Z=321.792,U=-179.877,V=-0.471,W=-179.230";
            ScanerPos.MidPos = "X=23.770,Y=496.962,Z=400.792,U=-179.877,V=-0.471,W=-179.230";
            ScanerPos.TakePos = "X=23.770,Y=496.962,Z=321.792,U=-179.877,V=-0.471,W=-179.230";

            FctPos posOne = new FctPos();
            posOne.IP = "10.5.32.103";
            posOne.LowPos = "X=745.216,Y=15.529,Z=358.381,U=-0.132,V=0.364,W=-179.510";
            posOne.HighPos = "X=745.216,Y=15.529,Z=358.381,U=-0.132,V=0.364,W=-179.510";
            posOne.Pos = 0;
            fctPosList.Add(posOne);


            FctPos posTwo = new FctPos();
            posTwo.IP = "10.5.32.202";
            posTwo.LowPos = "X=759.425,Y=-198.176,Z=351.109,U=-0.463,V=-0.888,W=-179.926";
            posTwo.HighPos = "X=759.425,Y=-198.176,Z=351.109,U=-0.463,V=-0.888,W=-179.926";
            posTwo.Pos = 1000000;
            fctPosList.Add(posTwo);
         
        }


       
    }


  

}
