using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.DatabaseModels.General;
using AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart;
using AngularSPAWebAPI.Models.DatabaseModels.Communication;
using AngularSPAWebAPI.Models.DatabaseModels.Faq;
using AngularSPAWebAPI.Models.DatabaseModels.Inventory;

namespace AngularSPAWebAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Weight> Weights { get; set; }
        public DbSet<OogstkaartItem> OogstkaartItems { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Specificatie> Specificaties { get; set; }
        public DbSet<Afbeelding> Afbeeldingen { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionCategory> QuestionCategories { get; set; }
        public DbSet<View> Views { get; set; }
    public DbSet<BaseSubProduct> SubProducts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> productCategories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
      builder.Entity<SingleProduct>().HasBaseType<BaseSubProduct>();
      builder.Entity<UnitProduct>().HasBaseType<BaseSubProduct>();

    }
  }
}
