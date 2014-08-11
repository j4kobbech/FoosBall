namespace FoosBall.Main
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;
    using SendGrid;

    public static class Email
    {
        public const string DefaultFromAddress = "foosball@trustpilot.com";

        public static void Send(
            string recipient, 
            string subject, 
            string textBody, 
            string htmlBody,
            string userName = null,
            string password = null,
            string fromAddress = DefaultFromAddress)
        {
            try
            {
                // Credentials
                var user = userName ?? ConfigurationManager.AppSettings["SENDGRID_USERNAME"];
                var pw = password ?? ConfigurationManager.AppSettings["SENDGRID_PASSWORD"];

                // Create the message
                var myMessage = new SendGridMessage();
                myMessage.AddTo(recipient);
                myMessage.From = new MailAddress(fromAddress);
                myMessage.Subject = subject;
                myMessage.Text = textBody;
                myMessage.Html = htmlBody;
                myMessage.EnableTemplateEngine("63b53010-b6a1-4c7c-99fd-95557791407e");

                // SEND THE MESSAGE
                var credentials = new NetworkCredential(user, pw);
                
                // Create a Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                transportWeb.Deliver(myMessage);
            }
            catch (Exception ex)
            {
                //Nothing happens
            }
        }
    }
}