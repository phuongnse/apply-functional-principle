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

            EmailSetting = new EmailSetting(industry, false);
            Status = CustomerStatus.Regular;
        }

        private readonly string _name;
        public virtual CustomerName Name => (CustomerName) _name;

        private readonly string _primaryEmail;
        public virtual Email PrimaryEmail => (Email) _primaryEmail;

        private readonly string _secondaryEmail;
        public virtual Email SecondaryEmail => (Email) _secondaryEmail;

        public virtual EmailSetting EmailSetting { get; protected set; }
        public virtual CustomerStatus Status { get; protected set; }

        public virtual void DisableEmailing()
        {
            EmailSetting = EmailSetting.DisableEmailing();
        }

        public virtual void UpdateIndustry(Industry industry)
        {
            EmailSetting = EmailSetting.ChangeIndustry(industry);
        }

        public virtual bool CanBePromoted()
        {
            return Status != CustomerStatus.Gold;
        }

        public virtual void Promote()
        {
            if (!CanBePromoted())
                throw new InvalidOperationException();

            Status = Status switch
            {
                CustomerStatus.Regular => CustomerStatus.Preferred,
                CustomerStatus.Preferred => CustomerStatus.Gold,
                _ => throw new InvalidOperationException()
            };
        }
    }
}