using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureAppINTEX.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CountryOfResidence { get; set; }
        public string Gender { get; set; }
        public decimal? Age { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); // Current user roles
        public List<string> AllRoles { get; set; } = new List<string>(); // All available roles
        public List<string> SelectedRoles { get; set; } = new List<string>(); // Roles selected in the form

    }

}
