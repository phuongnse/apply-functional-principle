using System.Linq;
using ApplyFunctionalPrinciple.Logic.Common;
using ApplyFunctionalPrinciple.Logic.Utils;

namespace ApplyFunctionalPrinciple.Logic.Model
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