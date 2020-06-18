using FluentNHibernate.Mapping;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(customer => customer.Id);

            Map(customer => customer.Name)
                .CustomType<string>()
                .Access.CamelCaseField(Prefix.Underscore);

            Map(customer => customer.PrimaryEmail)
                .CustomType<string>()
                .Access.CamelCaseField(Prefix.Underscore);

            Map(customer => customer.SecondaryEmail)
                .CustomType<string>()
                .Access.CamelCaseField(Prefix.Underscore)
                .Nullable();

            Map(customer => customer.EmailCampaign).CustomType<EmailCampaign>();
            Map(customer => customer.Status).CustomType<CustomerStatus>();

            References(customer => customer.Industry);
        }
    }
}