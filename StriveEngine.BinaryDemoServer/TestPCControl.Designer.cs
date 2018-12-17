namespace StriveEngine.BinaryDemoServer
{
    partial class TestPCControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pc003 = new StriveEngine.BinaryDemoServer.PCUserControl();
            this.pc004 = new StriveEngine.BinaryDemoServer.PCUserControl();
            this.pc002 = new StriveEngine.BinaryDemoServer.PCUserControl();
            this.pc001 = new StriveEngine.BinaryDemoServer.PCUserControl();
            this.SuspendLayout();
            // 
            // pc003
            // 
            this.pc003.Location = new System.Drawing.Point(15, 117);
            this.pc003.Name = "pc003";
            this.pc003.Size = new System.Drawing.Size(139, 99);
            this.pc003.TabIndex = 0;
            // 
            // pc004
            // 
            this.pc004.Location = new System.Drawing.Point(183, 117);
            this.pc004.Name = "pc004";
            this.pc004.Size = new System.Drawing.Size(139, 99);
            this.pc004.TabIndex = 0;
            // 
            // pc002
            // 
            this.pc002.Location = new System.Drawing.Point(183, 3);
            this.pc002.Name = "pc002";
            this.pc002.Size = new System.Drawing.Size(139, 99);
            this.pc002.TabIndex = 0;
            // 
            // pc001
            // 
            this.pc001.Location = new System.Drawing.Point(3, 1);
            this.pc001.Name = "pc001";
            this.pc001.Size = new System.Drawing.Size(139, 99);
            this.pc001.TabIndex = 0;
            // 
            // TestPCControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pc003);
            this.Controls.Add(this.pc004);
            this.Controls.Add(this.pc002);
            this.Controls.Add(this.pc001);
            this.Name = "TestPCControl";
            this.Size = new System.Drawing.Size(345, 222);
            this.Load += new System.EventHandler(this.TestPCControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private PCUserControl pc001;
        private PCUserControl pc002;
        private PCUserControl pc004;
        private PCUserControl pc003;
    }
}
