using ApplyFunctionalPrinciple.Logic.Common;
using System.Collections.Generic;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class CustomerName : ValueObject<CustomerName>
    {
        public string Value { get; }

        private CustomerName(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static Result<CustomerName> Create(Maybe<string> maybeCustomerName)
        {
            if (maybeCustomerName.HasNoValue)
                return Result.Fail<CustomerName>("Customer name should not be empty");

            var customerName = maybeCustomerName.Value.Trim();

            if (customerName == string.Empty)
                return Result.Fail<CustomerName>("Customer name should not be empty");

            if (customerName.Length > 200)
                return Result.Fail<CustomerName>("Customer name is too long");

            return Result.Ok(new CustomerName(customerName));
        }

        public static explicit operator CustomerName(string customerName)
        {
            return Create(customerName).Value;
        }

        public static implicit operator string(CustomerName customerName)
        {
            return customerName.Value;
        }
    }
}
