using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FwCore.Tasks;
using Atms.Common;

namespace StriveEngine.BinaryDemoServer
{
    public partial class TestPCControl : UserControl
    {
        public TestPCControl()
        {
            InitializeComponent();
        }

        private void RefreshUI()
        {

            TaskEx.Run(delegate
            {
                while (true)
                {
                    foreach (var item in SystemInfo.ClientInfoList)
                    {
                        if (item.Name == "pc001")
                        {
                            UpdatePcUi(pc001, item.IsOnLine,item.Name);
                        }
                        if (item.Name == "pc002")
                        {
                            UpdatePcUi(pc001, item.IsOnLine, item.Name);
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }

            });
        
        }

        private void UpdatePcUi(PCUserControl pc, int status,string name)
        {
            if (pc.InvokeRequired)
            {
                pc.Invoke((MethodInvoker)delegate
                {
                    pc.SetPcName(name);
                     // pc.SetOnLineStatus(status);
                    if (status == 1)
                    {
                         pc.SetOnLineStatus("在线");
                    }
                    else
                    {
                        pc.SetOnLineStatus("离线");
                    }
                });
            }
            else
            {
                pc.SetPcName(name);
               // pc.SetOnLineStatus(status);
                if (status == 1)
                {
                    pc.SetOnLineStatus("在线");
                }
                else
                {
                    pc.SetOnLineStatus("离线");
                }
            }
        }

        private void TestPCControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            RefreshUI();

        }
    }
}
