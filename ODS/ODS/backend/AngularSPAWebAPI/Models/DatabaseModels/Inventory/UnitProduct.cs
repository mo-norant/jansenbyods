using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public class UnitProduct : BaseSubProduct
  {
    //vb 6m/lengte prijs is per lengte
    public int UnitValue { get; set; }
    public string UnitMetric { get; set; }
    public string UnitCall { get; set; }

    public override double CalculatePrice()
    {
      return ProductPrice;
    }
  }
}
