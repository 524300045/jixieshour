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
    public partial class RobotUserControl : UserControl
    {

        public delegate void OutPutDelegate(string info);

        public event OutPutDelegate OutPutEvent;


        public RobotUserControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OutPutEvent!=null)
            {
                OutPutEvent("aa");
            }
        }
    }
}
