using FluentNHibernate.Mapping;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class IndustryMap : ClassMap<Industry>
    {
        public IndustryMap()
        {
            Id(industry => industry.Id);

            Map(industry => industry.Name);
        }
    }
}