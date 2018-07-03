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
using AngularSPAWebAPI.Services.Email;
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
  public class OogstkaartController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IHostingEnvironment _env;
    private readonly ILogger _logger;
    private readonly RoleManager<IdentityRole> _rolemanager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _usermanager;
 //   private readonly IEmailService _emailservice;
    public OogstkaartController(
      UserManager<ApplicationUser> usermanager,
      RoleManager<IdentityRole> rolemanager,
      SignInManager<ApplicationUser> signInManager,
    //  IEmailService emailservice,
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
   //   _emailservice = emailservice;
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
        .Include(i => i.Location).ToListAsync();
      var filtereditems = allitems.Where(i => i.Location != null).ToList();
      return Ok(filtereditems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
      var tempuser = await _usermanager.GetUserAsync(User);
      if (tempuser == null) return NotFound("User not found");
      var items = await _context.OogstkaartItems.Where(i => i.UserID == tempuser.Id).Include(i => i.Gallery)
        .Include(i => i.Avatar).Include(i => i.Location).Include(i => i.Specificaties).Include(i => i.Files).ToListAsync();
      if (!items.Any()) return NotFound("user has no items");
      var item = items.FirstOrDefault(i => i.OogstkaartItemID == id);
      if (item == null) return NotFound("user has no item with provided ID");

      return Ok(item);
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
                  Name = file.Name
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
                  Name = file.Name
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
                  Name = file.Name
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


    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
      var user = await _usermanager.GetUserAsync(User);
      if (user != null)
      {
        var item = await _context.OogstkaartItems.Where(i => i.OogstkaartItemID == id).Where(i => user.Id == i.UserID)
          .Include(i => i.Gallery).Include(i => i.Location).Include(i => i.Specificaties).SingleAsync();

        if (item != null)
        {
          foreach (var specificatie in item.Specificaties) _context.Specificaties.Remove(specificatie);

          foreach (var foto in item.Gallery) _context.Afbeeldingen.Remove(foto);

          _context.Locations.Remove(item.Location);

          _context.OogstkaartItems.Remove(item);
          await _context.SaveChangesAsync();
          return Ok();
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
        item.Avatar = updatingitem.Avatar;
        item.Category = updatingitem.Category;
        item.Concept = updatingitem.Concept;
        item.CreateDate = updatingitem.CreateDate;
        item.DatumBeschikbaar = updatingitem.DatumBeschikbaar;
        item.Gallery = updatingitem.Gallery;
        item.Hoeveelheid = updatingitem.Hoeveelheid;
        item.Jansenserie = updatingitem.Jansenserie;

        item.Omschrijving = updatingitem.Omschrijving;
        item.VraagPrijsPerEenheid = updatingitem.VraagPrijsPerEenheid;
        item.VraagPrijsTotaal = updatingitem.VraagPrijsTotaal;

        item.Location.Latitude = updatingitem.Location.Latitude;
        item.Location.Longtitude = updatingitem.Location.Longtitude;


        if (updatingitem.Specificaties.Any())
          foreach (var specificatie in updatingitem.Specificaties)
          {
            if (specificatie.SpecificatieID == 0 && specificatie.SpecificatieSleutel != null &&
                specificatie.SpecificatieValue != null) item.Specificaties.Add(specificatie);


            foreach (var existingChild in item.Specificaties.ToList())
              if (updatingitem.Specificaties.All(c => c.SpecificatieID != existingChild.SpecificatieID))
                _context.Specificaties.Remove(existingChild);

            var temp = await _context.Specificaties.Where(i => i.SpecificatieID == specificatie.SpecificatieID)
              .FirstOrDefaultAsync();

            if (temp != null)
            {
              temp.SpecificatieOmschrijving = specificatie.SpecificatieOmschrijving;
              temp.SpecificatieSleutel = specificatie.SpecificatieSleutel;
              temp.SpecificatieEenheid = specificatie.SpecificatieEenheid;
            }
          }
        else{
          foreach (var specificatie in item.Specificaties)
            _context.Specificaties.Remove(specificatie);
        }
        _context.OogstkaartItems.Update(item);
        await _context.SaveChangesAsync();
        return Ok();
      }

      return BadRequest();
    }

    [HttpGet("acceptedrequests")]
    public async Task<IActionResult> GetAcceptedRequests(){
      var user = await _usermanager.GetUserAsync(User);

      if(user == null){
        return NotFound();
      }

      /*
      var oogstkaartitems = await _context.OogstkaartItems.Where(i => i.UserID == user.Id).Include(ir => ir.Requests).ToListAsync();
      List<Request> reqs = new List<Request>();

      foreach(var item in oogstkaartitems){
        foreach(var r in item.Requests){
          if(r.Status == "accepted"){
            reqs.Add(r);
          }
        }
      }

      return Ok(reqs);
       */      

      var requests = await _context.Requests.Where(r => r.Status == "accepted").Include(ir => ir.Messages).Include(ic => ic.Company).ToListAsync();

      if(requests == null){
        return NotFound();
      }

      return Ok(requests);

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

     
      var requestcount =  _context.Requests.Where(r => r.Status == "accepted").Where(req => req.UserViewed == false).Include(ir => ir.Messages).Include(ic => ic.Company).ThenInclude(i => i.Address).Count();

      return Ok(requestcount);
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

      var emailaddress = new EmailAddress
      {
        Address = request.Company.Email,
        Name = request.Company.CompanyName
      };

      var to = new EmailAddress
      {
        Address = "info@jansenbyods.com",
        Name = "Ods Oogstkaart"
      };


      var mail = new EmailMessage
      {
        Content = item.Artikelnaam,
        Subject = "Nieuwe aanvraag voor " + item.Artikelnaam  
      };

      mail.FromAddresses.Add(emailaddress);
      mail.ToAddresses.Add(to);
   //   _emailservice.Send(mail);
      
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
