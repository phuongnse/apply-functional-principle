using System.Linq;
using ApplyFunctionalPrinciple.Logic.Common;
using ApplyFunctionalPrinciple.Logic.Utils;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class IndustryRepository : Repository<Industry>
    {
        public IndustryRepository(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Maybe<Industry> GetByName(string name)
        {
            return UnitOfWork
                .Query<Industry>()
                .SingleOrDefault(industry => industry.Name == name);
        }
    }
}