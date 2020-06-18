using NullGuard;

[assembly: NullGuard(ValidationFlags.All)]

namespace ApplyFunctionalPrinciple.Logic.Utils
{
    public static class Initer
    {
        public static void Init(string connectionString)
        {
            SessionFactory.Init(connectionString);
        }
    }
}
