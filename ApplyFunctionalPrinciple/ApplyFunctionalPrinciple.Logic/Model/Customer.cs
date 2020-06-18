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
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PrimaryEmail = primaryEmail ?? throw new ArgumentNullException(nameof(primaryEmail));
            SecondaryEmail = secondaryEmail;
            Industry = industry ?? throw new ArgumentNullException(nameof(industry));
            EmailCampaign = GetEmailCampaign(industry);
            Status = CustomerStatus.Regular;
        }

        public virtual CustomerName Name { get; protected set; }
        public virtual Email PrimaryEmail { get; protected set; }
        public virtual Email SecondaryEmail { get; protected set; }
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