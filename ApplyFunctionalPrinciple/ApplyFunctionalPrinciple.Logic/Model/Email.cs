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
            return maybeEmail
                .ToResult("Email should not be empty")
                .OnSuccess(email => email.Trim())
                .Ensure(email => email != string.Empty, "Email should not be empty")
                .Ensure(email => email.Length <= 256, "Email is too long")
                .Ensure(email => Regex.IsMatch(email, @"^(.+)@(.+)$"), "Email is invalid")
                .OnSuccess(email => new Email(email));
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