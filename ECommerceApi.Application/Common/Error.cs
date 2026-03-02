using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Common
{
    /// <summary>
    /// Describes why an operation failed. Immutable — created once, never modified.
    /// 
    /// ErrorCode is a machine-readable string like "Order.NotFound" or "Product.InsufficientStock".
    /// The API layer maps ErrorCodes to HTTP status codes.
    /// Message is human-readable and safe to send to the client.
    /// </summary>
    public sealed record Error(string Code, string Message)
    {
        public static Error NotFound(string entity, object id) => 
            new($"{entity}.NotFound", $"{entity} with ID '{id}' was not found.");
        
        public static Error NotFound(string entity, string field, object value) =>
            new($"{entity}.NotFound", $"{entity} with {field} '{value}' was not found.");
        
        public static Error Conflict(string entity, string reason) =>
            new($"{entity}.Conflict", reason);

        public static Error Validation(string field, string reason) =>
            new("Validation.Failed", $"Validation failed for '{field}': {reason}");

        public static Error Unauthorized(string reason = "You are not authorized to perform this action.") =>
            new("Authorization.Unauthorized", reason);

        public static Error Forbidden(string reason = "You do not have permission to access this resource.") =>
            new("Authorization.Forbidden", reason);

        public static Error BusinessRule(string code, string message) =>
            new(code, message);

        // Sentinel for cases where you need a non-null Error but have no failure
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}
