using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.General
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
      public string Email { get; set; }
    }
}
