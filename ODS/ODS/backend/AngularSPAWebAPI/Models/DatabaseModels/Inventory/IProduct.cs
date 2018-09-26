using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public interface IProduct
  {

    double CalculatePrice();
    
  }
}
