namespace WCAE.WForms
{
    using WCAE.WGUIs.Controls;
    using System.Windows.Forms;
    partial class Form_Main: Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开TriangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开WGOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开MeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.StatuBar1 = new System.Windows.Forms.StatusStrip();
            this.Statu1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.StatuBar1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(190, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开TriangleToolStripMenuItem,
            this.打开WGOToolStripMenuItem,
            this.打开MeshToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开TriangleToolStripMenuItem
            // 
            this.打开TriangleToolStripMenuItem.Name = "打开TriangleToolStripMenuItem";
            this.打开TriangleToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.打开TriangleToolStripMenuItem.Text = "打开Triangle";
            this.打开TriangleToolStripMenuItem.Click += new System.EventHandler(this.打开TriangleToolStripMenuItem_Click);
            // 
            // 打开WGOToolStripMenuItem
            // 
            this.打开WGOToolStripMenuItem.Name = "打开WGOToolStripMenuItem";
            this.打开WGOToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.打开WGOToolStripMenuItem.Text = "打开WGO";
            this.打开WGOToolStripMenuItem.Click += new System.EventHandler(this.打开WGOToolStripMenuItem_Click);
            // 
            // 打开MeshToolStripMenuItem
            // 
            this.打开MeshToolStripMenuItem.Name = "打开MeshToolStripMenuItem";
            this.打开MeshToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.打开MeshToolStripMenuItem.Text = "打开Mesh";
            this.打开MeshToolStripMenuItem.Click += new System.EventHandler(this.打开MeshToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.DimGray;
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainer1.Panel1.ImeMode = System.Windows.Forms.ImeMode.On;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.StatuBar1);
            this.splitContainer1.Size = new System.Drawing.Size(947, 650);
            this.splitContainer1.SplitterDistance = 190;
            this.splitContainer1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(48, 72);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StatuBar1
            // 
            this.StatuBar1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Statu1});
            this.StatuBar1.Location = new System.Drawing.Point(0, 628);
            this.StatuBar1.Name = "StatuBar1";
            this.StatuBar1.Size = new System.Drawing.Size(753, 22);
            this.StatuBar1.TabIndex = 0;
            this.StatuBar1.Text = "statusStrip1";
            // 
            // Statu1
            // 
            this.Statu1.Name = "Statu1";
            this.Statu1.Size = new System.Drawing.Size(56, 17);
            this.Statu1.Text = "状态显示";
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 650);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimizeBox = false;
            this.Name = "Form_Main";
            this.Text = "WForms";
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.ResizeBegin += new System.EventHandler(this.Form_Main_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form_Main_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form_Main_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.StatuBar1.ResumeLayout(false);
            this.StatuBar1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开WGOToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem 打开TriangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开MeshToolStripMenuItem;
        private System.Windows.Forms.StatusStrip StatuBar1;
        private System.Windows.Forms.ToolStripStatusLabel Statu1;

    }
}

