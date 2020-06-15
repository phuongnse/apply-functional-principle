using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;

namespace ApplyFunctional.Logic.Utils
{
    public class Convention : IHasManyConvention, IReferenceConvention, IClassConvention, IIdConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name);
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.LazyLoad();
            instance.AsBag();
            instance.Cascade.SaveUpdate();
            instance.Inverse();
        }

        public void Apply(IIdentityInstance instance)
        {
            instance.Column(instance.EntityType.Name + "ID");
            instance.GeneratedBy.HiLo("Ids", "NextHigh", "9", "EntityName = '" + instance.EntityType.Name + "'");
        }

        public void Apply(IManyToOneInstance instance)
        {
            instance.LazyLoad(Laziness.Proxy);
            instance.Cascade.None();
            instance.Not.Nullable();
        }
    }
}