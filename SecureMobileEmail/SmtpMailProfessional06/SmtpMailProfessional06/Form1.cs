using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SMTP;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace SmtpMailProfessional06
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Label msg1;
        private Button msg1Button;
        static List<string> msgList = new List<string>();
        private TabPage[] tb_index = new TabPage[10];
        static int offset = 0;
        static int mailInList = 0;
        static int len = 0;
        static bool listUpdated = false;
        static RecvMail[] mailList = new RecvMail[10];
        public delegate void updateMsgBoxD();
        public updateMsgBoxD msgBoxDelegate;
        public delegate void updateProgressBarD(bool visible, bool add, bool set, int val);
        public updateProgressBarD prgBarDelegate;

        static string passPhrase = "Pas5pr@se";           // can be any string
        static string saltValue = "s@1tValue";            // can be any string
        static string hashAlgorithm = "SHA1";             // can be "MD5" or "SHA1"
        static int passwordIterations = 1;                // can be any number
        static string initVector = "@1B2c3D4e5F6g7H8";    // must be 16 bytes
        static int keySize = 128;                         // can be 192 or 128 or 256


        public Form1()
        {
            InitializeComponent();
            int i;
            for (i = 0; i < 10; i++)
            {
                tb_index[i] = new TabPage();
                mailList[i] = new RecvMail();
            }

            Thread receiverThread = new Thread(receiveMails);
            receiverThread.IsBackground = true;
            receiverThread.Start();
            msgBoxDelegate = new updateMsgBoxD(updateMsgBoxMethod);
            
        }

        
        public void updateMsgBoxMethod()
        {
            this.msgBox.DataSource = null;
            this.msgBox.DataSource = msgList;
            //msgBox.Update();

            ((CurrencyManager)this.msgBox.BindingContext[msgList]).Refresh();
               
        }

        private void updateMsgBox()
        {
            Console.WriteLine("Inside updateMsgBox........");
            while (true)
            {
                while (!listUpdated)
                    Thread.Sleep(1000);
                msgBox.DataSource = null;
                msgBox.DataSource = msgList;
                ((CurrencyManager)msgBox.BindingContext[msgList]).Refresh();
                listUpdated = false;
                Thread.Sleep(2000);
            }
        }

        private static string getResponse(Socket s)
        {
            string sResponse;
            int response;
            byte[] bytes = new byte[2048];
            while (s.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            s.Receive(bytes, 0, s.Available, SocketFlags.None);
            sResponse = Encoding.ASCII.GetString(bytes, 0, (int)bytes.Length);
            //MessageBox.Show(sResponse);
            
            return sResponse;
        }

        private static string getResponse64(Socket s)
        {
            string sResponse;
            int response;
            byte[] bytes = new byte[2048];
            while (s.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            int recvd = s.Receive(bytes, 0, s.Available, SocketFlags.None);
            //sResponse = Encoding.ASCII.GetString(bytes, 0, (int)bytes.Length);
            //sResponse = Convert.ToBase64String(bytes);
            sResponse = Convert.ToBase64String(bytes, 0, recvd);
            //MessageBox.Show("len = " + recvd + "\n" + sResponse);
            sResponse = RijndaelSimple.Decrypt(sResponse,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);

            //MessageBox.Show(sResponse);

            return sResponse;
        }

        private bool getAttachment(int mailNo, string filename)
        {
            string redirector = "10.108.7.138";
            string sreq;
            byte[] req = new byte[1024];
            byte[] resp = new byte[10240];
            string sresp;
            int port = 8003;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            IPHostEntry IPhst = Dns.GetHostEntry(redirector);
            IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], port);

            Socket s = new Socket(endPt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("connecting to redirector..........");
            s.Connect(endPt);
            Console.WriteLine("Connected to redirector!!!");

            // create file
            string fullname = "\\Storage Card\\" + filename;
            FileInfo f = new FileInfo(fullname);
            FileStream fs = f.Open(FileMode.Create, FileAccess.Write);

            // send filename 
            sreq = mailNo + "\n" + filename;
            sreq = RijndaelSimple.Encrypt(sreq,
                                                passPhrase,
                                                saltValue,
                                                hashAlgorithm,
                                                passwordIterations,
                                                initVector,
                                                keySize);

            req = Convert.FromBase64String(sreq);
            s.Send(req, 0, req.Length, SocketFlags.None);

            bool done = false;
            do
            {
                string filepart;
                filepart = recvFilePart(s);
                if (filepart.Equals("__done__"))
                {
                    fs.Close();
                    s.Close();
                    return true;
                }
                else
                {
                    byte[] filebytes = new byte[filepart.Length];
                    filebytes = Convert.FromBase64String(filepart);
                    fs.Write(filebytes, 0, filebytes.Length);
                }

            } while (!done);

            return false;
        }


        private void receiveMails()
        {
            string redirector = "10.108.7.138";
            string sreq;
            byte[] req = new byte[1024];
            byte[] resp = new byte[10240];
            string sresp;
            int port = 8002;
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            IPHostEntry IPhst = Dns.GetHostEntry(redirector);
            IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], port);
            
            

            while (true)
            {
                
                Socket s = new Socket(endPt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("connecting to redirector..........");
                s.Connect(endPt);
                Console.WriteLine("Connected to redirector!!!");
                
                // send the offset
                sreq = "" + offset;
                sreq = RijndaelSimple.Encrypt(sreq,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);

                req = Convert.FromBase64String(sreq);
                s.Send(req, 0, req.Length, SocketFlags.None);
                //req = enc.GetBytes(sreq);
                //s.Send(req);
                // get number of mails to receive
                sresp = getResponse64(s);
                int mailToRecv = Int32.Parse(sresp);
                
                sreq = "" + 1;
                sreq = RijndaelSimple.Encrypt(sreq,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);

                req = Convert.FromBase64String(sreq);
                s.Send(req, 0, req.Length, SocketFlags.None);
                
                //req = enc.GetBytes(sreq);
                //s.Send(req);

                RecvMail[] tmpMailList = new RecvMail[10];

                int i, j;
                for (i = 0; i < mailToRecv; i++)
                {
                    sresp = getResponse64(s);
                    // parse and store msg in a list
                    char[] separator = new char[1];
                    separator[0] = '\n';
                    string[] lines = sresp.Split(separator);
                    tmpMailList[i] = new RecvMail();
                    tmpMailList[i].mailNo = Int32.Parse(lines[0]);
                    tmpMailList[i].from = lines[1];
                    tmpMailList[i].fromMail = lines[2];
                    tmpMailList[i].subject = lines[3];
                    tmpMailList[i].dateTimeInfo = lines[4];
                    //MessageBox.Show(lines[4] + "\n" + lines[5]);
                    tmpMailList[i].attachmentCount = Int32.Parse(lines[5]);
                    tmpMailList[i].attachments = null;
                    tmpMailList[i].attachments = new string[tmpMailList[i].attachmentCount];
                    for (j = 1; j < tmpMailList[i].attachmentCount; j++)
                    {
                        tmpMailList[i].attachments[j] = lines[5 + j];
                    }
                    tmpMailList[i].body = "";
                    for (j = 5 + tmpMailList[i].attachmentCount; j < lines.Length; j++)
                        tmpMailList[i].body = tmpMailList[i].body + "\n" + lines[j];
                    //MessageBox.Show(sresp);
                    sreq = "" + 1;
                    sreq = RijndaelSimple.Encrypt(sreq,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);

                    req = Convert.FromBase64String(sreq);
                    s.Send(req, 0, req.Length, SocketFlags.None);
                    
                    //req = enc.GetBytes(sreq);
                    //s.Send(req);
                }
                // copy temp list to global list
                for(i=0; i<mailToRecv; i++)
                    mailList[i] = tmpMailList[i];
                mailInList = mailToRecv;
                //string msg = "get new msgs again??";
                //MessageBox.Show(msg);

                // display updated list
                msgList.Clear();
                //msgBox.Items.Clear();
                for (i = 0; i < mailInList; i++)
                {
                    string elem = mailList[i].from + "-" + mailList[i].subject;
                    //MessageBox.Show(elem);
                    msgList.Add((string)elem);
                    
                    //msgBox.Items.Add(elem);
                }
                
                //msgBox.Items.
                
                this.msgBox.Invoke(this.msgBoxDelegate);

                Thread.Sleep(20000);
            }
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            SMTP.SmtpDirect.SmtpServer= "10.108.7.138";
            SMTP.SmtpDirect.SmtpServer = txtSmtp.Text;
            SMTP.SmtpDirect.SmtpPort = Int32.Parse(txtPort.Text);

			MailMessage message =new MailMessage();
			message.Body = txtBody.Text;
			message.From = txtFrom.Text;
			message.To = txtTo.Text;
			message.Subject = txtSubject.Text;

			if (txtAttachment.Text != "")
			{
				MailAttachment attachment = new MailAttachment(txtAttachment.Text);
				message.Attachments.Add(attachment);
			}

			if(SMTP.SmtpDirect.Send(message))
			{
				MessageBox.Show("Sent OK");
			}
			else
			{
				MessageBox.Show("Something BAD Happened!");
			}
            txtTo.Text = "";
            txtFrom.Text = "";
            txtSubject.Text = "";
            txtAttachment.Text = "";
            txtSmtp.Text = "";
            txtPort.Text = "";
            txtBody.Text = "";
            
		
        }

        private static string recvFilePart(Socket sockin)
        {
            byte[] req = new byte[1024];
            // Receive length
            while (sockin.Available == 0)
                System.Threading.Thread.Sleep(100);
            int rbytes = sockin.Receive(req, 0, sockin.Available, SocketFlags.None);
            string sreq = Convert.ToBase64String(req, 0, rbytes);
            //MessageBox.Show(sreq);

            sreq = RijndaelSimple.Decrypt(sreq,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);
            int msgLen = Int32.Parse(sreq);
            //MessageBox.Show("length = " + msgLen);
            // send ack for length

            byte[] resp = new byte[2048];
            string sresp = "000";

            //resp = enc.GetBytes(sresp);
            sresp = RijndaelSimple.Encrypt(sresp,
                               passPhrase,
                               saltValue,
                               hashAlgorithm,
                               passwordIterations,
                               initVector,
                               keySize);

            resp = Convert.FromBase64String(sresp);
            sockin.Send(resp, 0, resp.Length, SocketFlags.None);
            // MessageBox.Show("Length ack sent");

            // receive message
            int rcvd = 0;
            string encryptedMsgBody = "";
            req = null;
            req = new byte[msgLen];
            while (rcvd < msgLen)
            {


                while (sockin.Available == 0)
                    System.Threading.Thread.Sleep(100);
                rbytes = sockin.Receive(req, rcvd, sockin.Available, SocketFlags.None);
                rcvd = rcvd + rbytes;
            }
            encryptedMsgBody = Convert.ToBase64String(req);
            //MessageBox.Show("rbytes = " + req.Length + " length = " + encryptedMsgBody.Length);
            sreq = RijndaelSimple.Decrypt(encryptedMsgBody,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);

            //MessageBox.Show(sreq.Substring(0,3) + "  " + sreq.Substring(sreq.Length-3,2));
            resp = null;
            resp = new byte[2048];
            sresp = "000";

            //resp = enc.GetBytes(sresp);
            sresp = RijndaelSimple.Encrypt(sresp,
                               passPhrase,
                               saltValue,
                               hashAlgorithm,
                               passwordIterations,
                               initVector,
                               keySize);

            resp = Convert.FromBase64String(sresp);
            sockin.Send(resp, 0, resp.Length, SocketFlags.None);
            //MessageBox.Show("msgPart ack sent");
            return sreq;
        }


        /*private void buttonAction_Click(object sender, EventArgs e)
        {*/
           /* msg1.Text = "message 1";
            msg1Button.Text = "read";
            
            Point p = new Point(20, 20);
            msg1Button.Click += 
            tabPage1.Controls.Add(msg1);
            tabPage1.Controls.Add(msg1Button);
            msg1.Location = p;
            p = new Point(100, 20);
            msg1Button.Location = p;*/
            
            //msg1.Text = "message 2";
            //tabPage1.Controls.Add(msg1);

         /*   msgList.Add("message 1");
            msgList.Add("message 2");

            msgBox.DataSource = msgList;
        }*/

        private void readButton_Click(object sender, EventArgs e)
        {
            int index = msgBox.SelectedIndex;
            string s1 = "";
            s1 = s1 + mailList[index].from + "\r\nsubject: " + mailList[index].subject + "\r\n" + mailList[index].dateTimeInfo;
            s1 = s1 + "\r\n\r\n" + mailList[index].body;
            int i;
            string s2 = "";
            if (mailList[index].attachmentCount > 0)
                s2 = s2 + "\r\n\r\nAttachments:\r\n";

            Label label1 = new Label();
            label1.Text = s1;
            Label label2 = new Label();
            label2.Text = s2;
            TextBox textbox1 = new TextBox();
            Point p1 = new Point(10, 10);
            textbox1.Multiline = true;
            textbox1.Width = 200;
            textbox1.Height = 100;
            textbox1.Location = p1;
            textbox1.Text = s1;
            textbox1.Enabled = false;
            this.tb_index[index].Controls.Clear();
            this.tb_index[index].Controls.Add(textbox1);

            int h = 120;
            for (i = 1; i < mailList[index].attachmentCount; i++)
            {
                Label attachName = new Label();
                attachName.Text = mailList[index].attachments[i];
                Point p = new Point(10, h);
                attachName.Location = p;
                this.tb_index[index].Controls.Add(attachName);
                Button attachButton = new Button();
                attachButton.Text = "download";
                string filename = mailList[index].attachments[i];
                attachButton.Click += delegate(Object bsender, EventArgs be)
                {
                    int mailNo = mailList[index].mailNo;
                    MessageBox.Show("index = " + mailNo + " i=" + filename);
                    if (this.getAttachment(mailNo, filename))
                        MessageBox.Show("file downloaded");
                    else
                        MessageBox.Show("Unable to download");
                };
                p = new Point(160, h);
                h = h + 20;
                attachButton.Location = p;
                this.tb_index[index].Controls.Add(attachButton);
            }
                
            
            Point p2 = new Point(10, h);
            
            Button closeButton = new Button();
            closeButton.Text = "close";
            closeButton.Location = p2;
            closeButton.Click += delegate(Object sender2, EventArgs e2)
            {
                this.inbox_tab.Controls.Remove(tb_index[index]);
            };
            
            
            this.tb_index[index].Controls.Add(closeButton);
            this.tb_index[index].Text = "msg_" + index;
            this.inbox_tab.Controls.Add(tb_index[index]);
            this.tb_index[index].Focus();
            
            //MessageBox.Show(s);
        }

        private void buttonAction_Click(object sender, EventArgs e)
        {

        }

   
        
       
       
    }
}