namespace RoomClient
{
    partial class Chess
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
            this.Player = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Player
            // 
            this.Player.Location = new System.Drawing.Point(185, 3);
            this.Player.Name = "Player";
            this.Player.Size = new System.Drawing.Size(106, 75);
            this.Player.TabIndex = 2;
            this.Player.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel.Location = new System.Drawing.Point(12, 93);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(450, 360);
            this.panel.TabIndex = 3;
            // 
            // Chess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 468);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.Player);
            this.Name = "Chess";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Player;
        private System.Windows.Forms.Panel panel;
    }
}

