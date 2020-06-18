using FluentNHibernate.Mapping;
using NHibernate.Type;

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

            Map(customer => customer.Status).CustomType<CustomerStatus>();

            Component(
                customer => customer.EmailSetting, 
                componentPart =>
                {
                    componentPart.Map(emailSetting => emailSetting.EmailingIsDisabled);

                    componentPart.References(emailSetting => emailSetting.Industry);
                });
        }
    }
}