using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ECommerceApi.Application.Common;

public sealed class Result<T>
{
   
    private Result(bool isSuccess, T? value, Error error)
    {
        // Invariant: success requires a value (for non-void results), failure requires an error
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failed result must have an error.");

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    // Value is only meaningful when IsSuccess is true.
    // Accessing Value on a failed result should be treated as a programmer error.
    public T? Value { get; }

    // Error is only meaningful when IsFailure is true.
    public Error Error { get; }

    public static Result<T> Success(T value) =>
        new(true, value, Error.None);

    public static Result<T> Failure(Error error) =>
        new(false, default, error);

    // Convenience: implicit conversion from T creates a success result
    // Allows: return product; instead of return Result<Product>.Success(product);
    public static implicit operator Result<T>(T value) => Success(value);

    // Convenience: implicit conversion from Error creates a failure result
    // Allows: return Error.NotFound("Order", id); instead of Result<OrderDto>.Failure(...)
    public static implicit operator Result<T>(Error error) => Failure(error);
}

public sealed class Result
{
    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static readonly Result Success = new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}