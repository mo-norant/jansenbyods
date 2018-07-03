using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.General
{
    public class File
    {
    public int FileID { get; set; }
    public string URI { get; set; }
    public DateTime Create { get; set; }
    public string Name { get; set; }
    public string Omschrijving { get; set; }
  }
}
