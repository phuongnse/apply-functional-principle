using System;
using System.Collections.Generic;
using ApplyFunctionalPrinciple.Logic.Common;
using static ApplyFunctionalPrinciple.Logic.Model.EmailCampaign;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class EmailSetting : ValueObject<EmailSetting>
    {
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
            return new EmailSetting(industry, false);
        }

        private EmailCampaign GetEmailCampaign()
        {
            if (EmailingIsDisabled)
                return None;

            return Industry.Name switch
            {
                Industry.CarsIndustry => LatestCarModels,
                Industry.PharmacyIndustry => PharmacyNews,
                Industry.OtherIndustry => Generic,
                _ => throw new ArgumentException()
            };
        }
    }
}