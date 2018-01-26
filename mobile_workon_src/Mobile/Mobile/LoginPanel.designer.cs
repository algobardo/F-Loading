namespace Mobile
{
    partial class LoginPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPanel));
            this.textBoxuser = new System.Windows.Forms.TextBox();
            this.textBoxpass = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.Label();
            this.pass = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // textBoxuser
            // 
            this.textBoxuser.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textBoxuser.Location = new System.Drawing.Point(108, 199);
            this.textBoxuser.Name = "textBoxuser";
            this.textBoxuser.Size = new System.Drawing.Size(115, 21);
            this.textBoxuser.TabIndex = 0;
            // 
            // textBoxpass
            // 
            this.textBoxpass.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textBoxpass.Location = new System.Drawing.Point(108, 226);
            this.textBoxpass.Name = "textBoxpass";
            this.textBoxpass.PasswordChar = '*';
            this.textBoxpass.Size = new System.Drawing.Size(115, 21);
            this.textBoxpass.TabIndex = 1;
            // 
            // user
            // 
            this.user.BackColor = System.Drawing.Color.Black;
            this.user.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.user.ForeColor = System.Drawing.Color.White;
            this.user.Location = new System.Drawing.Point(19, 199);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(83, 20);
            this.user.Text = "Username";
            // 
            // pass
            // 
            this.pass.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.pass.ForeColor = System.Drawing.Color.White;
            this.pass.Location = new System.Drawing.Point(19, 227);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(73, 20);
            this.pass.Text = "Password";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(240, 237);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // 
            // LoginPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pass);
            this.Controls.Add(this.user);
            this.Controls.Add(this.textBoxpass);
            this.Controls.Add(this.textBoxuser);
            this.Controls.Add(this.pictureBox1);
            this.Name = "LoginPanel";
            this.Size = new System.Drawing.Size(240, 294);
            this.Resize += new System.EventHandler(this.LoginPanel_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxuser;
        private System.Windows.Forms.TextBox textBoxpass;
        private System.Windows.Forms.Label user;
        private System.Windows.Forms.Label pass;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
