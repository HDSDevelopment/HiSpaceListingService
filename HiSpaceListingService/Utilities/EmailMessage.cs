using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace HiSpaceListingService.Utilities
{
    public class EmailMessage
    {
        string Host = "smtp.gmail.com";
        bool EnableSsl = true;
        bool UseDefaultCredentials = false;
        int Port = 587;
        string email = "kanu.priya@highbrowdiligence.com";
        string password = "hds@12345";

        public bool Send(string ToEmail, string Subject, string Body)
        {
            var smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = UseDefaultCredentials;
            smtpClient.Host = Host;
            smtpClient.Port = Port;
            smtpClient.Credentials = new NetworkCredential(email, password);
            smtpClient.EnableSsl = EnableSsl;

            try
            {
                //smtpClient.Send("email", "recipient", "subject", "body");
                smtpClient.Send(email, ToEmail, Subject, Body);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
