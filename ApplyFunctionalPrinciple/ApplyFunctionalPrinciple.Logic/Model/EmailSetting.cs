using System;
using System.Collections.Generic;
using ApplyFunctionalPrinciple.Logic.Common;
using static ApplyFunctionalPrinciple.Logic.Model.EmailCampaign;
using static ApplyFunctionalPrinciple.Logic.Model.Industry;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class EmailSetting : ValueObject<EmailSetting>
    {
        private EmailSetting()
        {
        }

        public EmailSetting(Industry industry, bool emailingIsDisabled)
        {
            Industry = industry;
            EmailingIsDisabled = emailingIsDisabled;
        }

        public Industry Industry { get; }
        public bool EmailingIsDisabled { get; }
        public EmailCampaign EmailCampaign => GetEmailCampaign();

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Industry;
            yield return EmailingIsDisabled;
        }

        public EmailSetting DisableEmailing()
        {
            return new EmailSetting(Industry, true);
        }

        public EmailSetting ChangeIndustry(Industry industry)
        {
            return new EmailSetting(industry, EmailingIsDisabled);
        }

        private EmailCampaign GetEmailCampaign()
        {
            if (EmailingIsDisabled)
                return None;

            if (Industry == Cars)
                return LatestCarModels;

            if (Industry == Pharmacy)
                return PharmacyNews;

            if (Industry == Other)
                return Generic;

            throw new InvalidOperationException();
        }
    }
}