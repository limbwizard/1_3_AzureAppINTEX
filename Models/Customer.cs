using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AzureAppINTEX.Models
{
    public class Customer : IdentityUser
    {
        // Custom properties specific to your application
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CountryOfResidence { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }

        // Navigation properties for domain-specific relationships
        // Assuming Order and Recommendation are other domain models in your application
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Recommendation>? Recommendations { get; set; }
    }
}
