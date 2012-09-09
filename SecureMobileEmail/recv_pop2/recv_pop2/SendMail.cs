using System;
using System.Collections.Generic;
using System.Text;

namespace recv_pop2
{
    class SendMail
    {
        public string from;
        public string to;
        public string subject;
        public int attachmentCount;
        public string[] attachments;
        public string body;
        public SendMail()
        {
        }
    }
}
