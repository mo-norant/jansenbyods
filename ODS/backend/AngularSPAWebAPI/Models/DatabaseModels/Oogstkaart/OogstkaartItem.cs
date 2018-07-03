using AngularSPAWebAPI.Models.DatabaseModels.Communication;
using AngularSPAWebAPI.Models.DatabaseModels.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart
{
    public class OogstkaartItem
    {
        public int OogstkaartItemID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Omschrijving { get; set; }
        public string Artikelnaam { get; set; }
        public Location Location { get; set; }
        public string Jansenserie { get; set; }
        public string Category { get; set; }
        public DateTime DatumBeschikbaar { get; set; }
        public int Hoeveelheid { get; set; }
        public float VraagPrijsPerEenheid { get; set; }
        public float VraagPrijsTotaal { get; set; }
        public string Concept { get; set; }
        public bool OnlineStatus { get; set; }
        public bool TransportInbegrepen { get; set; }
        public string UserID { get; set; }
        public int Views { get; set; }
        public Afbeelding Avatar { get; set; }
        public ICollection<Specificatie> Specificaties { get; set; }
        public ICollection<Afbeelding> Gallery { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Request> Requests { get; set; }
        public DateTime LastUpdatet { get; set; }
  }
}
