using AngularSPAWebAPI.Models.DatabaseModels.General;

namespace AngularSPAWebAPI.Models.AccountViewModels
{
    /// <summary>
    /// Class required to create a new user.
    /// </summary>
    public class CreateViewModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string password2 { get; set; }
        public string name { get; set; }

      public Company Company { get; set; }
    }
}
