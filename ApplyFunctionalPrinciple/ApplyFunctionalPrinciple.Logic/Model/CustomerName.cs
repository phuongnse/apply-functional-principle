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
            return maybeCustomerName
                .ToResult("Customer name should not be empty")
                .OnSuccess(customerName => customerName.Trim())
                .Ensure(customerName => customerName != string.Empty, "Customer name should not be empty")
                .Ensure(customerName => customerName.Length <= 200, "Customer name is too long")
                .OnSuccess(customerName => new CustomerName(customerName));
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