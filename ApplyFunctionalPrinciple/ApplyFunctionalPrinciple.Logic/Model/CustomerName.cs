using System.Collections.Generic;
using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class CustomerName : ValueObject<CustomerName>
    {
        private CustomerName(string value)
        {
            Value = value;
        }

        public string Value { get; }

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

            return customerName.Length > 200
                ? Result.Fail<CustomerName>("Customer name is too long")
                : Result.Ok(new CustomerName(customerName));
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