﻿using ApplyFunctionalPrinciple.Logic.Common;
using System.Collections.Generic;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class CustomerName : ValueObject<CustomerName>
    {
        private string Value { get; }

        private CustomerName(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        private static Result<CustomerName> Create(string customerName)
        {
            if (customerName == null)
                return Result.Fail<CustomerName>("Customer name should not be empty");

            customerName = customerName.Trim();

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
