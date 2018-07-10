using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{
  public class EmailConfiguration : IEmailConfiguration
  {

    public EmailConfiguration()
    {
      this.SmtpPassword = "Catharina2018*";
      this.SmtpPort = 25;
      this.SmtpUsername = "info@jansenbyods.com";
      this.SmtpServer = "mail.jansenbyods.com";
     
    }


    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
  }

}
