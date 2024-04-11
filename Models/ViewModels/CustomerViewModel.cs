using AzureAppINTEX.Models;

namespace AzureAppINTEX.ViewModels
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        // New properties
        public int? CustomerID { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CountryOfResidence { get; set; }
        public string Gender { get; set; }
        public decimal? Age { get; set; }
        public string Email { get; set; }

        // Adjust the constructor to include the new properties
        public CustomerViewModel(Customer customer)
        {
            Id = customer.Id;
            UserName = customer.UserName;
            Email = customer.Email;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            CustomerID = customer.CustomerID;
            BirthDate = customer.BirthDate;
            CountryOfResidence = customer.CountryOfResidence;
            Gender = customer.Gender;
            Age = customer.Age;
        }

        public CustomerViewModel() { }
    }
}
