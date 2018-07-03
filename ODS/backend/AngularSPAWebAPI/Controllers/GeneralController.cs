using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models.DatabaseModels.General;
using Microsoft.AspNetCore.Identity;
using AngularSPAWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.AccessTokenValidation;
using AngularSPAWebAPI.Models.DatabaseModels.Users;
using Microsoft.AspNetCore.Cors;

namespace AngularSPAWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class GeneralController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GeneralController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signingmanager)
        {
            _context = context;
            _usermanager = usermanager;
            _signInManager = signingmanager;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _usermanager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var tempuser = new InfoUser
            {
                ID = user.Id,
                CreationDate = user.CreateDate,
                Name = user.Name,
                Email = user.Email
            };

            return Ok(tempuser);
        }

        [HttpGet("hascompany")]
        public async Task<IActionResult> HasCompany()
    {

      var user = await  _usermanager.GetUserAsync(User);

      var tempuser = await _context.Users.Where(i => i.Id == user.Id).Include(i => i.Company).SingleAsync();

      if(tempuser.Company != null)
      {
        return Ok(true);
      }

      else
      {
        return Ok(false);
      }

      
    }

        [HttpPost("registercompany")]
        public async Task<IActionResult> PostCompany([FromBody] Company company )
        {
            var user = await _usermanager.GetUserAsync(User);

            if(user == null)
            {
                return NotFound();
            }

            user.Company = company;

            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

            return Ok();

        }


      [HttpPost("signout")]
      public async Task<IActionResult> SignOut()
      {
        await _signInManager.SignOutAsync();
        return Ok();

      }



    }
}
