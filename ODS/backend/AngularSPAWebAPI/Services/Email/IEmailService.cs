using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services.Email
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
  }
}
