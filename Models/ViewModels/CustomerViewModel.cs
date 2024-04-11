using AzureAppINTEX.Models; // Ensure this using directive matches your namespace for Customer

namespace AzureAppINTEX.ViewModels
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public CustomerViewModel(Customer customer)
        {
            Id = customer.Id;
            UserName = customer.UserName;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
        }

        // Add an empty constructor for model binding during creation
        public CustomerViewModel() { }
    }
}
