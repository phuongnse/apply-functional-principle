using System;
using NullGuard;

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

        public static Result Combine(params Result[] results)
        {
            foreach (var result in results)
                if (result.IsFailure)
                    return result;

            return Ok();
        }

        public T OnBoth<T>(Func<Result, T> func)
        {
            return func(this);
        }

        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
                action();

            return this;
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

        public Result<TK> OnSuccess<TK>(Func<T, TK> func)
        {
            return IsFailure ? Fail<TK>(Error) : Ok(func(_value));
        }

        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess)
                action(_value);

            return this;
        }

        public Result<T> Ensure(Func<T, bool> predicate, string errorMessage)
        {
            if (IsFailure)
                return this;

            return !predicate(_value) ? Fail<T>(errorMessage) : this;
        }

        public TK OnBoth<TK>(Func<Result<T>, TK> func)
        {
            return func(this);
        }
    }
}