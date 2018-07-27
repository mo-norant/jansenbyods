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
    private readonly IEmailService _emailService;

    public AdminController(
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

    [HttpPost("sold/{id}")]
    public async Task<IActionResult> OogstkaartitemSold([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(o => o.OogstkaartItemID == id)
          .FirstOrDefaultAsync();

        if (item != null)
        {
          item.Sold = !item.Sold;
          await _context.SaveChangesAsync();


          return Ok(item.Sold);
        }
      }

      return BadRequest();
    }

    [HttpPost("requests/delete/{id}")]
    public async Task<IActionResult> DeleteRequest([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var request = await _context.Requests.Include(i => i.Messages).FirstOrDefaultAsync(i => i.RequestID == id);

      if(request != null)
      {
        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();
        return Ok();
      }

      return BadRequest();
    }


    [HttpPost("requests/update/{id}")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromQuery]string status, [FromQuery]string decision)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var request = await _context.Requests.FirstOrDefaultAsync(i => i.RequestID == id);

      var item = await _context.OogstkaartItems.Where(o => o.OogstkaartItemID == request.OogstkaartID).Include(i => i.Views).FirstOrDefaultAsync();
      var emailuser = await _context.Users.FirstOrDefaultAsync(i => item.UserID == i.Id);

      request.Decision = decision;

      request.Status = status;
      await _context.SaveChangesAsync();

        //mail naar klant sturen dat status over een bepaald product werd goedgekeurd.
        var message = new EmailMessage();
        message.Subject = String.Format("Er werd een status voor een aanvraag van uw product ('{0}') gewijzigd naar '{1}' .", item.Artikelnaam, request.Status);
        message.Content = string.Format("Geachte, " +
          "U hebt een aanvraag voor product ('{0}') dat naar status <strong>{1}</strong> is gewijzigd.{2} Als uw product niet werd goedgekeurd, gelieve ons dan te contacteren. " +
          "{2}met vriendelijke groeten," +
          "{2}Jansen By ODS", item.Artikelnaam, status , System.Environment.NewLine);

        message.ToAddresses.Add(new EmailAddress { Name = request.Name, Address = emailuser.Email });
        message.FromAddresses.Add(new EmailAddress { Name = "Jansen By ODS", Address = "info@jansenbyods.com" });

        await _emailService.Send(message);
        return Ok(request.Status);
      }

      
    
    [HttpGet("oogstkaart")]
    public async Task<IActionResult> GetOogstkaartProducten()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var items = await _context.OogstkaartItems.Include(i => i.Specificaties).Include(i => i.Gallery).Include(i => i.Views).Include(i => i.Files).Include(i => i.Avatar).Where( i => i.Avatar != null).Include(i => i.Location).ToListAsync();
      return Ok(items);

    }

    [HttpGet("oogstkaart/{id}")]
    public async Task<IActionResult> GetOogstkaartProducten([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();
      var product = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Include(i => i.Views).Include(i => i.Avatar).Include(i => i.Files).Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Specificaties).FirstOrDefaultAsync();
      if (product == null)
      {
        return NotFound("Product niet gevonden");
      }


      return Ok(product);

    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] OogstkaartItem updatingitem)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == updatingitem.OogstkaartItemID)
         .Include(i => i.Location).Include(i => i.Views).Include(i => i.Specificaties).FirstOrDefaultAsync();

        if (item == null) return NotFound();

        var owner = await _context.Users.FirstOrDefaultAsync(i => i.Id == item.UserID);

        item.Artikelnaam = updatingitem.Artikelnaam;
        item.Category = updatingitem.Category;
        item.Concept = updatingitem.Concept;
        item.CreateDate = updatingitem.CreateDate;
        item.DatumBeschikbaar = updatingitem.DatumBeschikbaar;
        item.Hoeveelheid = updatingitem.Hoeveelheid;
        item.Jansenserie = updatingitem.Jansenserie;

        item.Omschrijving = updatingitem.Omschrijving;
        item.VraagPrijsPerEenheid = updatingitem.VraagPrijsPerEenheid;
        item.VraagPrijsTotaal = updatingitem.VraagPrijsTotaal;

        item.Location.Latitude = updatingitem.Location.Latitude;
        item.Location.Longtitude = updatingitem.Location.Longtitude;

        //bekijk alle specificaties dat wel in de lijst voorkomen en update die

        foreach (var tempspec in updatingitem.Specificaties)
        {
          var spec = await _context.Specificaties.FirstOrDefaultAsync(i => i.SpecificatieID == tempspec.SpecificatieID);

          if (spec == null)
          {
            item.Specificaties.Add(tempspec);

          }
          else
          {
            spec.SpecificatieSleutel = tempspec.SpecificatieSleutel;
            spec.SpecificatieEenheid = tempspec.SpecificatieEenheid;
            spec.SpecificatieValue = tempspec.SpecificatieValue;
            spec.SpecificatieOmschrijving = tempspec.SpecificatieOmschrijving;
          }


        }


        foreach (var spec in item.Specificaties)
        {
          var sp = updatingitem.Specificaties.FirstOrDefault(i => i.SpecificatieID == spec.SpecificatieID);
          if (sp == null)
          {
            _context.Specificaties.Remove(spec);
          }
        }

        var specs = item.Specificaties.Where(i => i.SpecificatieValue == null || i.SpecificatieSleutel == null).ToList();
        foreach (var sp in specs)
        {
          item.Specificaties.Remove(sp);
        }
       

        _context.OogstkaartItems.Update(item);

        EmailMessage mail = new EmailMessage();
        mail.FromAddresses.Add(new EmailAddress { Address = "info@jansenbyods.com" });
        mail.Subject = string.Format("Product {0}  werd aangepast op de oogstkaart op {1} ", item.Artikelnaam, DateTime.Now.ToShortDateString());
        mail.ToAddresses.Add(new EmailAddress { Address = owner.Email });
        mail.Content = string.Format("De administator paste je product ('{0}') op de oogstkaart. <a href='http://jansenbyods.com/oogstkaart/{1}'>Bekijk het aangepaste product.</a>", item.Artikelnaam, item.OogstkaartItemID.ToString());
        await _emailService.Send(mail);

        await _context.SaveChangesAsync();


        return Ok(item);
      }

      return BadRequest();
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var user = await _userManager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id)
          .Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Avatar).Include(i => i.Files).Include(i => i.Requests).ThenInclude(i => i.Messages).Include(i => i.Specificaties).SingleAsync();


        var message = new EmailMessage();
        message.FromAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = "info@jansenbyods.com" });
        message.ToAddresses.Add(new EmailAddress { Name = user.UserName, Address = user.Email });
        message.Subject = String.Format("Er werd '{0}' uit de oogstkaart verwijderd.", item.Artikelnaam);
        message.Content = String.Format("Er werd '{0}' uit de oogstkaart verwijderd.", item.Artikelnaam);


        if (item != null)
        {


          _context.Views.RemoveRange(item.Views);
          _context.Files.RemoveRange(item.Files);
          _context.Afbeeldingen.Remove(item?.Avatar);
          _context.Specificaties.RemoveRange(item.Specificaties);
          _context.Afbeeldingen.RemoveRange(item.Gallery);
          _context.Requests.RemoveRange(item.Requests);
          _context.Locations.Remove(item?.Location);
          _context.OogstkaartItems.Remove(item);
          await _context.SaveChangesAsync();

          await _emailService.Send(message);
          return Ok();
        }
      }

      return BadRequest();
    }



    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] string status)
    {

      var user = await _userManager.GetUserAsync(User);
      if (user == null) return BadRequest();

      if (status == "tobereviewed")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).OrderByDescending(i => i.Create).Include(ic => ic.Company).ToListAsync();
        return Ok(requests);
      }
      else if (status == "declined")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).OrderByDescending(i => i.Create).Include(ic => ic.Company).ToListAsync();
        return Ok(requests);
      }
      else if (status == "accepted")
      {
        var requests = await _context.Requests.Where(r => r.Status == status).Include(ir => ir.Messages).OrderByDescending(i => i.Create).Include(ic => ic.Company).ToListAsync();
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

    //Usermanager

    [HttpGet("user")]
    public async Task<IActionResult> GetUsers()
    {

      //verwijder administrator accounts
      var users = await _context.Users.Include(i => i.Company).ThenInclude(i => i.Address).ToListAsync();
      List<ApplicationUser> temp = new List<ApplicationUser>();
      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        var adminrole = roles.FirstOrDefault( i => i == "administrator");

        if(adminrole != null)
        {
          temp.Add(user);
        }
        
      }

      foreach (var user in temp)
      {
        users.Remove(user);
      }

      if(users == null || users.Count == 0){
        return NotFound("Geen gebruikers");
      }

      if(users.Count > 0)
      {
        return Ok(users);
      }

      return BadRequest();
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUsers([FromRoute] string id)
    {

      if (ModelState.IsValid)
      {
        
        if (id == null)
        {
          return BadRequest("Userid is not correct");
        }

        var user = await _context.Users.Where(i => i.Id == id).Include(i => i.Company).ThenInclude(i => i.Address).FirstOrDefaultAsync();

        if(user != null)
        {
          user.PasswordHash = "";
          return Ok(user);
        }

        return NotFound("Geen gebruiker gevonden.");

      }

      return BadRequest();
    }

    [HttpPost("user/lockoutstatus/{userid}")]
    public async Task<IActionResult> ChangeLockoutStatus([FromRoute] string userid, bool lockstatusstatus)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }
      var user = await _userManager.FindByIdAsync(userid);
      if(user == null)
      {
        return NotFound();
      }

      var result = await _userManager.SetLockoutEnabledAsync(user, lockstatusstatus);
      if (result.Succeeded)
      {
        return Ok(lockstatusstatus);
      }
      return BadRequest();

    }


    [HttpPost("delete/user/{userid}")]
    public async Task<IActionResult> DeleteUser ([FromRoute] string userid)
     
    {
      if (ModelState.IsValid)
      {
        var user = await _context.Users.Where(i => i.Id == userid).Include(i => i.Company).ThenInclude(i => i.Address).FirstOrDefaultAsync();

        if(user == null)
        {
          return NotFound();
        }

        var oogstkaartproducten = await _context.OogstkaartItems.Where(i => i.UserID == user.Id).Include(i => i.Specificaties)
        .Include(i => i.Gallery).Include(i => i.Files).Include(i => i.Avatar).Include(i => i.Location).ToListAsync();

        _context.OogstkaartItems.RemoveRange(oogstkaartproducten);
        _context.Addresses.Remove(user.Company.Address);
        _context.Companies.Remove(user.Company);


        var result = await _userManager.DeleteAsync(user);
        if(result.Succeeded)
        {
          var message = new EmailMessage();

          //notify jansen by ods
          message.ToAddresses.Add(new EmailAddress { Name = "Jansen by ODS" , Address = "info@jansenbyods.com" });
          message.FromAddresses.Add(new EmailAddress { Name = "Jansen By ODS", Address = "info@jansenbyods.com" });

          message.Subject = String.Format("Gebruiker {0} werd verwijderd.", user.UserName);
          message.Content = String.Format("Gebruiker {0} werd verwijderd.", user.UserName);
          await _emailService.Send(message);

          //notify klant
          message = new EmailMessage();
          message.ToAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = user.Email });
          message.FromAddresses.Add(new EmailAddress { Name = "Jansen By ODS", Address = "info@jansenbyods.com" });

          message.Subject = String.Format("U werd account Jansen by ODS-account is verwijderd.");
          message.Content = String.Format("Geachte," +
            "uw Jansen By ODS werd verwijderd door de administrator.");

          await _emailService.Send(message);
          await _context.SaveChangesAsync();

          return Ok();
        }

        return BadRequest(result.Errors);
        
      }
      return BadRequest();
    }

    [HttpPost("lockenabled/{userid}")]
    public async Task<IActionResult> LockoutEnabled([FromRoute] string userid)
    {
      if (ModelState.IsValid)
      {
        var user = await _context.Users.Where(i => i.Id == userid).FirstOrDefaultAsync();

        if (user == null)
        {
          return NotFound();
        }

        user.LockoutEnabled = !user.LockoutEnabled;
        await _context.SaveChangesAsync();

        if (!user.LockoutEnabled)
        {
          var message = new EmailMessage();
          message.ToAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = user.Email });
          message.FromAddresses.Add(new EmailAddress { Name = "Jansen By ODS", Address = "info@jansenbyods.com" });

          message.Subject = String.Format("U werd Jansen by ODS-account werd geactiveerd.");
          message.Content = String.Format("Geachte," + System.Environment.NewLine +
            "uw Jansen By ODS werd verwijderd door de administrator.");
          return Ok(user.LockoutEnabled);
        }

      }

      return BadRequest();
    }

    [HttpPost("delete/file/{guid}")]
    public async Task<IActionResult> RemoveFile([FromRoute] string guid)
    {
      if (ModelState.IsValid)
      {
        var afbeelding = await _context.Afbeeldingen.FirstAsync(i => i.URI == guid);

        if (afbeelding != null)
        {
          _context.Afbeeldingen.Remove(afbeelding);
          await _context.SaveChangesAsync();
          return Ok();
        }

        var file = await _context.Files.FirstAsync(i => i.URI == guid);
        if (file != null)
        {
          _context.Files.Remove(file);
          await _context.SaveChangesAsync();
          return Ok();
        }
        return NotFound();
      }

      return BadRequest();

    }

    [HttpPost("message/user/{userid}")]
    public async Task<IActionResult> PostMessage([FromRoute] string userid, [FromQuery] string message, [FromQuery] string subject)
    {
      if (ModelState.IsValid)
      {
        var user = await _context.Users.Where(i => i.Id == userid).FirstOrDefaultAsync();

        if (user == null)
        {
          return NotFound();
        }

        if (user != null)
        {
          var email = new EmailMessage();
          email.ToAddresses.Add(new EmailAddress { Name = "Jansen by ODS", Address = user.Email });
          email.FromAddresses.Add(new EmailAddress { Name = "Jansen By ODS", Address = "info@jansenbyods.com" });

          email.Subject = subject;
          email.Content = message;

          await _emailService.Send(email);

          return Ok();
        }
      }
      return BadRequest();
    }


    private bool AreFallingInSameWeek(DateTime date1, DateTime date2)
    {
      return date1.AddDays(-(int)date1.DayOfWeek) == date2.AddDays(-(int)date2.DayOfWeek);
    }

   


  }
}



