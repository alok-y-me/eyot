using System;
using System.Text;
using System.Collections;

namespace SMTP
{
    /// <summary>
    /// Summary description for MailMessage.
    /// </summary>
    public class MailMessage
    {
        #region Fields

        private string to;
        private string from;
        private string body;
        private string subject;
        private string cc;
        private string bcc;
        private Encoding bodyEncoding;
        private IList attachments;

        #endregion

        #region Constructors

        public MailMessage()
        {
            attachments = new ArrayList();
        }

        #endregion

        #region properties

        public string To
        {
            get
            {
                return this.to;
            }
            set
            {

                this.to = value;
            }
        }

        public string From
        {
            get
            {
                return this.from;
            }
            set
            {

                this.from = value;
            }
        }

        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
            }
        }

        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
            }
        }

        public string Cc
        {
            get
            {
                return this.cc;
            }
            set
            {

                this.cc = value;
            }
        }

        public string Bcc
        {
            get
            {
                return this.bcc;
            }
            set
            {

                this.bcc = value;
            }
        }

        public Encoding BodyEncoding
        {
            get
            {
                return this.bodyEncoding;
            }
            set
            {
                this.bodyEncoding = value;
            }
        }

        public IList Attachments
        {
            get
            {
                return this.attachments;
            }
        }


        #endregion

    }
}

