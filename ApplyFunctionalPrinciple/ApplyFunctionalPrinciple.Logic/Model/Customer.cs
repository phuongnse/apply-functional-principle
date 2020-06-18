using ApplyFunctionalPrinciple.Logic.Common;
using System;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class Customer : Entity
    {
        protected Customer()
        {
        }

        public Customer(CustomerName name, Email primaryEmail, Email secondaryEmail, Industry industry)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _primaryEmail = primaryEmail ?? throw new ArgumentNullException(nameof(primaryEmail));
            _secondaryEmail = secondaryEmail;
            Industry = industry ?? throw new ArgumentNullException(nameof(industry));
            EmailCampaign = GetEmailCampaign(industry);
            Status = CustomerStatus.Regular;
        }

        private readonly string _name;
        public virtual CustomerName Name => (CustomerName) _name;

        private readonly string _primaryEmail;
        public virtual Email PrimaryEmail => (Email) _primaryEmail;

        private readonly string _secondaryEmail;
        public virtual Email SecondaryEmail => (Email) _secondaryEmail;

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