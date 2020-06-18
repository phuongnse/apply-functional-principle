using NullGuard;
using System;

namespace ApplyFunctionalPrinciple.Logic.Common
{
    public class Result
    {
        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(error) && !isSuccess)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default, false, message);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        protected internal Result([AllowNull] T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value => !IsSuccess ? throw new InvalidOperationException() : _value;
    }
}