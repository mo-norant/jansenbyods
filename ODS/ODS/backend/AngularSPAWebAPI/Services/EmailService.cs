using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{
    public class EmailService: IEmailService
    {

    private readonly IEmailConfiguration _emailConfiguration;

    public EmailService(IEmailConfiguration emailConfiguration)
    {
      _emailConfiguration = emailConfiguration;
    }



  
   

    public async Task Send(EmailMessage emailMessage)
    {
      var message = new MimeMessage();
      message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
      message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

      message.Subject = emailMessage.Subject;
      message.Body = new TextPart(TextFormat.Html)
      {
        Text = emailMessage.Content
      };

      using (var emailClient = new SmtpClient())
      {
        emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.Auto);
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
        await emailClient.SendAsync(message);
        await emailClient.DisconnectAsync(true);
      }
    }

    public async Task Send(EmailMessage emailMessage, BodyBuilder bodyBuilder)
    {
      var message = new MimeMessage();
      message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
      message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

      message.Subject = emailMessage.Subject;
      bodyBuilder.HtmlBody = emailMessage.Content;

      message.Body = message.Body = bodyBuilder.ToMessageBody();


      using (var emailClient = new SmtpClient())
      {
        emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.Auto);
        emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
        await emailClient.SendAsync(message);
        await emailClient.DisconnectAsync(true);
      }
    }
  }
}
