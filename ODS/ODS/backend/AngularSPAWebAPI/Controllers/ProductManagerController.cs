using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.DatabaseModels.Inventory;
using AngularSPAWebAPI.Models.DatabaseModels.Inventory.PostModels;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AngularSPAWebAPI.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Manage Accounts")]
  public class ProductManagerController : Controller
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ProductManagerController(
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<IdentityController> logger, ApplicationDbContext context, IEmailService emailService)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _signInManager = signInManager;
      _logger = logger;
      _context = context;
      _emailService = emailService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> TestType()
    {

      if (ModelState.IsValid)
      {
        var products = await _context.SubProducts.OfType<UnitProduct>().ToListAsync();

        return Ok(products[0].CalculatePrice());
      }

      return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("Productcategory")]
    public async Task<IActionResult> GetProductcategory()
    {
      if (ModelState.IsValid)
      {
        List<ProductCategory> pcs = await _context.productCategories.ToListAsync();
        return Ok(pcs);
      }


      return BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("Productcategory")]
    public async Task<IActionResult> Productcategory([FromBody] ProductCategory pc)
    {
      if (ModelState.IsValid)
      {
        await _context.productCategories.AddAsync(pc);
        await _context.SaveChangesAsync();
        return Ok(pc);
      }


      return BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("Product/{type}")]
    public async Task<IActionResult> PostProduct([FromBody] CreateSubProduct subproduct, [FromRoute] string type)
    {

      if (ModelState.IsValid)
      {
        ProductCategory pc;
        BaseSubProduct sub;
      

        if (type == "unit")
        {
          sub = new UnitProduct()
          {
            ProductPrice = subproduct.ProductPrice,
            Name = subproduct.Name,
            Description = subproduct.Description,
            UnitCall = subproduct.UnitCall,
            UnitMetric = subproduct.UnitMetric,
            UnitValue = subproduct.UnitValue

          };
          
        }
        else
        {
          sub = new SingleProduct
          {
            ProductPrice = subproduct.ProductPrice,
            Name = subproduct.Name,
            Description = subproduct.Description

          };

        }

        if (subproduct.ProductCategoryID != 0)
        {
          pc = await _context.productCategories.FirstOrDefaultAsync(i => i.ProductCategoryID == subproduct.ProductCategoryID);
          sub.ProductCategory = pc;
        }

        await _context.SubProducts.AddAsync(sub);
        await _context.SaveChangesAsync();
        return Ok();
      }
      return BadRequest();

    
    }


  }


}
