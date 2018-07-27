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
using AngularSPAWebAPI.Models.DatabaseModels.Faq;

namespace AngularSPAWebAPI.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Roles ="administrator")]
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

    [HttpGet("QuestionsCategories")]
    public async Task<IActionResult> GetQuestionCategories()
    {
      var qc = await _context.QuestionCategories.OrderBy(i => i.Title).ToListAsync();
      return Ok(qc);
    }

    [HttpPost("Category")]
    public async Task<IActionResult> PostCategory([FromBody] QuestionCategory qc)
    {
      qc.CreationDate = DateTime.Now;
      await _context.QuestionCategories.AddAsync(qc);
      await _context.SaveChangesAsync();
      return Ok(_context.QuestionCategories);
    }

    [HttpPost("Question/{qcid}")]
    public async Task<IActionResult> PostQuestion([FromRoute] int qcid ,[FromBody] Question q)
    {

      var qc = await _context.QuestionCategories.Include(i => i.Questions).FirstOrDefaultAsync(i => i.QuestionCategoryID == qcid);

      if(qc == null)
      {
        return NotFound();
      }

      q.CreateDate = DateTime.Now;
      qc.Questions.Add(q);
      await _context.SaveChangesAsync();
      return Ok();
    }

    [HttpPost("delete/Question/{qid}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] int qid)
    {
      var question = await _context.Questions.FirstOrDefaultAsync(i => i.QuestionID == qid);
      if(question == null)
      {
        return NotFound();
      }

      _context.Questions.Remove(question);

      var result = await _context.SaveChangesAsync();

      return Ok(result);

    }

    [HttpPost("delete/QuestionCategory/{qcid}")]
    public async Task<IActionResult> DeleteQuestionCategory([FromRoute] int qcid)
    {
      var qc = await _context.QuestionCategories.Include(i => i.Questions).FirstOrDefaultAsync(i => i.QuestionCategoryID == qcid);
      if (qc == null)
      {
        return NotFound();
      }

      _context.QuestionCategories.Remove(qc);

      var result = await _context.SaveChangesAsync();

      return Ok(result);

    }

  }
}
