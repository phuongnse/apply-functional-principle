using System;
using NullGuard;

namespace ApplyFunctionalPrinciple.Logic.Common
{
    public readonly struct Maybe<T> : IEquatable<Maybe<T>> where T : class
    {
        private Maybe([AllowNull] T value)
        {
            _value = value;
        }

        private readonly T _value;

        public T Value
        {
            get
            {
                if (HasNoValue)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        public bool HasValue => _value != null;
        public bool HasNoValue => !HasValue;

        public static implicit operator Maybe<T>([AllowNull] T value)
        {
            return new Maybe<T>(value);
        }

        public static bool operator ==(Maybe<T> left, T right)
        {
            return !left.HasNoValue && left._value.Equals(right);
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !(left == right);
        }

        public bool Equals(Maybe<T> other)
        {
            if (HasNoValue || other.HasNoValue)
                return false;

            if (HasNoValue && other.HasNoValue)
                return true;

            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Maybe<T> other))
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public override string ToString()
        {
            return HasNoValue ? "No value" : _value.ToString();
        }

        [return: AllowNull]
        public T Unwrap()
        {
            return HasValue ? _value : default;
        }

        [return: AllowNull]
        public TK Unwrap<TK>(Func<T, TK> selector)
        {
            return HasValue ? selector(_value) : default;
        }

        public Result<T> ToResult(string errorMessage)
        {
            return HasNoValue ? Result.Fail<T>(errorMessage) : Result.Ok(_value);
        }
    }
}