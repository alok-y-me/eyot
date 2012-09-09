using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Spartan.Net.Mail;
using OpenPOP;
using OpenPOP.MIMEParser;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Net.Mail;


namespace recv_pop2
{
    public partial class Form1 : Form
    {
        static long MAX_SIZE = 20971520;
        static string host;
        static int port;
        static string user;
        static string pass;
        static bool isReceiverAsleep = true;
        static bool closeForm = false;
        static bool receiverDone = true;
        static bool senderDone = true;
        static string passPhrase = "Pas5pr@se";           // can be any string
        static string saltValue = "s@1tValue";            // can be any string
        static string hashAlgorithm = "SHA1";             // can be "MD5" or "SHA1"
        static int passwordIterations = 1;                // can be any number
        static string initVector = "@1B2c3D4e5F6g7H8";    // must be 16 bytes
        static int keySize = 128;                         // can be 192 or 128 or 256

       
        
   

        public Form1()
        {
            InitializeComponent();
        }

        private static string getResponse64(Socket s)
        {
            string sResponse;
            byte[] bytes = new byte[2048];
            while (s.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            int recvd = s.Receive(bytes, 0, s.Available, SocketFlags.None);
            
            sResponse = Convert.ToBase64String(bytes, 0, recvd);
            
            sResponse = RijndaelSimple.Decrypt(sResponse,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);


            return sResponse;
        }

        private static string recvMsgPart(Socket sockin)
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

        private static void SendMsgPart(Socket s, string msgPart)
        {
            msgPart = RijndaelSimple.Encrypt(msgPart,
                                               passPhrase,
                                               saltValue,
                                               hashAlgorithm,
                                               passwordIterations,
                                               initVector,
                                               keySize);


            byte[] _msgPart = new byte[msgPart.Length];
            _msgPart = Convert.FromBase64String(msgPart);
            //_msgPart = Encoding.ASCII.GetBytes(msgPart);

            string msgLen = "" + _msgPart.Length;
            msgLen = RijndaelSimple.Encrypt(msgLen,
                                           passPhrase,
                                           saltValue,
                                           hashAlgorithm,
                                           passwordIterations,
                                           initVector,
                                           keySize);
            //MessageBox.Show(msgLen);

            byte[] _msgLen = new byte[1024];
            _msgLen = Convert.FromBase64String(msgLen);
            s.Send(_msgLen, 0, _msgLen.Length, SocketFlags.None);
            //MessageBox.Show("rbytes = " + _msgPart.Length + " length = " + msgPart.Length);
            // Receive an ack

            string sresp = getResponse64(s);
            sresp = sresp.Substring(0,3);
            if (!sresp.Equals("000"))
                MessageBox.Show("something is wrong");
            //   MessageBox.Show("length acked");


            s.Send(_msgPart, 0, _msgPart.Length, SocketFlags.None);

            // Receive an ack
            sresp = getResponse64(s);
            sresp = sresp.Substring(0, 3);
            if (!sresp.Equals("000"))
                MessageBox.Show("something is wrong");
            

        }


        private void receiveMails()
        {
            /*Spartan.Net.Mail.POP3 pop3 = new Spartan.Net.Mail.POP3();
            pop3.Connect(host, port, user, pass);*/
            receiverDone = false;
            int count = -1;
            string dirname;
            string filename;
            BinaryWriter bw;
            FileStream fs;
            BinaryReader br;
            //fs = new FileStream("E:\\redirector\\count.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            //br = new BinaryReader(fs);
            //count = Int32.Parse(br.ReadString());
            //br.Close();
            //fs.Close();
            lock (this)
            {
                count = Int32.Parse(File.ReadAllText("E:\\redirector\\count.txt"));
            }
            Console.WriteLine("count is " + count);

            while (true)
            {
               // Thread.Sleep(0);
                isReceiverAsleep = false;
                int newMessages = 0;
                int readMessages = 0;

                //pop3.ListMessages(out readMessages, out newMessages);

                OpenPOP.POP3.POPClient popclient = new OpenPOP.POP3.POPClient();
                popclient.Connect(host, port);
                popclient.Authenticate(user, pass);
  
                newMessages = popclient.GetMessageCount();

                for (int i = 1; i <= newMessages; i++)
                {
                    dirname = "E:\\redirector\\sent_mails\\" + (count + i);
                    Directory.CreateDirectory(dirname);
                    string message = "";
                    //pop3.RetrieveMessage(i, out message);
                    //pop3.DeleteMessage(i);
                    //bool fnsh = true;
                    //OpenPOP.MIMEParser.Message msg = new OpenPOP.MIMEParser.Message(ref fnsh, message);
                    OpenPOP.MIMEParser.Message msg = popclient.GetMessage(i, false);
                    message = msg.From + "\n" + msg.FromEmail + "\n" + msg.Subject + "\n" + msg.DateTimeInfo + "\n" + msg.AttachmentCount + "\n";
                    // Write to approproiate file instead of printing

                    //string show_msg = string.Format(sndr + "::" + subject);
                    //MessageBox.Show(show_msg);
                    for (int j = 1; j < msg.AttachmentCount; j++)
                    {
                        OpenPOP.MIMEParser.Attachment attachment = (OpenPOP.MIMEParser.Attachment)msg.Attachments[j];
                        message = message + attachment.ContentFileName + "\n";
                        filename = dirname + "\\" + attachment.ContentFileName;
                        msg.SaveAttachment(attachment, filename);
                       /* byte[] bytes = attachment.DecodedAsBytes();
                        filename = dirname + "\\" + attachment.ContentFileName;
                        fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                        bw = new BinaryWriter(fs);
                        bw.Flush();
                        bw.Close();
                        bw.Write(bytes);
                        fs.Close();*/
                        //MessageBox.Show(string.Format("attachment: {0}", attachment.ContentFileName));
                        //MessageBox.Show(attachment_txt);
                    }
                    //msg.SaveAttachments(dirname);
                    if (msg.AttachmentCount > 0)
                    {
                        OpenPOP.MIMEParser.Attachment attachment2 = (OpenPOP.MIMEParser.Attachment)msg.Attachments[0];
                        message = message + attachment2.DecodeAsText();
                    }
                    else
                    {
                        message = message + msg.MessageBody[msg.MessageBody.Count - 1];
                    }
                    filename = dirname + "\\mail.txt";
                    /*fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                    bw = new BinaryWriter(fs);
                    bw.Write(message);
                    bw.Flush();
                    bw.Close();
                    fs.Close();*/
                    File.WriteAllText(filename, message);
                    popclient.DeleteMessage(i);
                    //popclient.DeleteAllMessages();
                    // MessageBox.Show(message);
                }
                popclient.Disconnect();
                //pop3.Disconnect();
                //fs = new FileStream("E:\\redirector\\count.txt", FileMode.Open, FileAccess.Write, FileShare.Read);
                //bw = new BinaryWriter(fs);
                count = count + newMessages;
                string ncount = "" + count;
                lock (this)
                {
                    File.WriteAllText("E:\\redirector\\count.txt", ncount);
                }
                //bw.Write((int)count);
                //bw.Flush();
                //bw.Close();
                //fs.Close();
                isReceiverAsleep = true;
                /*if (closeForm)
                    break;*/
                Thread.Sleep(10000);
                
                //if (closeForm)
                  //  break;
            }
            receiverDone = true;
        }

        private void sendMails()
        {
            //IPAddress smtp_server = IPAddress.Parse("144.16.192.6");
            senderDone = false;
            IPAddress smtp_server = IPAddress.Parse(host);
            IPAddress redirector = IPAddress.Parse(host);
            int r_port = 8001;
            int smtp_port = 25;
            int response;
            bool isMsgBody = false;

            TcpListener listener = new TcpListener(redirector, r_port);
            listener.Start();
            

            while (true)
            {
                Console.WriteLine("Waiting for connections..........");
                Socket sockin = listener.AcceptSocket();
                Console.WriteLine("got connection from " + sockin.RemoteEndPoint.ToString());

                string msg = getResponse64(sockin);
                char[] delim = new char[1];
                delim[0] = '\n';
                string[] lines = msg.Split(delim);

                SendMail mail = new SendMail();
                mail.from = lines[0];
                mail.to = lines[1];
                mail.subject = lines[2];
                mail.attachmentCount = Int32.Parse(lines[3]);

                int i;
                if(mail.attachmentCount > 0){
                    mail.attachments = new string[mail.attachmentCount];
                    for (i = 0; i < mail.attachmentCount; i++)
                        mail.attachments[i] = lines[4 + i];
                }

                mail.body = "";
                for (i = 4 + mail.attachmentCount; i < lines.Length; i++) 
                    mail.body = mail.body + "\n" + lines[i];

                string sreq = "000";
                sreq = RijndaelSimple.Encrypt(sreq,
                                                passPhrase,
                                                saltValue,
                                                hashAlgorithm,
                                                passwordIterations,
                                                initVector,
                                                keySize);
                byte[] req = new byte[1024];
                req = Convert.FromBase64String(sreq);
                sockin.Send(req, 0, req.Length, SocketFlags.None);

                for (i = 0; i < mail.attachmentCount; i++)
                {
                    string fullname = "E:\\redirector\\tmp\\" + mail.attachments[i];
                    FileInfo f = new FileInfo(fullname);
                    FileStream fs = f.Open(FileMode.Create, FileAccess.Write);

                    bool done = false;
                    do
                    {
                        string filepart;
                        filepart = recvMsgPart(sockin);
                        if (filepart.Equals("__done__"))
                        {
                            fs.Close();
                            done = true;
                        }
                        else
                        {
                            byte[] filebytes = new byte[filepart.Length];
                            filebytes = Convert.FromBase64String(filepart);
                            fs.Write(filebytes, 0, filebytes.Length);
                        }

                    } while (!done);
                }
                System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage();
                mailMsg.To.Add(mail.to);
                System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(mail.from);
                mailMsg.From = mailAddress;
                mailMsg.Subject = mail.subject;
                mailMsg.Body = mail.body;

                for (i = 0; i < mail.attachmentCount; i++)
                {
                    string fullname = "E:\\redirector\\tmp\\" + mail.attachments[i];
                    System.Net.Mail.Attachment attchmnt = new System.Net.Mail.Attachment(fullname);
                    mailMsg.Attachments.Add(attchmnt);
                }

                SmtpClient smtpClient = new SmtpClient("10.108.7.138", 25);
                smtpClient.Send(mailMsg);
                sockin.Close();
            }

        }

        private void handleClient()
        {
            IPAddress redirector = IPAddress.Parse("10.108.7.138");
            int h_port = 8002;
            int count = 0;
            int requested = 0;

            TcpListener listener = new TcpListener(redirector, h_port);
            listener.Start();
            Console.WriteLine("listening for requests .........");

            while (true)
            {
                byte[] req = new byte[1024];
                byte[] resp = new byte[10240];
                bool flag = true;
                System.Text.Encoding enc = System.Text.Encoding.ASCII;

                
                Socket sockin = listener.AcceptSocket();
                Console.WriteLine("Got request connection...");
                
               /* int available;
                do
                {
                    try
                    {
                        available = sockin.Available;
                    }
                    catch (ObjectDisposedException exp)
                    {
                        //flag = false;
                        break;
                    }
                } while (available == 0);*/

                // get the offset
                int offset = 0;
                //int rbytes = sockin.Receive(req, 0, sockin.Available, SocketFlags.None);
                //string sreq = string.Format("{0}", enc.GetString(req));
                string sreq = getResponse64(sockin);
                offset = Int32.Parse(sreq);
                lock (this)
                {
                    count = Int32.Parse(File.ReadAllText("E:\\redirector\\count.txt"));
                }
                // send number mails to be received by the mobile app
                int mailsTosend = 0;
                if(count > offset){
                    if(((count - offset) -10) > 0)
                        mailsTosend = 10;
                    else
                        mailsTosend = count - offset;
                }

                string sresp = "" + mailsTosend;
                sresp = RijndaelSimple.Encrypt(sresp,
                                               passPhrase,
                                               saltValue,
                                               hashAlgorithm,
                                               passwordIterations,
                                               initVector,
                                               keySize);

                resp = Convert.FromBase64String(sresp);
                sockin.Send(resp, 0, resp.Length, SocketFlags.None);

                //resp = enc.GetBytes(sresp);
                //sockin.Send(resp);

                //rbytes = sockin.Receive(req);
                //sreq = enc.GetString(req);
                sreq = getResponse64(sockin);
                int resp_code = Int32.Parse(sreq);
                if (resp_code == 1)
                {
                    for (int i = 0; i < mailsTosend; i++)
                    {// send (offset - i) th mail
                        string mail;
                        int mail_no = count - offset - i;
                        string filename = "E:\\redirector\\sent_mails\\" + mail_no + "\\mail.txt";
                        mail = File.ReadAllText(filename);
                        mail = mail_no + "\n" + mail;
                        mail = RijndaelSimple.Encrypt(mail,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);
                        
                        resp = Convert.FromBase64String(mail);
                        sockin.Send(resp, 0, resp.Length, SocketFlags.None);
                     // receive an ack
                        //rbytes = sockin.Receive(req);
                        //sreq = enc.GetString(req);
                        sreq = getResponse64(sockin);
                        resp_code = Int32.Parse(sreq);
                        if (resp_code != 1)
                            break;
                    }
                }
                sockin.Close();
            }

        }

        private void handleAttachments()
        {
            IPAddress redirector = IPAddress.Parse("10.108.7.138");
            int h_port = 8003;
          
            TcpListener listener = new TcpListener(redirector, h_port);
            listener.Start();
            Console.WriteLine("listening for requests .........");

            while (true)
            {
                byte[] req = new byte[1024];
                byte[] resp = new byte[10240];
                bool flag = true;
                System.Text.Encoding enc = System.Text.Encoding.ASCII;


                Socket sockin = listener.AcceptSocket();
                Console.WriteLine("Got request connection...");

                string sreq = getResponse64(sockin);
                char [] delim = new char[1];
                delim[0] = '\n';
                string[] lines = sreq.Split(delim);
                int mailNo = Int32.Parse(lines[0]);
                string filename = lines[1];

                filename = "E:\\redirector\\sent_mails\\" + mailNo + "\\" + filename;

                FileInfo f = new FileInfo(filename);
                FileStream fs = f.OpenRead();
                byte[] binaryData;

                int chunksize = 600000;
                for (int j = 0; j < fs.Length; j = j + chunksize)
                {
                    int count = chunksize;
                    if ((j + count) > fs.Length)
                        count = (int)(fs.Length - (long)j);
                    int bytesRead = 0;
                    binaryData = null;
                    binaryData = new Byte[count];
                    fs.Seek((long)j, SeekOrigin.Begin);
                    while (bytesRead < count)
                    {
                        int bRead = fs.Read(binaryData, bytesRead, count - bytesRead);
                        bytesRead = bytesRead + bRead;
                    }
                    string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                    SendMsgPart(sockin, base64String);
                }
                SendMsgPart(sockin, "__done__");
                fs.Close();
                sockin.Close();
                MessageBox.Show(filename + " sent");
            }

        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            connect_button.Enabled = false;
            host = server_textbox.Text;
            port = Int32.Parse(port_textbox.Text);
            user = user_textbox.Text;
            pass = pass_textbox.Text;

            Thread receiverThread = new Thread(receiveMails);
            receiverThread.Name = "receiver";
            receiverThread.IsBackground = true;
            receiverThread.Start();
            Thread senderThread = new Thread(sendMails);
            senderThread.Name = "sender";
            senderThread.IsBackground = true;
            senderThread.Start();
            Thread handlerThread = new Thread(handleClient);
            handlerThread.Name = "handler";
            handlerThread.IsBackground = true;
            handlerThread.Start();
            Thread attachmentThread = new Thread(handleAttachments);
            attachmentThread.Name = "attachmenthandler";
            attachmentThread.IsBackground = true;
            attachmentThread.Start();
            //senderThread.Join();
            //receiverThread.Join();
            //connect_button.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           // closeForm = true;
            //while (!receiverDone)Thread.Sleep(100);
            
            
            //Application.Exit();
            Console.WriteLine("Closing the form");
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            //closeForm = true;
            //while (!receiverDone) Thread.Sleep(100);
            
            //
            //Application.Exit();
            Console.WriteLine("Closing the form");
            
        }
    }
}