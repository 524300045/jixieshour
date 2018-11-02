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

        public void SetMsg(string msg)
        {
            this.lbMsg.Text = msg;
        }
    }
}
