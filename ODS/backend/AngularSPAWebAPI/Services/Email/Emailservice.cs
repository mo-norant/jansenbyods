using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services.Email
{
  public class EmailService : IEmailService
  {
    private readonly IEmailConfiguration _emailConfiguration;

    public EmailService(IEmailConfiguration emailConfiguration)
    {
      _emailConfiguration = emailConfiguration;
    }


    public void Send(EmailMessage emailMessage)
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
        emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

        emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
        emailClient.Send(message);
        emailClient.Disconnect(true);
      }
    }
  }
}
