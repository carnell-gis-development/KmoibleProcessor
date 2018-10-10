using System;
using KMobileProcessor.Data;
using System.Net.Mail;

namespace KMobileProcessor.Controller
{
    public class MailSender
    {
        public static void SendNotification(string jobname)
        {
            string carnellSMTP = Global.SmtpServer;
            SmtpClient SmtpServer = new SmtpClient(carnellSMTP);
            string time = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Global.SmtpFromTo)
            };

            //mail.To.Add(Global.user1);
            //mail.To.Add(Global.user2);
            mail.To.Add(Global.Hao);
            mail.Subject = "K-mobile Automation Email - Status: Ready for QA";
            mail.Body = $"<h3>KMobile - This is an automated email, please do not reply.</h3><p>Your k-mobile job has been successfully processed and need you to implement data QA in database server.</p><p> Date: {time} </p><p> Job Number: {jobname} </p><p> Thank you!</p> ";
            mail.IsBodyHtml = true;

            SmtpServer.Port = 25;
            SmtpServer.Credentials = new System.Net.NetworkCredential("hao.ye@carnellgroup.co.uk", "Carnell2020");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
    }
}
