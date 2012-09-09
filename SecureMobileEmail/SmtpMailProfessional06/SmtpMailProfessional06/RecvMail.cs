using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SmtpMailProfessional06
{
    class RecvMail
    {
        public int mailNo;
        public string from;
        public string fromMail;
        public string subject;
        public string dateTimeInfo;
        public int attachmentCount;
        public string[] attachments;
        public string body;
        public RecvMail()
        {
        }
    }
}
