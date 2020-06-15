using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;

namespace ApplyFunctional.Logic.Utils
{
    public static class SessionFactory
    {
        private static ISessionFactory _sessionFactory;

        internal static ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

        public static void Init(string connectionString)
        {
            _sessionFactory = BuildSessionFactory(connectionString);
        }

        private static ISessionFactory BuildSessionFactory(string connectionString)
        {
            return Fluently
                .Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                .Mappings(mappingConfiguration => mappingConfiguration.FluentMappings
                    .AddFromAssembly(Assembly.GetExecutingAssembly())
                    .Conventions.Add(
                        ForeignKey.EndsWith("Id"),
                        ConventionBuilder.Property.When(
                            acceptanceCriteria => acceptanceCriteria.Expect(
                                propertyInspector => propertyInspector.Nullable,
                                Is.Not.Set),
                            propertyInstance => propertyInstance.Not.Nullable()))
                    .Conventions.Add<Convention>())
                .BuildSessionFactory();
        }
    }
}