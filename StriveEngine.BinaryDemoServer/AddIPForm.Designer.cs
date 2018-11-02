namespace StriveEngine.BinaryDemoServer
{
    partial class AddIPForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbIpName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.tbIp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbIpPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbIpTimoOut = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSure = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "设备名称:";
            // 
            // tbIpName
            // 
            this.tbIpName.Location = new System.Drawing.Point(150, 34);
            this.tbIpName.Name = "tbIpName";
            this.tbIpName.Size = new System.Drawing.Size(192, 21);
            this.tbIpName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "设备代号:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "设备IP:";
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(150, 68);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(192, 21);
            this.tbCode.TabIndex = 1;
            // 
            // tbIp
            // 
            this.tbIp.Location = new System.Drawing.Point(150, 103);
            this.tbIp.Name = "tbIp";
            this.tbIp.Size = new System.Drawing.Size(192, 21);
            this.tbIp.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "端口:";
            // 
            // tbIpPort
            // 
            this.tbIpPort.Location = new System.Drawing.Point(150, 138);
            this.tbIpPort.Name = "tbIpPort";
            this.tbIpPort.Size = new System.Drawing.Size(192, 21);
            this.tbIpPort.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(67, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "超时时间(S):";
            // 
            // tbIpTimoOut
            // 
            this.tbIpTimoOut.Location = new System.Drawing.Point(150, 172);
            this.tbIpTimoOut.Name = "tbIpTimoOut";
            this.tbIpTimoOut.Size = new System.Drawing.Size(192, 21);
            this.tbIpTimoOut.TabIndex = 1;
            this.tbIpTimoOut.Text = "0";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(265, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSure
            // 
            this.btnSure.Location = new System.Drawing.Point(119, 226);
            this.btnSure.Name = "btnSure";
            this.btnSure.Size = new System.Drawing.Size(75, 23);
            this.btnSure.TabIndex = 2;
            this.btnSure.Text = "确定";
            this.btnSure.UseVisualStyleBackColor = true;
            this.btnSure.Click += new System.EventHandler(this.btnSure_Click);
            // 
            // AddIPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 302);
            this.Controls.Add(this.btnSure);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbIpTimoOut);
            this.Controls.Add(this.tbIpPort);
            this.Controls.Add(this.tbIp);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.tbIpName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddIPForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设备添加";
            this.Load += new System.EventHandler(this.AddIPForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbIpName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.TextBox tbIp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbIpPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbIpTimoOut;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSure;
    }
}