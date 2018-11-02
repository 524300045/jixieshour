using StriveEngine.BinaryDemoServer.Dal;
using StriveEngine.BinaryDemoServer.Model;
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
    public partial class AddIPForm : Form
    {
        public AddIPForm()
        {
            InitializeComponent();
        }



        private void btnSure_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbIpName.Text.Trim()))
                {
                    MessageBox.Show("设备名称不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(tbCode.Text.Trim()))
                {
                    MessageBox.Show("设备代号不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(tbIp.Text.Trim()))
                {
                    MessageBox.Show("设备IP不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(tbIpPort.Text.Trim()))
                {
                    MessageBox.Show("设备端口不能为空");
                    return;
                }

                if (string.IsNullOrEmpty(tbIpTimoOut.Text.Trim()))
                {
                    MessageBox.Show("超时时间不能为空");
                    return;
                }
                int timeout = 0;
                try
                {
                    timeout = Convert.ToInt16(tbIpTimoOut.Text.Trim());
                    if (timeout < 0)
                    {
                        MessageBox.Show("超时时间不能小于0");
                        return;
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("超时时间请输入数字");
                    return;
                }

                ClientIpInfoModel model = new ClientIpInfoModel();
                model.Name = tbIpName.Text.Trim();
                model.Code = tbCode.Text.Trim();
                model.IP = tbIp.Text.Trim();
                model.Port = tbIpPort.Text.Trim();
                model.Timeouts = timeout;
                new ClientInfoDal().Insert(model);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {

                MessageBox.Show("出现异常:" + ex.Message);
            }


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AddIPForm_Load(object sender, EventArgs e)
        {
            tbIpName.Focus();
        }


    }
}
