using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Communication
{
    public class Message
    {
      public int MessageID { get; set; }
      public DateTime Created { get; set; }
      public string MessageString { get; set; }
      public DateTime LastUpdatet { get; set; }
      public bool Opened { get; set; }
    }
}
