using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.DatabaseModels.Communication;
using AngularSPAWebAPI.Models.DatabaseModels.General;
using AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = AngularSPAWebAPI.Models.DatabaseModels.General.File;


namespace AngularSPAWebAPI.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme,
   Policy = "Access Resources")]
  public class StatisticsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IHostingEnvironment _env;
    private readonly ILogger _logger;
    private readonly RoleManager<IdentityRole> _rolemanager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _usermanager;
    public StatisticsController(
      UserManager<ApplicationUser> usermanager,
      RoleManager<IdentityRole> rolemanager,
      SignInManager<ApplicationUser> signInManager,
      IHostingEnvironment appEnvironment,
      ILogger<IdentityController> logger,
      ApplicationDbContext context)
    {
      this._usermanager = usermanager;
      this._rolemanager = rolemanager;
      _signInManager = signInManager;
      _env = appEnvironment;
      _logger = logger;
      this._context = context;
    }

    [HttpGet("mostviewedproduct")]
    public async Task<IActionResult> GetMostViewedProduct()
    {
      var user = await _usermanager.GetUserAsync(User);

      if (user == null)
      {
        return NotFound("user niet gevonden.");
      }

      var oogstkaart = await _context.OogstkaartItems.Where(i => i.UserID == user.Id).Include(i =>  i.Views).MaxAsync(i => i.Views.Count);

      return Ok(oogstkaart);

    }


    [HttpGet("requestinreview")]
    public async Task<IActionResult> UnreadRequests()
    {
      var user = await _usermanager.GetUserAsync(User);

      if (user == null)
      {
        return NotFound("user niet gevonden.");
      }

      var items = await _context.OogstkaartItems.Where(i => i.UserID == user.Id).Include(i => i.Views).Include(i => i.Requests).ToListAsync();

      List<Request> requests = new List<Request>();

      int count = 0;

      foreach (var item in items)
      {
        requests.AddRange(item.Requests);
      }

      foreach (var req in requests)
      {
        if (req.Status == "tobereviewed")
        {
          count++;
        }
      }
      return Ok(count);
    }


    [HttpGet("reviewaccepted")]
    public async Task<IActionResult> RequestAccepted()
    {
      var user = await _usermanager.GetUserAsync(User);

      if (user == null)
      {
        return NotFound("user niet gevonden.");
      }

      var items = await _context.OogstkaartItems.Where(i => i.UserID == user.Id).Include(i => i.Views).Include(i => i.Requests).ToListAsync();

      List<Request> requests = new List<Request>();

      int count = 0;

      foreach (var item in items)
      {
        requests.AddRange(item.Requests);
      }

      foreach (var req in requests)
      {
        if (req.Status == "accepted")
        {
          count++;
        }
      }
      return Ok(count);
    }
  }

}
