using System.Windows.Forms;
using System.Drawing;

namespace TestWinForm
{
    class PanelWithoutAutoScoll : Panel
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }

    partial class StirrupForm
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
            this.picturebox = new System.Windows.Forms.PictureBox();
            this.choose_stirrup_type = new System.Windows.Forms.Button();
            this.parambox = new System.Windows.Forms.GroupBox();
            this.tip = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // picturebox
            // 
            this.picturebox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picturebox.Location = new System.Drawing.Point(10, 10);
            this.picturebox.Name = "picturebox";
            this.picturebox.Size = new System.Drawing.Size(300, 300);
            this.picturebox.TabIndex = 0;
            this.picturebox.TabStop = false;
            this.picturebox.Paint += new System.Windows.Forms.PaintEventHandler(this.picturebox1_Paint);
            // 
            // choose_stirrup_type
            // 
            this.choose_stirrup_type.Location = new System.Drawing.Point(220, 410);
            this.choose_stirrup_type.Name = "choose_stirrup_type";
            this.choose_stirrup_type.Size = new System.Drawing.Size(90, 25);
            this.choose_stirrup_type.TabIndex = 2;
            this.choose_stirrup_type.Text = "选择箍筋类型";
            this.choose_stirrup_type.UseVisualStyleBackColor = true;
            this.choose_stirrup_type.Click += new System.EventHandler(this.choose_stirruptype_Click);
            // 
            // parambox
            // 
            this.parambox.Location = new System.Drawing.Point(10, 320);
            this.parambox.Name = "parambox";
            this.parambox.Size = new System.Drawing.Size(300, 80);
            this.parambox.TabIndex = 7;
            this.parambox.TabStop = false;
            this.parambox.Text = "配筋参数";
            // 
            // tip
            // 
            this.tip.AutoSize = true;
            this.tip.ForeColor = System.Drawing.Color.Red;
            this.tip.Location = new System.Drawing.Point(10, 415);
            this.tip.Name = "tip";
            this.tip.Size = new System.Drawing.Size(0, 12);
            this.tip.TabIndex = 8;
            // 
            // StirrupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(319, 442);
            this.Controls.Add(this.tip);
            this.Controls.Add(this.parambox);
            this.Controls.Add(this.choose_stirrup_type);
            this.Controls.Add(this.picturebox);
            this.MaximumSize = new System.Drawing.Size(335, 700);
            this.MinimumSize = new System.Drawing.Size(335, 460);
            this.Name = "StirrupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "箍筋";
            this.Load += new System.EventHandler(this.StirrupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button choose_stirrup_type;
        public PictureBox picturebox;
        private GroupBox parambox;
        private Label tip;

    }
}

