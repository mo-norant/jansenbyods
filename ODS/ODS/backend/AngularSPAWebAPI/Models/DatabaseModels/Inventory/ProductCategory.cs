using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Inventory
{
  public class ProductCategory
  {
    public ProductCategory()
    {
      Creation = DateTime.Now;
    }

    public int ProductCategoryID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Creation { get; set; }
  }
}
