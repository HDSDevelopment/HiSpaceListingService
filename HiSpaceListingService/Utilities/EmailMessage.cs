﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingService.Utilities;
using HiSpaceListingService.ViewModel;
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
		//int Port = 465;
		string email = "no-reply@highbrowdiligence.com";
		string password = "ForWeb@HDS123";

		//enquiry
		public bool SendEnquiry(string ToEmail, string Subject, string Message, string Phone, string Name, string Email)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.Subject = Subject;
                mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                    + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                    + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"
                    +"<br>"
                    + "<div>"
                    + "<div>Dear <span style='font-weight:600'> HiSpace-User,</span></div>"

                    + "<div style='margin: 5px 0; font-weight:600;'>You have one new enquiry</div>"
                    + "<table style='border-collapse: collapse;border: 1px solid #a2a2a2;background: #fbe7c8;width: 100%'>"
                    + "<tboby>"
                    + "<tr>"
                    +"<th colspan='2' style='text-align:center;border: 1px solid #a2a2a2; padding: 10px;'>Enquired User Details</th>"
                    +"</tr>"
                    +"<tr>"
                    + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Name</th>"
                    + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Name + "</td>"
                    +"</tr>"
                    + "<tr>"
                    + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Email</th>"
                    + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Email + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Phone</th>"
                    + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Phone + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Message</th>"
                    + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Message + "</td>"
                    + "</tr>"
                    + "</tbody>"
					+"</table>"

					+ "<br>"
                    + "<br>"


                    + "<div>Sincerely,</div>"
                    + "<div style=''font-weight: 700;>HiSpace Team</div>"
                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                   + "</div>"

                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='text-align: center'>"
                    + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                    + "</div>"

                    + "</div>"
                    ;
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

		//contact form enquiry 
		public bool SendContactFormEnquiry(string Name, string Email, string Phone, string Text, string Subject)
		{
			//using (MailMessage mm = new MailMessage(email, "support@highbrowdiligence.com"))
			using (MailMessage mm = new MailMessage())
			{
				mm.From = new MailAddress(email, "HiSpace Team");
                mm.To.Add(new MailAddress("tamilarasan@highbrowdiligence.com"));
                mm.To.Add(new MailAddress("support@highbrowdiligence.com"));
                mm.Subject = Subject;
				mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
					+ "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

					+ "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"
					+ "<br>"
					+ "<div>"
					+ "<div>Dear <span style='font-weight:600'> HiSpace Team,</span></div>"

					+ "<div style='margin: 5px 0; font-weight:600;'>You have one new enquiry</div>"
					+ "<table style='border-collapse: collapse;border: 1px solid #a2a2a2;background: #fbe7c8;width: 100%'>"
					+ "<tboby>"
					+ "<tr>"
					+ "<th colspan='2' style='text-align:center;border: 1px solid #a2a2a2; padding: 10px;'>Enquired User Details</th>"
					+ "</tr>"
					+ "<tr>"
					+ "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Name</th>"
					+ "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Name + "</td>"
					+ "</tr>"
					+ "<tr>"
					+ "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Email</th>"
					+ "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Email + "</td>"
					+ "</tr>"
					+ "<tr>"
					+ "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Phone</th>"
					+ "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Phone + "</td>"
					+ "</tr>"
					+ "<tr>"
					+ "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Message</th>"
					+ "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + Text + "</td>"
					+ "</tr>"
					+ "</tbody>"
					+ "</table>"

					+ "<br>"
					+ "<br>"


					+ "<div>Sincerely,</div>"
					+ "<div style=''font-weight: 700;>HiSpace Team</div>"
					+ "</div>"

					+ "<br>" + "<br>"
					+ "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
				   + "</div>"

					+ "</div>"

					+ "<br>" + "<br>"
					+ "<div style='text-align: center'>"
					+ "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
					+ "</div>"

					+ "</div>"
					;
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
		public async Task<bool> SendSignup(string ToEmail, string Subject, string Name, string Password, string activationLink)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.Subject = Subject;
                mm.Body =
                    "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                    + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                    + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"

                    + "<div>"
                    + "<div> <h1> <span style='font-weight:600'>Welcome," + Name + "!</span> </h1></div>"
                    + "Please click the below activation link to activate your account "
                    + "<div>"+ activationLink + "</div>"
                    + "<a style='display:block; margin:20px; padding:10px; text-align:center; background:#ff9900; color:#ffffff;' href=" + activationLink + "> click to activate your account </a>"

                    + "<div>We're excited that you've chosen The Next Generation Real-Estate Industry Evolution <span style='font-weight:600'> 'HiSpace'</span>. You're probably ready to dive right in, so let's get started!</div>"
                    + "<br>"

                    +"<div>"
                    + "<h2 style='margin-bottom:0'>Your first email</h2>"
                    + "<p>For your protection (and ours) HiSpace has a brief verification process when you send your first mailing with us. We look it over, check for any broken links, send you an email and wait for your reply.</p>"
                    + "<p>We'll ask for a few details about how your contact list signed up. The goal of our permissions policy is to keep your emails landing in inboxes.</p>"
                    + "</div>"
                     + "<br>"

                    + "<div>"
                    + "<h2 style='margin-bottom:15px'>Your account details</h2>"
                     + "<div><span style='font-weight:600'> To login, Visit: <a href='https://www.thehispace.com/'>https://www.thehispace.com/</a></span></div>"
                     + "<div><span style='font-weight:600'> Your Login Id: " + ToEmail + "</span></div>"
                    + "<div> <span style='font-weight:600'>Password: " + Password + "</span></div>"
                    + "</div>"
                    + "<br>"

                    +"<div>"
                    + "<h2 style='margin-bottom:0'>Need a hand?</h2>"
                    + "<p>You can email the team anytime (24/7). If you ever feel stuck technically or creatively, just yell and we'll come running!</p>"
                    + "</div>"
                    + "<br>"

                    + "<br>"
                    + "<div>Cheers,</div>"
                    + "<div>HiSpace Team</div>"
                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                   + "</div>"

                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='text-align: center'>"
                    + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                    + "</div>"

                    +"</div>"
                    ;
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
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.Subject = Subject;
                mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                    + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                    + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"

                    + "<div>"
                    + "<div> <h1> <span style='font-weight:600'>Welcome to HiSpace!</span> </h1></div>"

                    + "<div>Thank You <span style='font-weight:600'>" + Name + "!</span> We have verified your request with us. Click the below link to login to your account to post your properties, spaces and your profession</div>"
                    + "<br>"

                    + "<div style='text-align:center;'>"
                     + "<a style='color: #ffffff;font-size: 16px;text-decoration: none;background: #f90;padding: 10px 20px;margin-top: 10px;display: inline-block;font-weight: 600;letter-spacing: 1px;border-radius: 30px;box-shadow: 5px 10px 30px -7px #000;' href='https://www.thehispace.com/' target='_blank'>HiSpace</a>"
                    + "</div>"
                     + "<br>"

                    + "<br>"
                    + "<div>Cheers,</div>"
                    + "<div>HiSpace Team</div>"
                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                   + "</div>"

                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='text-align: center'>"
                    + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                    + "</div>"

                    + "</div>"
                    ;
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

        //success email for enquiry
        public bool SendEnquirySuccessEmail(string ToEmail, string Name)
        {
            using (MailMessage mm = new MailMessage(email, ToEmail))
            {
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.Subject = "Enquiry Success";
                mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                    + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                    + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"

                    + "<div>"
                    + "<div> <h1> <span style='font-weight:600'>Welcome to HiSpace!</span> </h1></div>"

                    + "<div>Dear <span style='font-weight:600'>" + Name + ",</span></div>" 
                    + "<div style='margin: 5px 0;'>We would like to acknowledge that we have received your request.</div>"
                    + "<div style='margin: 5px 0;'>A support representative will be reviewing your request and will send you a personal response (usually within 24 hours).</div>"
                    + "<div style='margin: 5px 0;'>Thank you for partnering with HiSpace.</div>"
                    + "<br>"
                    + "<br>"


                    + "<div>Sincerely,</div>"
                    + "<div style=''font-weight: 700;>HiSpace Team</div>"
                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                   + "</div>"

                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='text-align: center'>"
                    + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                    + "</div>"

                    + "</div>"
                    ;
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

		//password recovery

		//Signup
		public bool SendPassword(string ToEmail, string Subject, string Name, string Password)
		{
			using (MailMessage mm = new MailMessage(email, ToEmail))
			{
				mm.From = new MailAddress(email, "HiSpace Team");
				mm.Subject = Subject;
				mm.Body =
					"<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
					+ "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

					+ "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"

					+ "<div>"
					+ "<div> <h1> <span style='font-weight:600'>Welcome," + Name + "!</span> </h1></div>"

					+ "<div>"
					+ "<h2 style='margin-bottom:15px'>Your account details</h2>"
					 + "<div><span style='font-weight:600'> To login, Visit: <a href='https://www.thehispace.com/'>https://www.thehispace.com/</a></span></div>"
					 + "<div><span style='font-weight:600'> Your Login Id: " + ToEmail + "</span></div>"
					+ "<div> <span style='font-weight:600'>Password: " + Password + "</span></div>"
					+ "</div>"
					+ "<br>"

					+ "<div>"
					+ "<h2 style='margin-bottom:0'>Need a hand?</h2>"
					+ "<p>You can email the team anytime (24/7). If you ever feel stuck technically or creatively, just yell and we'll come running!</p>"
					+ "</div>"
					+ "<br>"

					+ "<br>"
					+ "<div>Cheers,</div>"
					+ "<div>HiSpace Team</div>"
					+ "</div>"

					+ "<br>" + "<br>"
					+ "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
				   + "</div>"

					+ "</div>"

					+ "<br>" + "<br>"
					+ "<div style='text-align: center'>"
					+ "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
					+ "</div>"

					+ "</div>"
					;
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

        //Investor details
        public bool SendInvestorDetails(Investor investor)
        {
            //using (MailMessage mm = new MailMessage(email, "tamilarasan@highbrowdiligence.com"))
            using (MailMessage mm = new MailMessage())
            {
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.To.Add(new MailAddress("tamilarasan@highbrowdiligence.com"));
                mm.To.Add(new MailAddress("support@highbrowdiligence.com"));
                mm.Subject = "Investor Details";
                mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                   + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                   + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"
                   + "<br>"
                   + "<div>"
                   + "<div>Dear <span style='font-weight:600'> HiSpace Team,</span></div>"

                   + "<div style='margin: 5px 0; font-weight:600;'>You have one new investor enquiry</div>"
                   + "<table style='border-collapse: collapse;border: 1px solid #a2a2a2;background: #fbe7c8;width: 100%'>"
                   + "<tboby>"
                   + "<tr>"
                   + "<th colspan='2' style='text-align:center;border: 1px solid #a2a2a2; padding: 10px;'>Investor Details</th>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Investor Id</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.InvestorId + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>FirstName</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.FirstName + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>LastName</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.LastName + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Email</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Email + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Phone</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Phone + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Investment Type</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.InvestmentType + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Property Type</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.PropertyType + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Currency</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Currency + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Min Range</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.MinRange + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Max Range</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.MaxRange + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>During</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.During + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Country</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Country + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>State</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.State + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>District/City</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.District + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Neighborhood</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Neighborhood + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>Comment</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.Comment + "</td>"
                   + "</tr>"
                   + "<tr>"
                   + "<th style='border: 1px solid #a2a2a2; padding: 10px;'>CreatedDateTime</th>"
                   + "<td style='border: 1px solid #a2a2a2; padding: 10px;'>" + investor.CreatedDateTime + "</td>"
                   + "</tr>"
                   + "</tbody>"
                   + "</table>"

                   + "<br>"
                   + "<br>"

                   + "<div>Sincerely,</div>"
                   + "<div style=''font-weight: 700;>HiSpace Team</div>"
                   + "</div>"

                   + "<br>" + "<br>"
                   + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                  + "</div>"

                   + "</div>"

                   + "<br>" + "<br>"
                   + "<div style='text-align: center'>"
                   + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                   + "</div>"

                   + "</div>"
                   ;
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

        //Investor success
        public bool SendInvestorSuccess(string FirstName, string LastName, string InvestorEmail)
        {
            using (MailMessage mm = new MailMessage(email, InvestorEmail))
            {
                mm.From = new MailAddress(email, "HiSpace Team");
                mm.Subject = "Success";
                mm.Body = "<div style='background: #2ecc71;padding: 50px 30px 15px 30px;'>"
                    + "<div style='width:75%; margin: 0 auto;padding: 30px;background: #fff;font-size: 14px;color: #505050;'>"

                    + "<div style='text-align:center;background:#fbe7c8;padding: 10px 10px 5px 10px;'><img src='http://www.thehispace.com/images/logo.png' alt='HiSpace Logo' style='height: 60px' /></div>"

                    + "<div>"
                    + "<div> <h1> <span style='font-weight:600'>Welcome to HiSpace!</span> </h1></div>"

                    + "<div>Dear <span style='font-weight:600'>" + FirstName+" "+LastName + ",</span></div>"
                    + "<div style='margin: 5px 0;'>We would like to acknowledge that we have received your request.</div>"
                    + "<div style='margin: 5px 0;'>A support representative will be reviewing your request and will send you a personal response (usually within 24 hours).</div>"
                    + "<div style='margin: 5px 0;'>Thank you for partnering with HiSpace.</div>"
                    + "<br>"
                    + "<br>"


                    + "<div>Sincerely,</div>"
                    + "<div style=''font-weight: 700;>HiSpace Team</div>"
                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='color:#999999;font-size:11px;text-align:center;'> ©2020 HiSpace | Plot No. 267 | 2nd Floor | 2nd Main Road | Nehru Nagar | Kandanchavadi | Chennai | 600096"
                   + "</div>"

                    + "</div>"

                    + "<br>" + "<br>"
                    + "<div style='text-align: center'>"
                    + "<a style='color:#000000;font-size:10px;text-decoration:none;' href='https://www.hdsre.com/' target='_blank'><span style='font-family:Helvetica Neue,Helvetica,Arial,Verdana,sans-serif;font-size:12px;opacity:0.75;color:#000000'>Powered by <em style='font-style:normal;text-decoration:underline;font-weight:bold'> Highbrow Diligence Services Limited</em>®</span></a>"
                    + "</div>"

                    + "</div>"
                    ;
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
