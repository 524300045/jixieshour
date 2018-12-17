using Atms.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StriveEngine.BinaryDemoServer
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSure_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFctCode.Text.Trim()))
            {
                MessageBox.Show("请输入机台号");
                return;
            }
            if (string.IsNullOrWhiteSpace(tbLoginCode.Text.Trim()))
            {
                MessageBox.Show("请输入工号");
                return;
            }
            SystemInfo.LoginCode = tbLoginCode.Text.Trim();
            SystemInfo.FctCode = tbFctCode.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }
    }
}
