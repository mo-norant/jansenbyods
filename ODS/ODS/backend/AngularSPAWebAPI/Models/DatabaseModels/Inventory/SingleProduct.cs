using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public class SingleProduct : BaseSubProduct
  {
    public override double CalculatePrice()
    {
      return ProductPrice;
    }
  }
}
