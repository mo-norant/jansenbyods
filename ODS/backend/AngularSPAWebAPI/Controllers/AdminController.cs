using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Models.DatabaseModels.Communication;
using AngularSPAWebAPI.Models.DatabaseModels.Oogstkaart;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AngularSPAWebAPI.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Manage Accounts")]

  public class AdminController : Controller
  {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _context;

    public AdminController(
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





    [HttpGet("requests/{id}")]
    public async Task<IActionResult> GetItem([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);

      if (user == null)
      {
        return NotFound();
      }

      var request = await _context.Requests.Where(i => i.RequestID == id).Include(i => i.Messages).Include(i => i.Company).ThenInclude(i => i.Address).FirstOrDefaultAsync();


      if (request == null)
      {
        return NotFound();
      }


      return Json(request);
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequests([FromRoute] int requestId)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var request = await _context.Requests.FirstOrDefaultAsync(i => i.RequestID == requestId);
      return Ok(request);

    }

    [HttpPost("requests/update/{id}")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromQuery]string status)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var request = await _context.Requests.FirstOrDefaultAsync(i => i.RequestID == id);
      request.Status = status;
      await _context.SaveChangesAsync();

      if (status == "accepted")
      {
        await SendMessage("info@jansenbyods.com", "mo.bouzim@live.be", "Er werd een vraag voor uw product goedgekeurd.", "Er werd een aanvraag voor uw product goedgekeurd");
      }

      return Ok();

    }
    
    [HttpGet("oogstkaart")]
    public async Task<IActionResult> GetOogstkaarProducten()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var items = await _context.OogstkaartItems.Include(i => i.Specificaties).Include(i => i.Gallery).Include(i => i.Files).Include(i => i.Avatar).Include(i => i.Location).ToListAsync();
      return Ok(items);

    }

    [HttpGet("oogstkaart/{id}")]
    public async Task<IActionResult> GetOogstkaarProducten([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Include(i => i.Specificaties)
          .Include(i => i.Gallery).Include(i => i.Files).Include(i => i.Avatar).Include(i => i.Location)
          .FirstOrDefaultAsync();
      if (item != null) return Ok(item);
      return NotFound();

    }

    [HttpPost("delete/oogstkaart/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var item = await _context.OogstkaartItems.FirstOrDefaultAsync(i => i.OogstkaartItemID == id);
      if (item == null) return NotFound();
      _context.Remove(item);
      await _context.SaveChangesAsync();
      return Ok();
    }


    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] string status)
    {

      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();

      if (status == "tobereviewed")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).Include(ic => ic.Company).ToListAsync();
        return Ok(requests);
      }
      else if (status == "declined")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).Include(ic => ic.Company).ToListAsync();
        return Ok(requests);
      }
      else if (status == "accepted")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).Include(ic => ic.Company).ToListAsync();
        return Ok(requests);
      }
      else
      {
        var allitems = await _context.Requests.Include(ir => ir.Messages).Include(ic => ic.Company).ToListAsync();
        return Ok(allitems);
      }
    }


    
    [HttpGet("requestsfromperiod")]
    public async Task<IActionResult> RequestsfromPeriod([FromQuery] string period)
    {

      if(period == "today")
      {
        var requests = await _context.Requests.Where(i => i.Create == DateTime.Today).Include(i => i.Messages).Include(i => i.Company).Take(5).ToListAsync();
        return Ok(DateTime.Today.ToShortDateString());
      }
      else if (period == "week")
      {
         var requests = await _context.Requests.Where(i => AreFallingInSameWeek(i.Create, DateTime.Today)).Include(i => i.Messages).Include(i => i.Company).Take(5).ToListAsync();
         return Ok(requests);
      }
      else if (period == "month")
      {
        var requests = await _context.Requests.Where(i => i.Create.Month == DateTime.Today.Month).OrderBy(i => i.Create).Include(i => i.Messages).Include(i => i.Company).Take(5).ToListAsync();
        return Ok(requests);
      }

      else
      {
        return BadRequest();
      }

    }
    
    private bool AreFallingInSameWeek(DateTime date1, DateTime date2)
    {
      return date1.AddDays(-(int)date1.DayOfWeek) == date2.AddDays(-(int)date2.DayOfWeek);
    }

    private async Task SendMessage(string sender, string receiver, string subject, string messagebody)
    {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(sender, sender));
      message.To.Add(new MailboxAddress(receiver, receiver));
      message.Subject = subject;

      message.Body = new TextPart("plain")
      {
        Text = messagebody
      };

      using (var client = new SmtpClient())
      {
        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await client.ConnectAsync("mail.jansenbyods.com", 25, false);

        // Note: only needed if the SMTP server requires authentication
        await client.AuthenticateAsync("info@jansenbyods.com", "Catharina2018*");

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
      }
    }


  }
}



