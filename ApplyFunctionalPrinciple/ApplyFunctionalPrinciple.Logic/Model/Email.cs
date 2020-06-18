using System.Collections.Generic;
using System.Text.RegularExpressions;
using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public sealed class Email : ValueObject<Email>
    {
        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static Result<Email> Create(Maybe<string> maybeEmail)
        {
            if (maybeEmail.HasNoValue)
                return Result.Fail<Email>("Email should not be empty");

            var email = maybeEmail.Value.Trim();

            if (email == string.Empty)
                return Result.Fail<Email>("Email should not be empty");

            if (email.Length > 256)
                return Result.Fail<Email>("Email is too long");

            return !Regex.IsMatch(email, @"^(.+)@(.+)$")
                ? Result.Fail<Email>("Email is invalid")
                : Result.Ok(new Email(email));
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