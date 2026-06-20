namespace LibraryManagment.Helpers.Errors
{
    using FluentResults;

    // Base class for all custom errors in the application
    public abstract class DomainError : Error
    {
        // Code to identify the error type (422, 404, etc.)
        public string ErrorCode { get; }

        // Constructor that creates a new error with message and code
        protected DomainError(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    // Error when validation fails (example: invalid email)
    public class ValidationError : DomainError
    {
        // Name of the property that failed validation
        public string PropertyName { get; }

        // Create a validation error with property name and message
        public ValidationError(string propertyName, string message)
            : base($"Validation failed for '{propertyName}': {message}", "422")
        {
            PropertyName = propertyName;
        }
    }

    // Error when resource is not found in database
    public class NotFoundError : DomainError
    {
        // Name of the entity that was not found (example: "User")
        public string EntityName { get; }

        // The ID that was not found
        public object Id { get; }

        // Create a not found error with entity name and ID
        public NotFoundError(string entityName, object id)
            : base($"'{entityName}' with id '{id}' not found.", "404")
        {
            EntityName = entityName;
            Id = id;
        }
    }

    // Error when resource already exists (example: duplicate email)
    public class ConflictError : DomainError
    {
        // Create a conflict error with message
        public ConflictError(string message)
            : base(message, "409") { }
    }

    // Error when user is not authenticated
    public class UnauthorizedError : DomainError
    {
        // Create an unauthorized error with message
        public UnauthorizedError(string message)
            : base(message, "401") { }
    }

    // Error when user does not have permission
    public class ForbiddenError : DomainError
    {
        // Create a forbidden error with message
        public ForbiddenError(string message)
            : base(message, "403") { }
    }

    // Error when user makes too many requests (rate limiting)
    public class ThrottlingError : DomainError
    {
        // Time in seconds to wait before making another request
        public int RetryAfter { get; }

        // Create a throttling error with message and retry time
        public ThrottlingError(string message, int retryAfter)
            : base(message, "429")
        {
            RetryAfter = retryAfter;
        }
    }

    // Error when something unexpected happens in the server
    public class InternalServerError : DomainError
    {
        // Create an internal server error with message
        public InternalServerError(string message)
            : base(message, "500") { }
    }
}