using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart
{
    public class Location
    {
        public int LocationID { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }
}
