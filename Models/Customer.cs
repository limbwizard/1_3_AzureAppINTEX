using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AzureAppINTEX.Models
{
    public class Customer : IdentityUser
    {
        // Custom properties specific to your application
        public int? CustomerID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CountryOfResidence { get; set; }
        public string? Gender { get; set; }
        public decimal? Age { get; set; }

        // Navigation properties for domain-specific relationships
        // Assuming Order and Recommendation are other domain models in your application
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<CustomerRecommendation>? Recommendations { get; set; }
    }
}
