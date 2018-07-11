using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.AccountViewModels;
using AngularSPAWebAPI.Services;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

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
    private readonly IEmailService _emailService;
        public IdentityController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<IdentityController> logger, ApplicationDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _emailService = emailService;
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
    [Authorize(Roles = "administrator, user")]
    [HttpPost("CheckMail")]
    public async Task<IActionResult> CheckEmail()
    {
      var user = await _userManager.GetUserAsync(User);
      if(user != null)
      {
        if (user.EmailConfirmed)
        {
          return Ok(false);
        }
        return Ok(true);
      }
      return BadRequest();
    
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

      if (ModelState.IsValid)
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
                await AddToRole(model.email, "user");
                await AddClaims(model.email);

          string token = await _userManager.
               GenerateEmailConfirmationTokenAsync(user);

          var callbackUrl = Url.Action("ConfirmEmail", "Identity", new { userId = user.Id, code = token }, protocol: HttpContext.Request.Scheme);

          var message = new EmailMessage();
          message.FromAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = "info@jansenbyods.com" });
          message.ToAddresses.Add(new EmailAddress{ Name = model.name, Address = model.email });

          message.Subject = "Bevestig uw JansenByODS-account.";
          message.Content = string.Format("<a href='{0}'>Gelieve deze link te bevestigen om uw account te activeren.</a>", callbackUrl);

          await _emailService.Send(message);
          _logger.LogInformation(3, "User created a new account with password.");
          await _signInManager.SignInAsync(user, isPersistent: false);
          return Ok();
            }
          return BadRequest(result);
      }
      return BadRequest();

    }

    [AllowAnonymous]
    [HttpPost("forgotpassword")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
      if (string.IsNullOrEmpty(email))
        return NotFound("email niet gevonden");

      var user = await _userManager.FindByEmailAsync(email);

      if(user == null)
      {
        return NotFound("niet gevonden");

      }

      if (!await _userManager.IsEmailConfirmedAsync(user))
        return NotFound("Gebruiker heeft email nog niet bevestigd");


      var confrimationCode = await _userManager.GeneratePasswordResetTokenAsync(user);
      byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(confrimationCode);
      var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
      var link = Url.Action("ResetPassword", "Identity", new { email = email, code = codeEncoded }, protocol: HttpContext.Request.Scheme);


      var message = new EmailMessage();
      message.FromAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = "info@jansenbyods.com" });
      message.ToAddresses.Add(new EmailAddress { Name = email, Address =email });

      message.Subject = "Reset uw wachtwoord van uw JansenByODS-account.";
      message.Content = string.Format("<a href='{2}/login/resetpassword?userid={0}&code={1}'>Gelieve deze link te bevestigen om uw account te activeren.</a>", user.Id, codeEncoded, "http://admin.jansenbyods.com");

      await _emailService.Send(message);

      return Ok();
    }

    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromQuery] string userid, [FromQuery] string code , [FromBody] ResetPasswordView view)
    {
      if (ModelState.IsValid)
      {
        if (userid == null || code == null)
        {
          return BadRequest("Params zijn leeg.");
        }

        var user = await _userManager.FindByIdAsync(userid);
        var codeDecodedBytes = WebEncoders.Base64UrlDecode(code);
        var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
        
        if (user == null)
        {
          return NotFound("user not found");
        }

        if(view.password != view.password2)
        {
          return BadRequest("wachtwoorden zijn niet gelijk.");
        }

        var result = await _userManager.ResetPasswordAsync(user, codeDecoded, view.password);

        if (result.Succeeded)
        {
          var message = new EmailMessage();
          message.FromAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = "info@jansenbyods.com" });
          message.ToAddresses.Add(new EmailAddress { Name = user.UserName, Address = user.Email });

          message.Subject = "Uw wachtwoord van uw Jansen By ODS-account werd gereset.";
          message.Content = String.Format("Uw wachtwoord van uw Jansen By ODS-account werd op {0} gewijzigd. ", DateTime.Now.ToString());

          await _emailService.Send(message);
          return Ok();
        }

      }



      return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("Usernameused")]
    public async Task<IActionResult> UsernameUsed([FromQuery] string username)
    {
      var user = await _context.Users.FirstOrDefaultAsync(i => i.UserName == username);
      if(user == null)
      {
        return Ok(true);
      }
      return Ok(false);
    }


    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
      if (userId == null || code == null)
        return BadRequest();

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        throw new ApplicationException($"Unable to load user with ID '{userId}'.");

      var result = await _userManager.ConfirmEmailAsync(user, code);
      if (result.Succeeded)
      {
        return Redirect("http://admin.jansenbyods.com/login/registersucces");
      }

     return BadRequest();

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



    private async Task AddToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task AddClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var claims = new List<Claim> {
                new Claim(type: JwtClaimTypes.Name, value: user.UserName)
            };
            await _userManager.AddClaimsAsync(user, claims);
        }
    }
}
