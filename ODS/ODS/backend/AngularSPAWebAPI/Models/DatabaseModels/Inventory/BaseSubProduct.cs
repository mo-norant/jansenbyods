using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public abstract class BaseSubProduct : IProduct
  {
    [Key]
    public int ProductID { get; set; }
    public double ProductPrice { get; set; }
    public string Name { get; set; }
    public DateTime Creation { get;}
    public string Description { get; set; }
    public ProductCategory ProductCategory { get; set; }

    public BaseSubProduct()
    {
      Creation = DateTime.Now;
    }

    public abstract double CalculatePrice();
  }
}
