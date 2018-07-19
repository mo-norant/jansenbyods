using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
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
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles ="user")]
  public class FaqController : Controller
    {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _context;

    public FaqController(
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      SignInManager<ApplicationUser> signInManager,
      ILogger<IdentityController> logger, ApplicationDbContext context)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _signInManager = signInManager;
      _logger = logger;
      _context = context;
    }

    [AllowAnonymous]
    [HttpGet("Question")]
    public async Task<IActionResult> GetQuestions()
    {
      var questions = await _context.QuestionCategories.Include( i => i.Questions).ToListAsync();
      return Ok(questions);
    }

  }
}
