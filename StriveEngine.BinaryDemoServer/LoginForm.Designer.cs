namespace StriveEngine.BinaryDemoServer
{
    partial class LoginForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.tbFctCode = new System.Windows.Forms.TextBox();
            this.tbLoginCode = new System.Windows.Forms.TextBox();
            this.btnSure = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "机台号:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "工  号:";
            // 
            // tbFctCode
            // 
            this.tbFctCode.Location = new System.Drawing.Point(129, 59);
            this.tbFctCode.Name = "tbFctCode";
            this.tbFctCode.Size = new System.Drawing.Size(156, 21);
            this.tbFctCode.TabIndex = 1;
            this.tbFctCode.Text = "SENTRY-T2-5";
            // 
            // tbLoginCode
            // 
            this.tbLoginCode.Location = new System.Drawing.Point(129, 104);
            this.tbLoginCode.Name = "tbLoginCode";
            this.tbLoginCode.Size = new System.Drawing.Size(156, 21);
            this.tbLoginCode.TabIndex = 1;
            this.tbLoginCode.Text = "M009287";
            // 
            // btnSure
            // 
            this.btnSure.Location = new System.Drawing.Point(63, 162);
            this.btnSure.Name = "btnSure";
            this.btnSure.Size = new System.Drawing.Size(75, 23);
            this.btnSure.TabIndex = 2;
            this.btnSure.Text = "登录";
            this.btnSure.UseVisualStyleBackColor = true;
            this.btnSure.Click += new System.EventHandler(this.btnSure_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(210, 162);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 240);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSure);
            this.Controls.Add(this.tbLoginCode);
            this.Controls.Add(this.tbFctCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动化设备登陆";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbFctCode;
        private System.Windows.Forms.TextBox tbLoginCode;
        private System.Windows.Forms.Button btnSure;
        private System.Windows.Forms.Button btnClose;
    }
}