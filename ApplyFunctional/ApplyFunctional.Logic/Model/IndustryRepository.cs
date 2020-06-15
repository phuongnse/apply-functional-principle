using System.Linq;
using ApplyFunctional.Logic.Common;
using ApplyFunctional.Logic.Utils;

namespace ApplyFunctional.Logic.Model
{
    public class IndustryRepository : Repository<Industry>
    {
        public IndustryRepository(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Industry GetByName(string name)
        {
            return UnitOfWork
                .Query<Industry>()
                .SingleOrDefault(industry => industry.Name == name);
        }
    }
}
