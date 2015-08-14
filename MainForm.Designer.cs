namespace RigRepeater
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBoxLocal = new System.Windows.Forms.ListBox();
            this.listBoxRemote = new System.Windows.Forms.ListBox();
            this.buttonLink = new System.Windows.Forms.Button();
            this.buttonUnlink = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBoxLocal
            // 
            this.listBoxLocal.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxLocal.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLocal.FormattingEnabled = true;
            this.listBoxLocal.Location = new System.Drawing.Point(2, 30);
            this.listBoxLocal.Name = "listBoxLocal";
            this.listBoxLocal.Size = new System.Drawing.Size(413, 82);
            this.listBoxLocal.TabIndex = 0;
            this.listBoxLocal.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxLocal_DrawItem);
            this.listBoxLocal.SelectedIndexChanged += new System.EventHandler(this.listBoxLocal_SelectedIndexChanged);
            // 
            // listBoxRemote
            // 
            this.listBoxRemote.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxRemote.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxRemote.FormattingEnabled = true;
            this.listBoxRemote.Location = new System.Drawing.Point(2, 168);
            this.listBoxRemote.Name = "listBoxRemote";
            this.listBoxRemote.Size = new System.Drawing.Size(413, 82);
            this.listBoxRemote.TabIndex = 1;
            this.listBoxRemote.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxRemote_DrawItem);
            this.listBoxRemote.SelectedIndexChanged += new System.EventHandler(this.listBoxRemote_SelectedIndexChanged);
            // 
            // buttonLink
            // 
            this.buttonLink.Enabled = false;
            this.buttonLink.Location = new System.Drawing.Point(111, 131);
            this.buttonLink.Name = "buttonLink";
            this.buttonLink.Size = new System.Drawing.Size(223, 23);
            this.buttonLink.TabIndex = 2;
            this.buttonLink.Text = "Link local rig to remote rig\'s frequency";
            this.buttonLink.UseVisualStyleBackColor = true;
            this.buttonLink.Click += new System.EventHandler(this.buttonLink_Click);
            // 
            // buttonUnlink
            // 
            this.buttonUnlink.Enabled = false;
            this.buttonUnlink.Location = new System.Drawing.Point(340, 131);
            this.buttonUnlink.Name = "buttonUnlink";
            this.buttonUnlink.Size = new System.Drawing.Size(75, 23);
            this.buttonUnlink.TabIndex = 3;
            this.buttonUnlink.Text = "Unlink";
            this.buttonUnlink.UseVisualStyleBackColor = true;
            this.buttonUnlink.Click += new System.EventHandler(this.buttonUnlink_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Local Rigs:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-1, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Remote Rigs:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 262);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonUnlink);
            this.Controls.Add(this.buttonLink);
            this.Controls.Add(this.listBoxRemote);
            this.Controls.Add(this.listBoxLocal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "WriteLog Rig Control Repeater";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBoxLocal;
        private System.Windows.Forms.ListBox listBoxRemote;
        private System.Windows.Forms.Button buttonLink;
        private System.Windows.Forms.Button buttonUnlink;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

