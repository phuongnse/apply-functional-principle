using FluentNHibernate.Mapping;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(customer => customer.Id);

            Map(customer => customer.Name);
            Map(customer => customer.PrimaryEmail);
            Map(customer => customer.SecondaryEmail).Nullable();
            Map(customer => customer.EmailCampaign).CustomType<EmailCampaign>();
            Map(customer => customer.Status).CustomType<CustomerStatus>();

            References(customer => customer.Industry);
        }
    }
}