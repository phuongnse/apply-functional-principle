using System;
using NHibernate.Proxy;

namespace ApplyFunctional.Logic.Common
{
    public abstract class Entity
    {
        protected Entity()
        {
        }

        protected Entity(long id)
        {
            Id = id;
        }

        public virtual long Id { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity other))
                return false;

            if (GetRealType() != other.GetRealType())
                return false;

            if (Id == 0 || other.Id == 0)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (GetRealType().ToString() + Id).GetHashCode();
        }

        private Type GetRealType()
        {
            return NHibernateProxyHelper.GetClassWithoutInitializingProxy(this);
        }
    }
}