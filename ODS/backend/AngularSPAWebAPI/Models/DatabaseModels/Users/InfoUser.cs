using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Users
{
    public class InfoUser
    {
        public string ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
