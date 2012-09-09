namespace SmtpMailProfessional06
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
            this.inbox_tab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.readButton = new System.Windows.Forms.Button();
            this.msgBox = new System.Windows.Forms.ListBox();
            this.buttonAction = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmdSend = new System.Windows.Forms.Button();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtSmtp = new System.Windows.Forms.TextBox();
            this.txtAttachment = new System.Windows.Forms.TextBox();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.msg1 = new System.Windows.Forms.Label();
            this.msg1Button = new System.Windows.Forms.Button();
            this.inbox_tab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // inbox_tab
            // 
            this.inbox_tab.Controls.Add(this.tabPage1);
            this.inbox_tab.Controls.Add(this.tabPage2);
            this.inbox_tab.Location = new System.Drawing.Point(0, 0);
            this.inbox_tab.Name = "inbox_tab";
            this.inbox_tab.SelectedIndex = 0;
            this.inbox_tab.Size = new System.Drawing.Size(240, 291);
            this.inbox_tab.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.readButton);
            this.tabPage1.Controls.Add(this.msgBox);
            this.tabPage1.Controls.Add(this.buttonAction);
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(240, 268);
            this.tabPage1.Text = "inbox";
            // 
            // readButton
            // 
            this.readButton.Location = new System.Drawing.Point(139, 230);
            this.readButton.Name = "readButton";
            this.readButton.Size = new System.Drawing.Size(72, 20);
            this.readButton.TabIndex = 3;
            this.readButton.Text = "read";
            this.readButton.Click += new System.EventHandler(this.readButton_Click);
            // 
            // msgBox
            // 
            this.msgBox.Location = new System.Drawing.Point(0, 16);
            this.msgBox.Name = "msgBox";
            this.msgBox.Size = new System.Drawing.Size(237, 170);
            this.msgBox.TabIndex = 2;
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(26, 230);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(72, 20);
            this.buttonAction.TabIndex = 1;
            this.buttonAction.Text = "action";
            this.buttonAction.Click += new System.EventHandler(this.buttonAction_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cmdSend);
            this.tabPage2.Controls.Add(this.txtBody);
            this.tabPage2.Controls.Add(this.txtPort);
            this.tabPage2.Controls.Add(this.txtSmtp);
            this.tabPage2.Controls.Add(this.txtAttachment);
            this.tabPage2.Controls.Add(this.txtSubject);
            this.tabPage2.Controls.Add(this.txtFrom);
            this.tabPage2.Controls.Add(this.txtTo);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(232, 265);
            this.tabPage2.Text = "compose mail";
            // 
            // cmdSend
            // 
            this.cmdSend.Location = new System.Drawing.Point(141, 233);
            this.cmdSend.Name = "cmdSend";
            this.cmdSend.Size = new System.Drawing.Size(72, 20);
            this.cmdSend.TabIndex = 21;
            this.cmdSend.Text = "send";
            this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
            // 
            // txtBody
            // 
            this.txtBody.Location = new System.Drawing.Point(7, 181);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(206, 46);
            this.txtBody.TabIndex = 13;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(113, 137);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 21);
            this.txtPort.TabIndex = 12;
            // 
            // txtSmtp
            // 
            this.txtSmtp.Location = new System.Drawing.Point(113, 111);
            this.txtSmtp.Name = "txtSmtp";
            this.txtSmtp.Size = new System.Drawing.Size(100, 21);
            this.txtSmtp.TabIndex = 11;
            // 
            // txtAttachment
            // 
            this.txtAttachment.Location = new System.Drawing.Point(113, 83);
            this.txtAttachment.Name = "txtAttachment";
            this.txtAttachment.Size = new System.Drawing.Size(100, 21);
            this.txtAttachment.TabIndex = 10;
            // 
            // txtSubject
            // 
            this.txtSubject.Location = new System.Drawing.Point(113, 56);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(100, 21);
            this.txtSubject.TabIndex = 9;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(113, 29);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(100, 21);
            this.txtFrom.TabIndex = 8;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(113, 4);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(100, 21);
            this.txtTo.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(7, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.Text = "body";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(7, 138);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.Text = "port";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 20);
            this.label5.Text = "sever";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.Text = "attachment";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.Text = "subject";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.Text = "sender email id";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.Text = "Receiver email id";
            // 
            // msg1
            // 
            this.msg1.Location = new System.Drawing.Point(0, 0);
            this.msg1.Name = "msg1";
            this.msg1.Size = new System.Drawing.Size(100, 20);
            // 
            // msg1Button
            // 
            this.msg1Button.Location = new System.Drawing.Point(0, 0);
            this.msg1Button.Name = "msg1Button";
            this.msg1Button.Size = new System.Drawing.Size(72, 20);
            this.msg1Button.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.inbox_tab);
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.inbox_tab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl inbox_tab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtSmtp;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Button cmdSend;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.ListBox msgBox;
        private System.Windows.Forms.Button readButton;
        private System.Windows.Forms.TextBox txtAttachment;

    }
}


