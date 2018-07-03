using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.General
{
    public class Afbeelding
    {
        public int AfbeeldingID { get; set; }
        public string URI { get; set; }
        public DateTime Create { get; set; }
        public string Name { get; set; }
        public string Omschrijving { get; set; }
  }
}
