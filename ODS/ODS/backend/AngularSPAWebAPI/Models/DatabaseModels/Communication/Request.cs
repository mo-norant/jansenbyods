using AngularSPAWebAPI.Models.DatabaseModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart;

namespace AngularSPAWebAPI.Models.DatabaseModels.Communication
{
    public class Request
    {
    public int RequestID { get; set; }
    public string Name { get; set; }
    public Company Company { get; set; }
    public int OogstkaartID { get; set; }
    public string Status { get; set; }
    public DateTime Create { get; set; }
    public DateTime LastUpdatet { get; set; }
    public bool UserViewed { get; set; }
    public string Decision { get; set; }
    public ICollection<Message> Messages { get; set; }
    public OogstkaartItem Item { get; set; }
    
  }
}
