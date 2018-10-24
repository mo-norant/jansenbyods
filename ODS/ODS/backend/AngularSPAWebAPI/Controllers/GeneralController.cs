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
using AngularSPAWebAPI.Services;

namespace AngularSPAWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Roles = "administrator, user")]

  public class GeneralController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;

        public GeneralController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signingmanager, IEmailService emailService
          )
        {
            _context = context;
            _usermanager = usermanager;
            _signInManager = signingmanager;
            _emailService = emailService;
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

    [AllowAnonymous]
    [HttpPost("checkemail/{username}")]
    public async Task<IActionResult> Login([FromRoute] string username)
    {

      if (ModelState.IsValid)
      {
        var user = await _usermanager.FindByEmailAsync(username);

        if(user == null)
        {
          return NotFound();
        }

        if (user.EmailConfirmed)
        {

          //check of gebruiker niet gelocked is

          if (user.LockoutEnabled)
          {
            return NotFound(new { message = "Uw account werd geblokkeerd. Contacteer Jansen By ODS om uw account terug te activeren.", code = "lockout" });

          }

          return Ok();
        }


        // check if gebruiker email heeft bevestigd.
        string token = await _usermanager.
              GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = Url.Action("ConfirmEmail", "Identity", new { userId = user.Id, code = token }, protocol: HttpContext.Request.Scheme);

        var message = new EmailMessage();
        message.FromAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = "info@jansenbyods.com" });
        message.ToAddresses.Add(new EmailAddress { Name = user.UserName, Address = user.Email });

        message.Subject = "Bevestig uw JansenByODS-account.";
        message.Content = string.Format("<a href='{0}'>Gelieve deze link te bevestigen om uw account te activeren.</a>", callbackUrl);

        await _emailService.Send(message);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return NotFound(new { message = "uw account werd nog niet geactiveerd. Ga naar uw email om uw account te activeren.", code = "emailactivation" });


      }

      return BadRequest();
    }

    [HttpGet("username")]
    [AllowAnonymous]
    public async Task<IActionResult> SignOut([FromQuery] string usermail )
    {
      if (ModelState.IsValid)
      {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == usermail);
        if(user == null)
        {
          return Ok(false);
        }
        return Ok(true);
      }

      return BadRequest();
    }


    [HttpPost("signout")]
      public async Task<IActionResult> SignOut()
      {
        await _signInManager.SignOutAsync();
        return Ok();

      }
    }
}
