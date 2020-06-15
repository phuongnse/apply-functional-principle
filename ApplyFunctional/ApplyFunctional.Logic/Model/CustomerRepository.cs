using System.Linq;
using ApplyFunctional.Logic.Common;
using ApplyFunctional.Logic.Utils;

namespace ApplyFunctional.Logic.Model
{
    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Customer GetByName(string name)
        {
            return UnitOfWork
                .Query<Customer>()
                .SingleOrDefault(customer => customer.Name == name);
        }
    }
}
