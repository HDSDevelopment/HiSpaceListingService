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

        //enquiry
        public bool SendEnquiry(string ToEmail, string Subject, string Message, string Phone, string Name, string Email)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.Subject = Subject;
                mm.Body = "<div> Dear <span style='font-weight:600'> HiSpace-User</span></div>"
                    + "<br>"
                    + "<div> <span style='font-weight:600'>You have one new enquiry</span></div>"
                    + "<br>"
                    + "<div><span style='font-weight:600'>Name: </span>" + Name + "</div>"
                    + "<div><span style='font-weight:600'>Email: </span>" + Email + "</div>"
                    + "<div><span style='font-weight:600'>Phone: </span>" + Phone + "</div>"
                    + "<div><span style='font-weight:600'>Message: </span>" + Message + "</div>"
                    + "<br>"
                    + "<br>"
                    + "<div>Thank You</div>";
                mm.IsBodyHtml = true;

                var smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = UseDefaultCredentials;
                smtpClient.Host = Host;
                smtpClient.Port = Port;
                smtpClient.Credentials = new NetworkCredential(email, password);
                smtpClient.EnableSsl = EnableSsl;

                try
                {
                    //smtpClient.Send("email", "recipient", "subject", "body");
                    smtpClient.Send(mm);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        //Signup
        public bool SendSignup(string ToEmail, string Subject, string Name, string Password)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.Subject = Subject;
                mm.Body = "<div> Dear <span style='font-weight:600'>" + Name + "</span></div>"
                    +"<br>"
                    + "<div>You have successfully signed up for The Next Generation Real-Estate Industry Evolution <span style='font-weight:600'> 'HiSpace'</span></div>"
                    +"<br>"
                    + "<div>HiSpace-User Credential is given below</div>"
                    + "<div><span style='font-weight:600'> Your Email/UserName: " + ToEmail + "</span></div>"
                    + "<div> <span style='font-weight:600'>Password: " + Password + "</span></div>"
                    + "<br>" 
                    + "<br>"
                    + "<div>Thank You</div>";
                mm.IsBodyHtml = true;

                var smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = UseDefaultCredentials;
                smtpClient.Host = Host;
                smtpClient.Port = Port;
                smtpClient.Credentials = new NetworkCredential(email, password);
                smtpClient.EnableSsl = EnableSsl;

                try
                {
                    //smtpClient.Send("email", "recipient", "subject", "body");
                    smtpClient.Send(mm);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        //Background check
        public bool BackgroundCheckEmail(string ToEmail, string Name, string Subject)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.Subject = Subject;
                mm.Body = "<div> Dear <span style='font-weight:600'>" + Name + "</span></div>"
                    + "<div> Your background verification process completed successfully. Now you are able to add your <span style='font-weight:600'>Space/Properties/Professional</span> in our HiSpace application</div>"
                    + "<br>"
                    + "<br>"
                    + "<div>Thank You</div>";
                mm.IsBodyHtml = true;

                var smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = UseDefaultCredentials;
                smtpClient.Host = Host;
                smtpClient.Port = Port;
                smtpClient.Credentials = new NetworkCredential(email, password);
                smtpClient.EnableSsl = EnableSsl;

                try
                {
                    //smtpClient.Send("email", "recipient", "subject", "body");
                    smtpClient.Send(mm);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }


    }
}
