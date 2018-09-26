using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public class Product : IProduct
  {

    public Product()
    {
      Creation = DateTime.Now;
    }

    public int ProductID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Availability { get; set; }
    public DateTime Creation { get;}
    public ProductCategory ProductCategory { get; set; }
    public ICollection<BaseSubProduct> SubProducts { get; set; }

    public double CalculatePrice()
    {
      double total = 0;

      foreach (var pr in SubProducts)
      {
        total += pr.ProductPrice;
      }

      return total;
    }
  }
}
