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

        private List<ClientInfo> clientInfos = new List<ClientInfo>();


        private List<ClientSendInfo> clientSendList = new List<ClientSendInfo>();

        private int pageIndex;

        private int pageSize=10;

        private int totalPage = 0;

        private int totalLogCount = 0;

        //MES接口
        string messerver = ConfigurationManager.AppSettings["mesip"];
        string mesport = ConfigurationManager.AppSettings["mesport"];


        private System.Timers.Timer _timerSendBarCode;

        private System.Timers.Timer _timerResult;


        SerialPort serialPort1 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);      //初始化串口设置
        public delegate void Displaydelegate(byte[] InputBuf);
        Byte[] OutputBuf = new Byte[128];
        public Displaydelegate disp_delegate;

        public Form2()
        {
            InitializeComponent();
            _timerSendBarCode = new System.Timers.Timer();
            _timerSendBarCode.Elapsed+=_timerSendBarCode_Tick;
            _timerSendBarCode.Enabled = true;
            _timerSendBarCode.Interval = 1000;

            _timerResult = new System.Timers.Timer();
            _timerResult.Elapsed+=_timerResult_Elapsed;
            _timerResult.Enabled = true;
            _timerResult.Interval = 5000;

            //显示
            disp_delegate = new Displaydelegate(DispUI);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(Comm_DataReceived);

        }


        #region 定时器
        
        private void _timerResult_Elapsed(object sender, ElapsedEventArgs e)
        {

            try
            {
                var curList = clientSendList.Where(p=>p.ReceiveTime!=null).ToList();
                if (curList.Count > 0)
                {
                    foreach (var item in curList)
                    {
                        int second = ExecDateDiff(DateTime.Now, item.ReceiveTime.Value);
                        if (second>60)
                        {

                            ClientLogModel logModel = new ClientLogModel();
                            logModel.Content = "客户端返回结果超时,请检查客户端";
                            logModel.IP = item.IP;
                            InsertLog(logModel);
                            //等待结果超时
                            var curResult = clientSendList.Where(p => p.IP == item.IP);
                            if (curResult != null && curResult.FirstOrDefault() != null)
                            {
                                clientSendList.Remove(curResult.FirstOrDefault());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("_timerResult_Elapsed" + ex.Message);
            }
        }

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

        private void _timerSendBarCode_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                var curList = clientSendList.Where(p => p.BarCode!="").ToList();

                if (curList.Count > 0)
                {
                    foreach (var item in curList)
                    {
                      
                        int second = ExecDateDiff(DateTime.Now, item.SendBarCodeTime);
                        if (item.TotalSendCount>=3)
                        {
                          var curResult=clientSendList.Where(p=>p.IP==item.IP);
                          if (curResult!=null&&curResult.FirstOrDefault()!=null)
                          {
                              clientSendList.Remove(curResult.FirstOrDefault());
                          }
                       
                          ClientLogModel logModel = new ClientLogModel();
                          logModel.Content = "发送次数超超过3次，请检查客户端";
                          logModel.IP = item.IP;
                          InsertLog(logModel);

                          this.ShowClientMsg(item.IpPoint, logModel.Content);
                          continue;
                        }
                        if (second>5)
                        {
                            //重新发送条码
                            string  record = "重新发送条码"+item.BarCode+"到客户端";
                            this.ShowClientMsg(item.IpPoint, record);

                            ClientLogModel logModel = new ClientLogModel();
                            logModel.Content = record;
                            logModel.IP = item.IP;
                            InsertLog(logModel);

                            //回复消息体
                            MsgResponseContract response = new MsgResponseContract("", item.BarCode);
                           // response.Key = request.Key;
                            byte[] bReponse = SerializeHelper.SerializeObject(response);
                            //回复消息头
                            MessageHead head = new MessageHead(bReponse.Length, MessageType.ServerResponseBarCode);
                            byte[] bHead = head.ToStream();

                            //构建回复消息
                            byte[] resMessage = new byte[bHead.Length + bReponse.Length];
                            Buffer.BlockCopy(bHead, 0, resMessage, 0, bHead.Length);
                            Buffer.BlockCopy(bReponse, 0, resMessage, bHead.Length, bReponse.Length);

                            //发送回复消息
                            this.tcpServerEngine.PostMessageToClient(item.IpPoint, resMessage);

                            item.SendBarCodeTime = DateTime.Now;

                            item.TotalSendCount += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("_timerSendBarCode_Tick" + ex.Message);
            }
        }

        #endregion

        #region 客户端监听界面
        
     
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _timerSendBarCode.Start();
                _timerResult.Start();
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

                var curDevInfo = clientSendList.Where(p => p.IP == client.Address.ToString());
                if (curDevInfo != null && curDevInfo.FirstOrDefault() != null)
                {
                    clientSendList.Remove(curDevInfo.FirstOrDefault());
                }

                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);
                string record = "收到客户端" + request.DeviceCode + "ready请求:";
                this.ShowClientMsg(client, record);


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



                #endregion
            }

            if (msgType==NewMessageType.ClientSendTestFinished)
            {
                #region 客户端发送测试完成


                var curDevInfo = clientSendList.Where(p => p.IP == client.Address.ToString());
                if (curDevInfo != null && curDevInfo.FirstOrDefault() != null)
                {
                    clientSendList.Remove(curDevInfo.FirstOrDefault());
                }

                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);
                string record = "收到客户端" + request.DeviceCode + "测试完成请求:";
                this.ShowClientMsg(client, record);

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

                #endregion
            }

            if (msgType == NewMessageType.ClientSendResult)
            {
                #region 服务端收到客户端发送结果

                var curDevInfo = clientSendList.Where(p => p.IP == client.Address.ToString());
                if (curDevInfo != null && curDevInfo.FirstOrDefault() != null)
                {
                    clientSendList.Remove(curDevInfo.FirstOrDefault());
                }


                MsgRequestContract request = (MsgRequestContract)SerializeHelper.DeserializeBytes(bMsg, MessageHead.HeadLength, bMsg.Length - MessageHead.HeadLength);
                string record = "收到客户端发送结果:" + request.Msg;
                this.ShowClientMsg(client, record);



                // record = "发送收到结果消息给客户端:";
                //this.ShowClientMsg(client, record);
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
            ClientInfo curIpDevInfo=null;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<string,System.Net.IPEndPoint>(this.ShowEvent), msg,ipe);
            }
            else
            {
                if (msg=="上线")
                {
                    #region 上线处理
                    
                    var result = clientInfos.Where(p => p.IpPoint == ipe);
                    if (result == null || result.FirstOrDefault() == null)
                    {
                        ClientInfo info = new ClientInfo();
                        info.IpPoint = ipe;
                        info.State = 1;
                        info.StateDes = "在线";
                        info.IsOnLine = 1;
                        if (curResult!=null)
                        {
                            info.Code = curResult.FirstOrDefault().Code;
                            info.Name = curResult.FirstOrDefault().Name;
                        }
                        curIpDevInfo=info;
                        clientInfos.Add(info);
                    }
                    else
                    {
                     
                        result.FirstOrDefault().State = 1;
                        result.FirstOrDefault().StateDes = "在线";
                    }

                    #endregion
                }
                if (msg=="下线")
                {
                    #region 下线处理
                    
                    var result = clientInfos.Where(p => p.IpPoint == ipe);
                    if (result != null && result.FirstOrDefault()!=null)
                    {
                        result.FirstOrDefault().State = 0;
                        result.FirstOrDefault().StateDes = "下线";
                        result.FirstOrDefault().IsOnLine = 0;
                    }
                     curIpDevInfo=result.FirstOrDefault();

                    #endregion
                }

                this.lvIps.Items.Clear();
                for (int i = 0; i < clientInfos.Count; i++)
                {
                    ListViewItem itemIp = new ListViewItem(new string[] {clientInfos[i].Name,clientInfos[i].Code, clientInfos[i].IpPoint.ToString(), clientInfos[i].StateDes });
                    this.lvIps.Items.Insert(0, itemIp);

                    //更新控件显示状态

                    foreach (Control item in panelPC.Controls)
                    {
                        if (item is PCUserControl)
                        {
                            PCUserControl testPcControl = (PCUserControl)item;
                            if (testPcControl.Name==clientInfos[i].Code)
                            {
                                testPcControl.SetMsg(clientInfos[i].StateDes);
                            }
                        }
                    }
                }

                this.toolStripLabel_event.Text = ipe.ToString()+msg;
                string msgText = curIpDevInfo.Name + "--" + curIpDevInfo.Code+"--"+ipe.ToString()+":"+msg;;
                this.rchInfo.AppendText(msgText+"\r\n");
                this.rtbInfo.AppendText(msgText + "\r\n");
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

        private void ShowClientMsg(IPEndPoint client, string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbDelegate<IPEndPoint,string>(this.ShowClientMsg),client, msg);
            }
            else
            {
                if (msg != "上线")
                {
                    var curInfo = AppInfo.ClientInfoList.Where(p => p.IP == client.Address.ToString());
                    if (curInfo != null && curInfo.FirstOrDefault() != null)
                    {
                        msg = curInfo.FirstOrDefault().Name + "--" + curInfo.FirstOrDefault().Code + "--" + curInfo.FirstOrDefault().IP + ":"+msg;
                    }

                    rchInfo.AppendText(msg+"\r\n");
                    rtbInfo.AppendText(msg + "\r\n");

                    ClientLogModel model=new ClientLogModel();
                    model.Content=msg;
                    model.IP = client.Address.ToString();
                    InsertLog(model);
                }
                else
                {

                    var result = clientInfos.Where(p => p.IpPoint == client);
                    if (result == null || result.FirstOrDefault() == null)
                    {
                        ClientInfo info = new ClientInfo();
                        info.IpPoint = client;
                        info.State = 1;
                        info.StateDes = "在线";
                        clientInfos.Add(info);
                    }
                    else
                    {
                        result.FirstOrDefault().State = 1;
                        result.FirstOrDefault().StateDes = "在线";
                    }
                    this.lvIps.Items.Clear();
                    for (int i = 0; i < clientInfos.Count; i++)
                    {
                        ListViewItem itemIp = new ListViewItem(new string[] { clientInfos[i].IpPoint.ToString(), clientInfos[i].StateDes });
                        this.lvIps.Items.Insert(0, itemIp);
                    }
                }
              
               
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
        
      
        private void btnFirst_Click(object sender, EventArgs e)
        {
            pageIndex = 0;
            BindLog();

            btnFirst.Enabled = false;
            btnPre.Enabled = false;
            btnNext.Enabled = true;
            btnEnd.Enabled = true;

        }

        private void BindLog()
        {

            string begintime = dtBegin.Value.ToShortDateString() + " 00:00:00";
            string endTime = dtEnd.Value.ToShortDateString() + " 23:59:59";
            DataTable dt = new ClientLogDal().GetLogDt(pageIndex, pageSize, tbDevCode.Text.Trim(), begintime, endTime);
           this.dgvLog.DataSource=dt;
           totalLogCount = new ClientLogDal().GetLogDtCount(tbDevCode.Text.Trim(), begintime, endTime);
           if (totalLogCount % pageSize == 0)
           {
               totalPage = totalLogCount / pageSize;
           }
           else
           {
               totalPage = totalLogCount / pageSize + 1;
           }

           if (totalLogCount==0)
           {
               btnFirst.Enabled = false;
               btnEnd.Enabled = false;
               btnPre.Enabled = false;
               btnNext.Enabled = false;
           }


        }

  
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgvLog.AutoGenerateColumns = false;

            if (this.tabControl1.SelectedIndex==2)
            {
                dtBegin.Value = DateTime.Now;
                BindLog();
            }
            if (this.tabControl1.SelectedIndex ==1)
            {
                this.dgvClientInfo.AutoGenerateColumns = false;
                BindClientIPInfo();
            }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            pageIndex--;
            if (pageIndex <= 0)
            {
                pageIndex = 0;
                btnFirst.Enabled = false;
                btnPre.Enabled = false;
                btnNext.Enabled = true;
                btnEnd.Enabled = true;
            }
                BindLog();
   

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            pageIndex++;
            if (pageIndex >= totalPage - 1)
            {
                btnFirst.Enabled = true;
                btnPre.Enabled = true;
                btnNext.Enabled = false;
                btnEnd.Enabled = false;
            }
            BindLog();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {

            if (totalPage>0)
            {
                pageIndex = totalPage - 1;
                BindLog();
                btnNext.Enabled = false;
                btnEnd.Enabled = false;
                btnPre.Enabled = true;
                btnFirst.Enabled = true;
            }
        
           
        }

        #endregion

        #region 页面加载
    
        private void Form1_Load(object sender, EventArgs e)
        {
            //当前设备编码
            SeverInfo.DeviceCode = ConfigurationManager.AppSettings["devicecode"];
            this.Text = this.Text + SeverInfo.DeviceCode;//当前编码

            List<ClientIpInfoModel> list = new List<ClientIpInfoModel>();
            DataTable dt = new ClientInfoDal().GetDS();
            if (dt!=null&&dt.Rows.Count>0)
            {
                int width = 0;
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
                    
                    PCUserControl pc = new PCUserControl();
                    pc.Name = model.Code;
                    pc.LabelText(model.Name+"--"+model.Code);
                    pc.Location = new Point(width, 0);
                    width = width + pc.Width;
                    pc.SetMsg("离线");
                    panelPC.Controls.Add(pc);
                }
            }
            AppInfo.ClientInfoList = list;

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

            //串口打开
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("串口打开失败" + ex.Message);
                MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Task.Factory.StartNew(()=>{
          WriteLog();
        });
            //日志处理
          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timerResult.Enabled = false;
            _timerSendBarCode.Enabled = false;
            _timerResult.Dispose();
            _timerSendBarCode.Dispose();
            _timerResult = null;
            _timerSendBarCode = null;

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindLog();
        }

        #region 串口

        void Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

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

        public void DispUI(byte[] InputBuf)
        {
            //textBox1.Text = Convert.ToString(InputBuf);

            ASCIIEncoding encoding = new ASCIIEncoding();
            string info= encoding.GetString(InputBuf);
            LogHelper.Log("串口读取条码:" + info);
          //  richTextBox1.Text = encoding.GetString(InputBuf);
            rtbInfo.Text += info+"\r\n";

            string paraInfo = GetRequestInfo("", info, "", "", "", "", "", "");
            if (!string.IsNullOrEmpty(info))
            {
                #region 调取MES接口
                new MesDaalService().Connect(messerver, int.Parse(mesport), paraInfo);
                #endregion
            }
            
        }
        #endregion
        public string GetRequestInfo(string deviceCode, string sn,string step,string workcode,string line,string statusCode,string extendone,string extendtwo)
        {
            ScanBarCodeMesRequest request = new ScanBarCodeMesRequest();
            request.DeviceCode = deviceCode;
            request.SN = sn;
            request.STEP = step;
            request.WorkCode = workcode;
            request.LineSort = line;
            request.StatusCode = statusCode;
            request.ExtendOne = extendone;
            request.ExtendTwo = extendtwo;
            return request.DeviceCode + "," + request.SN+","+request.STEP+","+request.WorkCode+","+request.LineSort+",,"+request.StatusCode+","+request.ExtendOne+","+request.ExtendTwo;
        }


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


    }


  

}
