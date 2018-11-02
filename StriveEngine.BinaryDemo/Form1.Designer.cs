namespace StriveEngine.BinaryDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button3 = new System.Windows.Forms.Button();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.btnReady = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(251, 36);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "连接";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox_IP
            // 
            this.textBox_IP.Location = new System.Drawing.Point(94, 10);
            this.textBox_IP.Name = "textBox_IP";
            this.textBox_IP.Size = new System.Drawing.Size(205, 21);
            this.textBox_IP.TabIndex = 12;
            this.textBox_IP.Text = "127.0.0.1";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(94, 38);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(149, 21);
            this.textBox_port.TabIndex = 11;
            this.textBox_port.Text = "9000";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(48, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 23);
            this.label6.TabIndex = 9;
            this.label6.Text = "端口：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(12, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 23);
            this.label7.TabIndex = 10;
            this.label7.Text = "服务器IP：";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(14, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 10);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // rtbInfo
            // 
            this.rtbInfo.Location = new System.Drawing.Point(362, 9);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(320, 336);
            this.rtbInfo.TabIndex = 18;
            this.rtbInfo.Text = "";
            // 
            // btnReady
            // 
            this.btnReady.Location = new System.Drawing.Point(50, 85);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(75, 23);
            this.btnReady.TabIndex = 19;
            this.btnReady.Text = "发送Ready";
            this.btnReady.UseVisualStyleBackColor = true;
            this.btnReady.Click += new System.EventHandler(this.btnReady_Click);
            // 
            // btnResult
            // 
            this.btnResult.Location = new System.Drawing.Point(50, 140);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(75, 23);
            this.btnResult.TabIndex = 20;
            this.btnResult.Text = "发送结果";
            this.btnResult.UseVisualStyleBackColor = true;
            this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 369);
            this.Controls.Add(this.btnResult);
            this.Controls.Add(this.btnReady);
            this.Controls.Add(this.rtbInfo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox_IP);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "客户端 ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.Button btnReady;
        private System.Windows.Forms.Button btnResult;
    }
}

