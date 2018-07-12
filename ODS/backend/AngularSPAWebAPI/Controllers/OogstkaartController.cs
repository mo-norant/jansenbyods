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
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using File = AngularSPAWebAPI.Models.DatabaseModels.General.File;

namespace AngularSPAWebAPI.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme,
    Policy = "Access Resources")]

  public class OogstkaartController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IHostingEnvironment _env;
    private readonly ILogger _logger;
    private readonly RoleManager<IdentityRole> _rolemanager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _usermanager;
    private readonly IEmailService _emailservice;
    public OogstkaartController(
      UserManager<ApplicationUser> usermanager,
      RoleManager<IdentityRole> rolemanager,
      SignInManager<ApplicationUser> signInManager,
      IHostingEnvironment appEnvironment,
      ILogger<IdentityController> logger,
      ApplicationDbContext context, 
      IEmailService emailservice
      )
    {
      _usermanager = usermanager;
      _rolemanager = rolemanager;
      _signInManager = signInManager;
      _env = appEnvironment;
      _logger = logger;
      _context = context;
      _emailservice = emailservice;
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OogstkaartItem oogstkaartItem)
    {
      var now = DateTime.Now;
      var user = await _usermanager.GetUserAsync(User);

      if (ModelState.IsValid)
      {
        oogstkaartItem.OnlineStatus = true;
        oogstkaartItem.UserID = user.Id;
        oogstkaartItem.CreateDate = now;
        await _context.OogstkaartItems.AddAsync(oogstkaartItem);
        await _context.SaveChangesAsync();
        return Ok(oogstkaartItem.OogstkaartItemID);
      }

      return BadRequest();
    }


    [HttpPost("Location")]
    public async Task<IActionResult> Post([FromBody] Location Location, [FromQuery] int OogstkaartitemID)
    {
      if (ModelState.IsValid)
      {
        var item = await _context.OogstkaartItems.FirstOrDefaultAsync(o => o.OogstkaartItemID == OogstkaartitemID);

        if (item != null)
        {
          item.Location = Location;
          await _context.SaveChangesAsync();
          return Ok();
        }
      }

      return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var user = await _usermanager.GetUserAsync(User);

      if (!ModelState.IsValid) return BadRequest();

      if (user == null) return NotFound();

      var allitems = await _context.OogstkaartItems.Where(c => c.UserID == user.Id).Include(c => c.Specificaties)
        .Include(i => i.Location).Include(i => i.Avatar).ToListAsync();
      var filtereditems = allitems.Where(i => i.Location != null).Where(i => i.Avatar != null).ToList();
      return Ok(filtereditems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
      var tempuser = await _usermanager.GetUserAsync(User);
      var product = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => i.UserID == tempuser.Id).Include(i => i.Avatar).Include(i => i.Files).Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Specificaties).FirstOrDefaultAsync();
      if(product == null)
      {
        return NotFound("Product niet gevonden");
      }
      return Json(product);
    }

    [HttpPost("productstatus/{id}")]
    public async Task<IActionResult> PostProduct([FromRoute] int id)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(o => o.UserID == user.Id).Where(o => o.OogstkaartItemID == id)
          .FirstOrDefaultAsync();
        if (item != null)
        {
          item.OnlineStatus = !item.OnlineStatus;
          await _context.SaveChangesAsync();
          return Ok();
        }
      }

      return BadRequest();
    }


    [HttpPost("sold/{id}")]
    public async Task<IActionResult> OogstkaartitemSold([FromRoute] int id)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(o => o.UserID == user.Id).Where(o => o.OogstkaartItemID == id)
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




    [HttpPost("oogstkaartavatar/{id}")]
    public async Task<IActionResult> Oogstkaartavatar([FromRoute] int id)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => i.UserID == user.Id)
          .SingleOrDefaultAsync();

        if (item == null) return NotFound();

        var files = HttpContext.Request.Form.Files;

        foreach (var image in files)
          if (image != null && image.Length > 0)
          {
            var file = image;
            var uploads = Path.Combine(_env.WebRootPath, "uploads/images");
            if (file.Length > 0)
            {
              var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
              using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
              {
                await file.CopyToAsync(fileStream);
                item.Avatar = new Afbeelding
                {
                  Create = DateTime.Now,
                  URI = fileName,
                  Name = file.Name,
                  Extension = Path.GetExtension(fileName)
                };
              }
            }
          }

        await _context.SaveChangesAsync();
        return Ok();
      }

      return BadRequest();
    }



    [HttpPost("gallery/{id}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> PostImage([FromRoute] int id)
    {
      if (!ModelState.IsValid) return BadRequest();

      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => i.UserID == user.Id)
          .Include(i => i.Gallery).SingleOrDefaultAsync();

        if (item == null) return NotFound();

        var files = HttpContext.Request.Form.Files;

        if (item.Gallery == null) item.Gallery = new List<Afbeelding>();
        foreach (var image in files)
          if (image != null && image.Length > 0)
          {
            var file = image;
            var uploads = Path.Combine(_env.WebRootPath, "uploads/images");
            if (file.Length > 0)
            {
              var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
              using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
              {
                await file.CopyToAsync(fileStream);

                var afbeelding = new Afbeelding
                {
                  Create = DateTime.Now,
                  URI = fileName,
                  Name = file.Name,
                  Extension = Path.GetExtension(fileName)
                };
                item.Gallery.Add(afbeelding);
              }
            }
          }

        await _context.SaveChangesAsync();
        return Ok();
      }

      return BadRequest();
    }


    [HttpPost("files/{id}")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> PostFiles([FromRoute] int id)
    {
      if (!ModelState.IsValid) return BadRequest();

      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => i.UserID == user.Id)
          .Include(i => i.Files).Include(i => i.Gallery).SingleOrDefaultAsync();

        if (item == null) return NotFound();

        var files = HttpContext.Request.Form.Files;

        if (item.Gallery == null) item.Gallery = new List<Afbeelding>();
        foreach (var image in files)
          if (image != null && image.Length > 0)
          {
            var file = image;
            var uploads = Path.Combine(_env.WebRootPath, "uploads/files");
            if (file.Length > 0)
            {
              var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
              using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
              {
                await file.CopyToAsync(fileStream);

                var filesave = new File
                {
                  Create = DateTime.Now,
                  URI = fileName,
                  Name = file.Name,
                  Extension = Path.GetExtension(fileName)
                };


                item.Files.Add(filesave);
              }
            }
          }

        await _context.SaveChangesAsync();
        return Ok();
      }

      return BadRequest();
    }

    [HttpPost("notify/{id}")]
    public async Task<IActionResult> Notify([FromRoute] int id)
    {

      if (!ModelState.IsValid) {
        return BadRequest();
      }
      
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => i.UserID == user.Id)
          .Include(i => i.Files).Include(i => i.Gallery).SingleOrDefaultAsync();

        if (item == null)
        {
          BadRequest();

        }

        //Stuur bevestiging naar admin dat er een nieuw product is.
        EmailMessage mail = new EmailMessage();
        mail.FromAddresses.Add(new EmailAddress { Address = user.Email });
        mail.Subject = string.Format("Er is een nieuw product aangemaakt op de oogstkaart: {0} ", item.Artikelnaam);
        mail.ToAddresses.Add(new EmailAddress { Address = "info@jansenbyods.com" });
        mail.Content = string.Format("Er is een nieuw product ({0}) aangemaakt op de oogstkaart. <a href='http://jansenbyods.com/oogstkaart/{1}'>Bekijk product.</a>", item.Artikelnaam, item.OogstkaartItemID.ToString());
        await _emailservice.Send(mail);

        //Stuur bevestiging naar klant dat product werd gepubliceerd.

        EmailMessage responseack = new EmailMessage();
        responseack.FromAddresses.Add(new EmailAddress { Address = "info@jansenbyods.com" });
        responseack.Subject = string.Format("Uw product ' {0} ' werd succesvol op de oogstkaart gepubliceerd. ", item.Artikelnaam);
        responseack.ToAddresses.Add(new EmailAddress { Address = user.Email });
        responseack.Content = string.Format("U hebt succesvol een product ('{0}') op de oogstkaart gepubliceerd. <a href='http://jansenbyods.com/oogstkaart/{1}'>Bekijk uw product.</a>", item.Artikelnaam, item.OogstkaartItemID.ToString());
        await _emailservice.Send(responseack);


        return Ok();

      }

      
      return BadRequest();
      
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => user.Id == i.UserID)
          .Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Avatar).Include(i => i.Files).Include(i => i.Requests).ThenInclude(i => i.Messages).Include(i => i.Specificaties).SingleAsync();

        if (item != null)
        {

          _context.Files.RemoveRange(item.Files);
          _context.Afbeeldingen.Remove(item.Avatar);
          _context.Specificaties.RemoveRange(item.Specificaties);
          _context.Afbeeldingen.RemoveRange(item.Gallery);
          _context.Requests.RemoveRange(item.Requests);
          _context.Locations.Remove(item.Location);
          _context.OogstkaartItems.Remove(item);
          await _context.SaveChangesAsync();
          return Ok();
        }
      }

      return BadRequest();
    }

    [HttpPost("delete/range")]
    public async Task<IActionResult> DeleteRange([FromQuery] int[] ids)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        List<OogstkaartItem> items = new List<OogstkaartItem>();

        foreach (var id in ids)
        {
          items.Add( await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => user.Id == i.UserID)
          .Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Avatar).Include(i => i.Files).Include(i => i.Requests).ThenInclude(i => i.Messages).Include(i => i.Specificaties).SingleAsync());
        }

        foreach (var oi in items)
        {
          _context.Files.RemoveRange(oi.Files);
          _context.Afbeeldingen.Remove(oi.Avatar);
          _context.Specificaties.RemoveRange(oi.Specificaties);
          _context.Afbeeldingen.RemoveRange(oi.Gallery);
          _context.Requests.RemoveRange(oi.Requests);
          _context.Locations.Remove(oi.Location);
          _context.OogstkaartItems.Remove(oi);
          await _context.SaveChangesAsync();

          var allitems = await _context.OogstkaartItems.Where(c => c.UserID == user.Id).Include(c => c.Specificaties)
        .Include(i => i.Location).Include(i => i.Avatar).ToListAsync();
          var filtereditems = allitems.Where(i => i.Location != null).Where(i => i.Avatar != null).ToList();

          return Ok(filtereditems);
        }
      }

      return BadRequest();
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] OogstkaartItem updatingitem)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == updatingitem.OogstkaartItemID)
          .Where(i => i.UserID == user.Id).Include(i => i.Location).Include(i => i.Specificaties).FirstOrDefaultAsync();

        if (item == null) return NotFound();


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

          if(spec == null)
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


        //bekijk of er zaken verwijderd moeten worden die niet in de geüpdate lijst voorkomen
        foreach (var spec in item.Specificaties)
        {
          var sp = updatingitem.Specificaties.FirstOrDefault(i => i.SpecificatieID == spec.SpecificatieID);
          if(sp == null)
          {
            _context.Specificaties.Remove(spec);
          }
        }
        _context.OogstkaartItems.Update(item);
        await _context.SaveChangesAsync();

        return Ok();
      }

      return BadRequest();
    }

    [HttpGet("acceptedrequests")]
    public async Task<IActionResult> GetAcceptedRequests(){
      var userreq = await _usermanager.GetUserAsync(User);

      if(userreq == null){
        return NotFound("User not found");
      }

      var r = from requests in _context.Requests
                   join oogstkaartitems in _context.OogstkaartItems on requests.OogstkaartID equals oogstkaartitems.OogstkaartItemID
                   join users in _context.Users on oogstkaartitems.UserID equals users.Id
                   where requests.Status == "accepted"
                   where users.Id == userreq.Id
                   select requests;

      var result = await r.ToListAsync();


      return Ok(result);
    }


    [HttpGet("acceptedrequests/{id}")]
    public async Task<IActionResult> GetAcceptedRequest( [FromRoute] int id )
    {
      var user = await _usermanager.GetUserAsync(User);

      if (user == null)
      {
        return NotFound();
      }

      var request = await _context.Requests.Where(r => r.Status == "accepted").Where(req => req.RequestID == id ).Include(ir => ir.Messages).Include(ic => ic.Company).ThenInclude(i => i.Address).FirstOrDefaultAsync();

      if (request == null)
      {
        return NotFound();
      }

      return Ok(request);

    }

    [HttpPost("openrequest/{id}")]
    public async Task<IActionResult> OpenRequest( [FromRoute] int id ){
      var user = await _usermanager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound();
      }

      var request = await _context.Requests.Where(r => r.Status == "accepted").Where(req => req.RequestID == id).Include(ir => ir.Messages).Include(ic => ic.Company).ThenInclude(i => i.Address).FirstOrDefaultAsync();

      if (request == null)
      {
        return NotFound();
      }

      request.UserViewed = true;
      await _context.SaveChangesAsync();
      return Ok();
    }

    [HttpGet("openrequests")]
    public async Task<IActionResult> NewRequests()
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user == null)
      {
        return NotFound();
      }

     
      var requestcount =  _context.Requests.Where(r => r.Status == "accepted").Include(ir => ir.Messages).Include(ic => ic.Company).ThenInclude(i => i.Address).Count();

      return Ok(requestcount);
    }


    [AllowAnonymous]
    [HttpPost("mail")]
    public async Task<IActionResult> TestMail()
    {

      EmailMessage mail = new EmailMessage();
      mail.FromAddresses.Add(new EmailAddress { Address = "mo.bouzim@live.be" });
      mail.Subject = string.Format("Er is een nieuw product aangemaakt op de oogstkaart: {0} ", "test");
      mail.ToAddresses.Add(new EmailAddress { Address = "info@jansenbyods.com" });
      mail.Content = string.Format("Er is een nieuw product (é) aangemaakt op de oogstkaart. Bekijk product op: é");
      await _emailservice.Send(mail);
      return Ok();
    }

    [AllowAnonymous]
    [HttpPost("request/{id}")]
    public async Task<IActionResult> CreateRequest([FromRoute] int id, [FromBody] Request request)
    {
      if (!ModelState.IsValid) return BadRequest();

      var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Include(i => i.Requests)
        .FirstOrDefaultAsync();

      if (item == null) return BadRequest();
      request.Create = DateTime.Now;
      request.Status = "tobereviewed";
      request.Messages.First().Created = DateTime.Now;
      request.OogstkaartID = item.OogstkaartItemID;
      item.Requests.Add(request);
      await _context.SaveChangesAsync();

      EmailMessage mail = new EmailMessage();
      mail.FromAddresses.Add(new EmailAddress { Address = request.Company.Email });
      mail.Subject = string.Format("Er is een nieuwe aanvraag voor {0} ", item.Artikelnaam);
      mail.ToAddresses.Add(new EmailAddress { Address = "info@jansenbyods.com" });
      mail.Content = string.Format("Er is een nieuwe aanvraag ({0}) aangemaakt voor de oogstkaart." + item.OogstkaartItemID.ToString(), item.Artikelnaam);
      await _emailservice.Send(mail);
      return Ok();

    }

    [AllowAnonymous]
    [HttpPost("view/{id}")]
    public async Task<IActionResult> PostView([FromRoute] int id)
    {
      var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).FirstOrDefaultAsync();

      if (item != null)
      {
        item.Views++;
        await _context.SaveChangesAsync();
        return Ok(item.Views);
      }

      return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("mapview")]
    public async Task<IActionResult> GetAdmin()
    {
      var artikels = await _context.OogstkaartItems.Where(i => i.OnlineStatus).Include(i => i.Location)
        .Where(i => i.Location != null).Include(i => i.Avatar).Where(i => i.Avatar != null)
        .Include(i => i.Gallery).Include(i => i.Specificaties).Include(i => i.Files).ToListAsync();

   

      return Ok(artikels);
    }

    [AllowAnonymous]
    [HttpGet("mapview/{id}")]
    public async Task<IActionResult> GetItemFromRoute([FromRoute] int id)
    {
      var artikel = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Include(i => i.Location)
        .Where(i => i.Location != null).Include(i => i.Avatar).Include(i => i.Files)
        .Include(i => i.Gallery).Include(i => i.Specificaties).Include(i => i.Files).FirstOrDefaultAsync();

      if (artikel == null) return BadRequest();

      return Ok(artikel);
    }

		[AllowAnonymous]
    [HttpGet("gerelateerdeproducten")]
    public async Task<IActionResult> GetGerelateerdeProducten([FromQuery] int oogstkaartid)
    {
      var item = await _context.OogstkaartItems.FirstOrDefaultAsync(q => q.OogstkaartItemID == oogstkaartid);
      if(item == null){
        return NotFound();
      }

      var suggesteditems = await _context.OogstkaartItems.Where(p => p.Category == item.Category).OrderBy(q => q.Views).Include(i => i.Avatar).Where(i => i.Avatar != null).Take(4).ToListAsync();

      if(suggesteditems.Contains(item)){
        suggesteditems.Remove(item);
      }


      return Ok(suggesteditems);
    }

 
    



  }

}
