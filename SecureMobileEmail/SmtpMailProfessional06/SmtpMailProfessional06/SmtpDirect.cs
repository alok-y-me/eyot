using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
//using System.Web.Mail;

namespace SMTP
{
    /// <summary>
    /// provides methods to send email via smtp direct to mail server
    /// </summary>
    public class SmtpDirect
    {
        /// <summary>
        /// Get / Set the name of the SMTP mail server
        /// </summary>
        //static long MAX_SIZE = 12582912;
        static long max_length = 1024000;
        public static string SmtpServer;
        public static int SmtpPort;
        static string passPhrase = "Pas5pr@se";           // can be any string
        static string saltValue = "s@1tValue";            // can be any string
        static string hashAlgorithm = "SHA1";             // can be "MD5" or "SHA1"
        static int passwordIterations = 1;                // can be any number
        static string initVector = "@1B2c3D4e5F6g7H8";    // must be 16 bytes
        static int keySize = 128;                         // can be 192 or 128 or 256

        private enum SMTPResponse : int
        {
            CONNECT_SUCCESS = 220,
            GENERIC_SUCCESS = 250,
            DATA_SUCCESS = 354,
            QUIT_SUCCESS = 221,
            MSGLEN_SUCCESS = 000

        }
        public static bool Send(MailMessage message)
        {
            IPHostEntry IPhst = Dns.GetHostEntry(SmtpServer);
            IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], SmtpPort);
            Socket s = new Socket(endPt.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("connecting..........");
            s.Connect(endPt);
            Console.WriteLine("connected........");

            string msg = "";
            msg = msg + message.From + "\n";
            msg = msg + message.To + "\n";
            msg = msg + message.Subject + "\n";
            
            int attch_count = 0;
            if (message.Attachments.Count > 0)
                attch_count = message.Attachments.Count;
            msg = msg + attch_count + "\n";

            if (attch_count > 0)
            {
                foreach (object o in message.Attachments)
                {
                    MailAttachment a = o as MailAttachment;
                    //int pos = a.Filename.LastIndexOf('\\');
                    char[] delim = new char[1];
                    delim[0] = '\\';
                    string[] nameParts = a.Filename.Split(delim);
                    //nt count = a.Filename.Length - (pos+1);
                    //msg = msg + a.Filename.Substring(pos+1, count) + "\n";
                    string filename = nameParts[nameParts.Length - 1];
                    filename = Path.GetFileName(a.Filename);
                    msg = msg + filename + "\n";
                }
            }

            msg = msg + message.Body;

            Senddata(s, msg, false);
            if (Check_Response(s, SMTPResponse.MSGLEN_SUCCESS))
            {
                //MessageBox.Show("length acked");
            }

            if (message.Attachments.Count > 0)
            {
               

                foreach (object o in message.Attachments)
                {
                    MailAttachment a = o as MailAttachment;
                    byte[] binaryData;
                    if (a != null)
                    {
                        FileInfo f = new FileInfo(a.Filename);
                        
                        FileStream fs = f.OpenRead();
                        int chunksize = 600000;
                        
                        for (int j = 0; j < fs.Length; j = j + chunksize)
                        {
                            int count = chunksize;
                            if ( (j + count )> fs.Length)
                                count = (int)(fs.Length -(long)j);
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
                            
                            SendMsgPart(s, base64String);
                            
                            
                        }
                        SendMsgPart(s, "__done__");
                        fs.Close();
                        
                    }
                }
                
            }

            
            
            return true;
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
            if (Check_Response(s, SMTPResponse.MSGLEN_SUCCESS))
            {
                //MessageBox.Show("length acked");
            }
             //   MessageBox.Show("length acked");


            s.Send(_msgPart, 0, _msgPart.Length, SocketFlags.None);
            
            // Receive an ack
            if (Check_Response(s, SMTPResponse.MSGLEN_SUCCESS))
            {
                //MessageBox.Show("msg part acked");
            }

        }


        private static void Senddata(Socket s, string msg, bool isMsgBody)
        {
            byte[] _msg = new byte[max_length];

            
            msg = RijndaelSimple.Encrypt(msg,
                                               passPhrase,
                                               saltValue,
                                               hashAlgorithm,
                                               passwordIterations,
                                               initVector,
                                               keySize);

            
            _msg = Convert.FromBase64String(msg);
            
            if (isMsgBody)
            {// send the length 
                string msgLen = "" + _msg.Length;
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
                //MessageBox.Show("rbytes = " + _msg.Length + " length = " + msg.Length);
             // Receive an ack
                if (Check_Response(s, SMTPResponse.MSGLEN_SUCCESS)) ;
                   // MessageBox.Show("length acked");
            }
            s.Send(_msg, 0, _msg.Length, SocketFlags.None);
            
        }


        private static bool Check_Response(Socket s, SMTPResponse response_expected)
        {
            string sResponse;
            int response;
            byte[] bytes = new byte[2048];
            while (s.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            int rcvd = s.Receive(bytes, 0, s.Available, SocketFlags.None);
            sResponse = Convert.ToBase64String(bytes, 0, rcvd);
            
            sResponse = RijndaelSimple.Decrypt(sResponse,
                                                    passPhrase,
                                                    saltValue,
                                                    hashAlgorithm,
                                                    passwordIterations,
                                                    initVector,
                                                    keySize);
            //MessageBox.Show(sResponse);
            response = Convert.ToInt32(sResponse.Substring(0, 3));
            if (response != (int)response_expected)
                return false;
            return true;
        }

        private static bool Check_Response_old(Socket s, SMTPResponse response_expected)
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
            response = Convert.ToInt32(sResponse.Substring(0, 3));
            if (response != (int)response_expected)
                return false;
            return true;
        }
    }
}

