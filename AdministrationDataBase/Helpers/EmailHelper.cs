using System.Net;
using System.Net.Mail;

namespace AdministrationDataBase.Helpers
{
    public class EmailHelper
    {
        private readonly MailMessage Message = null;

        private readonly SmtpClient smtpClient = null;

        public MailAddress FromAddress { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailHelper()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            IConfiguration config = builder.Build();
            smtpClient = new SmtpClient
            {
                Host = config["Smtp:Server"],  //Configure as your email provider
                UseDefaultCredentials = false,
                EnableSsl = bool.Parse(config["Smtp:UseSSL"]),//comment if you don't need SSL  
                Credentials = new NetworkCredential(config["Smtp:UserName"], config["Smtp:Password"]),
                Port = Convert.ToInt32(config["Smtp:Port"])
            };
            Message = new MailMessage();
            FromAddress = new MailAddress(config["Site:Email"], config["Site:Name"]);

        }

        public EmailHelper(string host, int port, string userName, string password, bool ssl)
          : this()
        {
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
            smtpClient.Credentials = new NetworkCredential(userName, password);
        }

        public void AddToAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.To.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        public void AddCcAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.CC.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        public void AddBccAddress(string email, string name = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.Replace(",", ";");
                string[] emailList = email.Split(';');
                for (int i = 0; i < emailList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(emailList[i]))
                        Message.Bcc.Add(new MailAddress(emailList[i], name));
                }
            }
        }

        public void AddAttachment(string file, string mimeType)
        {
            Attachment attachment = new(file, mimeType);
            Message.Attachments.Add(attachment);
        }

        public void AddAttachment(Attachment objAttachment)
        {
            Message.Attachments.Add(objAttachment);
        }

        public void SendMail()
        {
            if (FromAddress == null || FromAddress != null && FromAddress.Address.Equals(""))
            {
                throw new Exception("From address not defined");
            }
            else
            {
                if (string.IsNullOrEmpty(FromAddress.DisplayName))
                    FromAddress = new MailAddress(FromAddress.Address, string.Empty);
                Message.From = FromAddress;
            }

            if (Message.To.Count <= 0)
            { throw new Exception("To address not defined"); }
            Message.Subject = Subject;
            Message.IsBodyHtml = true;
            Message.Body = Body;

            try
            {
                smtpClient.Send(Message);
            }
            catch
            {
            }
        }
    }
}