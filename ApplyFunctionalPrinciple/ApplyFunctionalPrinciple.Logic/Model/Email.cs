using ApplyFunctionalPrinciple.Logic.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class Email : ValueObject<Email>
    {
        private string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        private static Result<Email> Create(string email)
        {
            if (email == null)
                return Result.Fail<Email>("Email should not be empty");

            email = email.Trim();

            if (email == string.Empty)
                return Result.Fail<Email>("Email should not be empty");

            if (email.Length > 256)
                return Result.Fail<Email>("Email is too long");

            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return Result.Fail<Email>("Email is invalid");

            return Result.Ok(new Email(email));
        }

        public static explicit operator Email(string email)
        {
            return Create(email).Value;
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }
    }
}
