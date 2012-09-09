using System;
using System.Text;
using System.IO;

namespace SMTP
{
    /// <summary>
    /// Summary description for MailAttachment.
    /// </summary>
    public class MailAttachment
    {
        #region Fields

        private Encoding encoding;
        private string filename;

        #endregion

        #region Constructors

        public MailAttachment(string filename)
        {
            this.filename = filename;
            this.encoding = Encoding.Default;
            CheckFile(filename);
        }

        public MailAttachment(string filename, Encoding encoding)
        {
            this.filename = filename;
            this.encoding = encoding;
            CheckFile(filename);
        }

        #endregion

        #region properties


        public string Filename
        {
            get
            {
                return this.filename;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }

        #endregion

        #region helper methods


        private void CheckFile(string filename)
        {
            try
            {
                // Verify if we can open the file for reading
                File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read).Close();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Bad attachment", filename);
            }

        }

        #endregion
    }
}
