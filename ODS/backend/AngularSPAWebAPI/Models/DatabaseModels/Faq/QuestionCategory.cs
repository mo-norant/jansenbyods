using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Faq
{
    public class QuestionCategory
    {
    public int QuestionCategoryID { get; set; }
    public string Title { get; set; }
    public DateTime CreationDate { get; set; }
    public ICollection<Question> Questions { get; set; }
  }
}
