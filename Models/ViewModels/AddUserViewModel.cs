using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureAppINTEX.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string CountryOfResidence { get; set; }

        public string Gender { get; set; }

        public decimal? Age { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
