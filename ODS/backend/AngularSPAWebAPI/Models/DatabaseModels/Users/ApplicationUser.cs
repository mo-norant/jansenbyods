using AngularSPAWebAPI.Models.DatabaseModels.General;
using Microsoft.AspNetCore.Identity;
using System;

namespace AngularSPAWebAPI.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public Company Company { get; set; }
        public DateTime CreateDate { get; set; }
        public bool SubscripedWithCompany { get; set; }
        public string Name { get; set; }

    }
}
