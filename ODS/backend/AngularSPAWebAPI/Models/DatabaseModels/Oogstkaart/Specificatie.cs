using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart
{
    public class Specificatie
    {
    public int SpecificatieID { get; set; }
    public string SpecificatieSleutel { get; set; }
    public string SpecificatieValue { get; set; }
    public string SpecificatieEenheid { get; set; }
    public string SpecificatieOmschrijving { get; set; }
  }
}
