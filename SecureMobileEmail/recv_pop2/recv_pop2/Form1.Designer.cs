namespace recv_pop2
{
    partial class Form1
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
            this.title_label = new System.Windows.Forms.Label();
            this.server_label = new System.Windows.Forms.Label();
            this.server_textbox = new System.Windows.Forms.TextBox();
            this.port_label = new System.Windows.Forms.Label();
            this.port_textbox = new System.Windows.Forms.TextBox();
            this.user_label = new System.Windows.Forms.Label();
            this.user_textbox = new System.Windows.Forms.TextBox();
            this.pass_label = new System.Windows.Forms.Label();
            this.pass_textbox = new System.Windows.Forms.TextBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.exit_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Location = new System.Drawing.Point(93, 23);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(64, 13);
            this.title_label.TabIndex = 0;
            this.title_label.Text = "POP3 Client";
            // 
            // server_label
            // 
            this.server_label.AutoSize = true;
            this.server_label.Location = new System.Drawing.Point(27, 51);
            this.server_label.Name = "server_label";
            this.server_label.Size = new System.Drawing.Size(63, 13);
            this.server_label.TabIndex = 1;
            this.server_label.Text = "pop3 server";
            // 
            // server_textbox
            // 
            this.server_textbox.Location = new System.Drawing.Point(115, 48);
            this.server_textbox.Name = "server_textbox";
            this.server_textbox.Size = new System.Drawing.Size(115, 20);
            this.server_textbox.TabIndex = 2;
            // 
            // port_label
            // 
            this.port_label.AutoSize = true;
            this.port_label.Location = new System.Drawing.Point(27, 88);
            this.port_label.Name = "port_label";
            this.port_label.Size = new System.Drawing.Size(25, 13);
            this.port_label.TabIndex = 3;
            this.port_label.Text = "port";
            // 
            // port_textbox
            // 
            this.port_textbox.Location = new System.Drawing.Point(115, 85);
            this.port_textbox.Name = "port_textbox";
            this.port_textbox.Size = new System.Drawing.Size(115, 20);
            this.port_textbox.TabIndex = 4;
            // 
            // user_label
            // 
            this.user_label.AutoSize = true;
            this.user_label.Location = new System.Drawing.Point(27, 124);
            this.user_label.Name = "user_label";
            this.user_label.Size = new System.Drawing.Size(53, 13);
            this.user_label.TabIndex = 5;
            this.user_label.Text = "username";
            // 
            // user_textbox
            // 
            this.user_textbox.Location = new System.Drawing.Point(115, 121);
            this.user_textbox.Name = "user_textbox";
            this.user_textbox.Size = new System.Drawing.Size(115, 20);
            this.user_textbox.TabIndex = 6;
            // 
            // pass_label
            // 
            this.pass_label.AutoSize = true;
            this.pass_label.Location = new System.Drawing.Point(27, 164);
            this.pass_label.Name = "pass_label";
            this.pass_label.Size = new System.Drawing.Size(35, 13);
            this.pass_label.TabIndex = 7;
            this.pass_label.Text = "label1";
            // 
            // pass_textbox
            // 
            this.pass_textbox.Location = new System.Drawing.Point(115, 161);
            this.pass_textbox.Name = "pass_textbox";
            this.pass_textbox.Size = new System.Drawing.Size(115, 20);
            this.pass_textbox.TabIndex = 8;
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(155, 207);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(75, 23);
            this.connect_button.TabIndex = 9;
            this.connect_button.Text = "connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // exit_button
            // 
            this.exit_button.Location = new System.Drawing.Point(30, 206);
            this.exit_button.Name = "exit_button";
            this.exit_button.Size = new System.Drawing.Size(75, 23);
            this.exit_button.TabIndex = 10;
            this.exit_button.Text = "exit";
            this.exit_button.UseVisualStyleBackColor = true;
            this.exit_button.Click += new System.EventHandler(this.exit_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.exit_button);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.pass_textbox);
            this.Controls.Add(this.pass_label);
            this.Controls.Add(this.user_textbox);
            this.Controls.Add(this.user_label);
            this.Controls.Add(this.port_textbox);
            this.Controls.Add(this.port_label);
            this.Controls.Add(this.server_textbox);
            this.Controls.Add(this.server_label);
            this.Controls.Add(this.title_label);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label server_label;
        private System.Windows.Forms.TextBox server_textbox;
        private System.Windows.Forms.Label port_label;
        private System.Windows.Forms.TextBox port_textbox;
        private System.Windows.Forms.Label user_label;
        private System.Windows.Forms.TextBox user_textbox;
        private System.Windows.Forms.Label pass_label;
        private System.Windows.Forms.TextBox pass_textbox;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Button exit_button;
    }
}

