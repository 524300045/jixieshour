namespace StriveEngine.BinaryDemoServer
{
    partial class PCUserControl
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
            this.labelname = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelname
            // 
            this.labelname.BackColor = System.Drawing.Color.Green;
            this.labelname.Location = new System.Drawing.Point(8, 9);
            this.labelname.Name = "labelname";
            this.labelname.Size = new System.Drawing.Size(124, 42);
            this.labelname.TabIndex = 14;
            this.labelname.Text = "未知";
            this.labelname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbStatus.Location = new System.Drawing.Point(47, 58);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(31, 12);
            this.lbStatus.TabIndex = 15;
            this.lbStatus.Text = "离线";
            // 
            // PCUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.labelname);
            this.Name = "PCUserControl";
            this.Size = new System.Drawing.Size(139, 95);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelname;
        private System.Windows.Forms.Label lbStatus;
    }
}
