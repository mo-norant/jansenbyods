using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{
    public interface IEmailService
  {
    Task Send(EmailMessage emailMessage);
    Task Send(EmailMessage emailMessage, BodyBuilder bodyBuilder);

  }
}
