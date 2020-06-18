using System;
using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class Customer : Entity
    {
        private readonly string _name;

        private readonly string _primaryEmail;

        private string _secondaryEmail;

        protected Customer()
        {
        }

        public Customer(CustomerName name, Email primaryEmail, Maybe<Email> secondaryEmail, Industry industry)
        {
            _name = name;
            _primaryEmail = primaryEmail;
            SecondaryEmail = secondaryEmail;
            EmailSetting = new EmailSetting(industry, false);
            Status = CustomerStatus.Regular;
        }

        public virtual CustomerName Name => (CustomerName) _name;
        public virtual Email PrimaryEmail => (Email) _primaryEmail;

        public virtual Maybe<Email> SecondaryEmail
        {
            get => _secondaryEmail == null ? null : (Email) _secondaryEmail;
            protected set { _secondaryEmail = value.Unwrap(email => email.Value); }
        }

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
                CustomerStatus.Gold => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException()
            };
        }
    }
}