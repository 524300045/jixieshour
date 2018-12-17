using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StriveEngine.BinaryDemoServer
{
    public partial class PCUserControl : UserControl
    {
        public PCUserControl()
        {
            InitializeComponent();
        }

        public void LabelText(string text)
        {
            this.labelname.Text = text;
        }

        public void SetOnLineStatus(string msg)
        {
            if (this.InvokeRequired)
            {
                lbStatus.Invoke((MethodInvoker)delegate {
                    this.lbStatus.Text = msg;
                });
            }
            else
            {
                this.lbStatus.Text = msg;
            }
            
        }

        public void SetPcName(string name)
        {
            
            if (this.InvokeRequired)
            {
                lbStatus.Invoke((MethodInvoker)delegate
                {
                    labelname.Text = name;
                });
            }
            else
            {
                labelname.Text = name;
            }
            
        }

       
    }
}
