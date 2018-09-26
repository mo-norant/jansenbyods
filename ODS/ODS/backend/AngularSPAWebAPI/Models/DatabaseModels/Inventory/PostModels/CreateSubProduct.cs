using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory.PostModels
{
  public class CreateSubProduct
  {
    public int ProductID { get; set; }
    public double ProductPrice { get; set; }
    public int ProductCategoryID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Creation { get; }

    //vb 6m/lengte prijs is per lengte
    //vb Unitvalue/Unitmetric is Unitcall
    public int UnitValue { get; set; }
    public string UnitMetric { get; set; }
    public string UnitCall { get; set; }

    public CreateSubProduct()
    {
      Creation = DateTime.Now;
    }

  }
}
