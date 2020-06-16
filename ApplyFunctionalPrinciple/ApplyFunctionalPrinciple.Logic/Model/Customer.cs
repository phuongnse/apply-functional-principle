using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class Customer : Entity
    {
        protected Customer()
        {
        }

        public Customer(string name, string primaryEmail, string secondaryEmail, Industry industry)
        {
            Name = name;
            PrimaryEmail = primaryEmail;
            SecondaryEmail = secondaryEmail;
            Industry = industry;
            EmailCampaign = GetEmailCampaign(industry);
            Status = CustomerStatus.Regular;
        }

        public virtual string Name { get; protected set; }
        public virtual string PrimaryEmail { get; protected set; }
        public virtual string SecondaryEmail { get; protected set; }
        public virtual Industry Industry { get; protected set; }
        public virtual EmailCampaign EmailCampaign { get; protected set; }
        public virtual CustomerStatus Status { get; protected set; }

        private static EmailCampaign GetEmailCampaign(Industry industry)
        {
            return industry.Name switch
            {
                Industry.CarsIndustry => EmailCampaign.LatestCarModels,
                Industry.PharmacyIndustry => EmailCampaign.PharmacyNews,
                _ => EmailCampaign.Generic
            };
        }

        public virtual void DisableEmailing()
        {
            EmailCampaign = EmailCampaign.None;
        }

        public virtual void UpdateIndustry(Industry industry)
        {
            if (EmailCampaign == EmailCampaign.None)
                return;

            EmailCampaign = GetEmailCampaign(industry);
            Industry = industry;
        }

        public virtual bool CanBePromoted()
        {
            return Status != CustomerStatus.Gold;
        }

        public virtual void Promote()
        {
            Status = Status == CustomerStatus.Regular ? CustomerStatus.Preferred : CustomerStatus.Gold;
        }
    }
}