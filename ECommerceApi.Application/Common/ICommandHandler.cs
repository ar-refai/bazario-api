using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Common
{
    /// TCommand: the input (e.g., PlaceOrderCommand)
    /// TResult: what you get back on success (e.g., Guid — the new order's ID)
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface ICommandHandler<TCommand>
    {
        Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
