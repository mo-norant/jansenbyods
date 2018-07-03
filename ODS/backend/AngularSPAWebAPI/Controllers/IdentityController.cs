using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.AccountViewModels;
using AngularSPAWebAPI.Models.DatabaseModels.General;
using AngularSPAWebAPI.Services;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AngularSPAWebAPI.Models.DatabaseModels.Users;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Identity Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Manage Accounts")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<IdentityController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            this._context = context;
        }

        /// <summary>
        /// Gets all the users.
        /// </summary>
        /// <returns>Returns all the users</returns>
        // GET api/identity/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var role = await _roleManager.FindByNameAsync("user");
            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            return new JsonResult(users);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Create
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]CreateViewModel model)
        {

            var user = new ApplicationUser
            {
                AccessFailedCount = 0,
                Email = model.email,
                EmailConfirmed = false,
                LockoutEnabled = false,
                NormalizedEmail = model.email.ToUpper(),
                NormalizedUserName = model.email.ToUpper(),
                TwoFactorEnabled = false,
                UserName = model.email,
                CreateDate = DateTime.Now,
                Name = model.name,
              Company = model.Company
                
                
            };

            if(model.password != model.password2)
            {
                return BadRequest("passwords are not alike");
            }

            var result = await _userManager.CreateAsync(user, model.password);

            if (result.Succeeded)
            {
                await addToRole(model.email, "user");
                await addClaims(model.email);
                return new JsonResult(user.Id);
            }

            return BadRequest(result);

            
        }
       

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Delete
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.DeleteAsync(user);

            return new JsonResult(result);
        }


    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {

      var users = await _context.Users.ToListAsync();

      return Ok(users);
    }



    private async Task addToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task addClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = new List<Claim> {
                new Claim(type: JwtClaimTypes.Name, value: user.UserName)
            };
            await _userManager.AddClaimsAsync(user, claims);
        }
    }
}
