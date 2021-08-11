using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TMSDeloitte.Helper
{
    public class Email
    {

        public bool SendEmail(string ToEmail, string body, string Subject, string cc,out string msg)
        {
            msg = "";
            try
            {
                //MailTo = ConfigurationSettings.AppSettings["MailTo"].ToString();

                string to = "", bcc = "", subject = "", ReplyTo = "", attachment = "";

                to = ToEmail;

                using (MailMessage message = new MailMessage())
                {
                    string[] Add = to.ToString().Split(',');
                    message.From = new MailAddress(ConfigurationSettings.AppSettings["MailFrom"].ToString(), ConfigurationSettings.AppSettings["MailFromName"].ToString());
                    if (Add.Length > 1)
                    {
                        for (int a = 0; a < Add.Length; a++)
                        {
                            message.To.Add(new MailAddress(Add[a].ToString()));
                        }
                    }
                    else { message.To.Add(new MailAddress(to)); }
                    if (cc != null && cc != "")
                    {
                        message.CC.Add(new MailAddress(cc));
                    }
                    if (bcc != null && bcc != "")
                    {
                        message.Bcc.Add(new MailAddress(bcc));
                    }
                    if (ReplyTo != null && ReplyTo != "")
                    {
                        message.ReplyToList.Add(new MailAddress(ReplyTo));
                    }
                    message.IsBodyHtml = true;


                    message.Subject = Subject;
                    message.Body = body;



                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.UseDefaultCredentials = true;
                        smtp.Host = ConfigurationSettings.AppSettings["SmtpServer"].ToString();


                        if (bool.Parse(ConfigurationSettings.AppSettings["UseCredentials"]))
                        {
                            string EmailPass = (ConfigurationSettings.AppSettings["Password"].ToString());
                            smtp.Credentials = new NetworkCredential(ConfigurationSettings.AppSettings["UserEmail"].ToString(), EmailPass);

                        }
                        else
                        {
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;

                        }
                        smtp.Port = Convert.ToInt32(ConfigurationSettings.AppSettings["Port"].ToString());
                        smtp.EnableSsl = Convert.ToBoolean(ConfigurationSettings.AppSettings["isSSL"].ToString());


                        try
                        {
                            smtp.Send(message);
                        }
                        catch (Exception ex)
                        {
                            msg = "Exception occured."+ ex.Message;
                            Log log = new Log();
                            string msgLog = ("Exception occured on sending email,  email:  " + ToEmail + Environment.NewLine + "Exception: " + ex.Message);
                            log.LogFile(msgLog);
                            log.InputOutputEmailDocLog(subject, msgLog);
                            return false;
                        }
                    }
                   
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = "Exception occured." + ex.Message;
                Log log = new Log();
                string msgLog = ("Exception occured on sending email,  email:  " + ToEmail + Environment.NewLine + "Exception: " + ex.Message);
                log.LogFile(msgLog);
                log.InputOutputEmailDocLog(Subject, msgLog);
                return false;
            }
        }

    }
}