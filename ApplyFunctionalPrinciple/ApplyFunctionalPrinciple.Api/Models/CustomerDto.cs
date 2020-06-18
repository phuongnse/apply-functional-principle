using ApplyFunctionalPrinciple.Logic.Model;

namespace ApplyFunctionalPrinciple.Api.Models
{
    public class CustomerDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string Industry { get; set; }
        public EmailCampaign EmailCampaign { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
