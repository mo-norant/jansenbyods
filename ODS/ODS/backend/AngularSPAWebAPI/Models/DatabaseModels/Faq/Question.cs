using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Models.DatabaseModels.Faq
{
    public class Question
    {
    public int QuestionID { get; set; }
    public string _Question { get; set; }
    public string Answer { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
