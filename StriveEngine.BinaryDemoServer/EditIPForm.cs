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
    public partial class EditIPForm : Form
    {

        public int id;
        public EditIPForm()
        {
            InitializeComponent();
        }

        public EditIPForm(int _id)
        {
            InitializeComponent();
            this.id = _id;
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
                if (rb0.Checked)
                {
                    model.Status = 0;
                }
                else
                {
                    model.Status = 1;
                }
                model.Id = id;
                model.Name = tbIpName.Text.Trim();
                model.Code = tbCode.Text.Trim();
                model.IP = tbIp.Text.Trim();
                model.Port = tbIpPort.Text.Trim();
                model.Timeouts = timeout;
                new ClientInfoDal().Update(model);
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

        private void EditIPForm_Load(object sender, EventArgs e)
        {
            DataTable dt = new ClientInfoDal().GetDSById(id);
            if (dt != null)
            {
                tbIpName.Text = dt.Rows[0]["name"].ToString();
                tbCode.Text = dt.Rows[0]["code"].ToString();
                tbIp.Text = dt.Rows[0]["ip"].ToString();
                tbIpPort.Text = dt.Rows[0]["port"].ToString();
                tbIpTimoOut.Text = dt.Rows[0]["timeouts"].ToString();
                if (dt.Rows[0]["status"].ToString() == "1")
                {
                    rb1.Checked = true;
                    rb0.Checked = false;
                }
                else
                {
                    rb0.Checked = true;
                    rb1.Checked = false;
                }
            }

            tbIpName.Focus();
        }


    }
}
