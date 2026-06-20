using FluentResults.Extensions.AspNetCore;
using LibraryManagment.Helpers.Errors;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagment.Helpers
{
    // This class converts FluentResults errors to HTTP responses
    // It decides which HTTP status code to return based on error type
    public class FluentResultsEndpointProfile : DefaultAspNetCoreResultEndpointProfile
    {
        // Provider to get HTTP context information
        private Func<HttpContext>? _httpContextProvider;

        // Set the HTTP context provider
        public void SetHttpContextProvider(Func<HttpContext> httpContextProvider)
        {
            _httpContextProvider = httpContextProvider;
        }

        // Override method that transforms failed results to HTTP responses
        public override ActionResult TransformFailedResultToActionResult(
            FailedResultToActionResultTransformationContext context)
        {
            var result = context.Result;

            // Check if the result has a validation error
            if (result.HasError<ValidationError>(out var validationErrors))
            {
                // Return HTTP 400 Bad Request with error details
                return new BadRequestObjectResult(
                    validationErrors.Select(e => new ErrorResponseDto(e.Message, e.ErrorCode)));
            }

            // Check if the result has a not found error
            if (result.HasError<NotFoundError>(out var notFoundErrors))
            {
                var notFoundError = notFoundErrors.First();

                // Return HTTP 404 Not Found with error details
                return new NotFoundObjectResult(
                    new ErrorResponseDto(notFoundError.Message, notFoundError.ErrorCode));
            }

            // Check if the result has a conflict error
            if (result.HasError<ConflictError>(out var conflictErrors))
            {
                var conflictError = conflictErrors.First();

                // Return HTTP 409 Conflict with error details
                return new ConflictObjectResult(
                    new ErrorResponseDto(conflictError.Message, conflictError.ErrorCode));
            }

            // Check if the result has an unauthorized error
            if (result.HasError<UnauthorizedError>(out var unauthorizedErrors))
            {
                var unauthorizedError = unauthorizedErrors.First();

                // Return HTTP 401 Unauthorized with error details
                return new UnauthorizedObjectResult(
                    new ErrorResponseDto(unauthorizedError.Message, unauthorizedError.ErrorCode));
            }

            // Check if the result has a forbidden error
            if (result.HasError<ForbiddenError>(out var forbiddenErrors))
            {
                var forbiddenError = forbiddenErrors.First();

                // Return HTTP 403 Forbidden with error details
                return new ObjectResult(
                    new ErrorResponseDto(forbiddenError.Message, forbiddenError.ErrorCode))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // Check if the result has a throttling error
            if (result.HasError<ThrottlingError>(out var throttlingErrors))
            {
                var error = throttlingErrors.First();

                // Create response with retry information
                var response = new ErrorResponseDto(error.Message, error.ErrorCode,
                    new Dictionary<string, object> { { "RetryAfter", error.RetryAfter } });

                // Return HTTP 429 Too Many Requests with retry time
                return new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status429TooManyRequests
                };
            }

            // Check if the result has an internal server error
            if (result.HasError<InternalServerError>(out var serverErrors))
            {
                var error = serverErrors.First();

                // Return HTTP 500 Internal Server Error with error details
                return new ObjectResult(
                    new ErrorResponseDto(error.Message, error.ErrorCode))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            // Check if the result has a general domain error
            if (result.HasError<DomainError>(out var domainErrors))
            {
                var domainError = domainErrors.First();

                // Return HTTP 400 Bad Request for generic domain errors
                return new BadRequestObjectResult(
                    new ErrorResponseDto(domainError.Message, domainError.ErrorCode));
            }

            // If no error matches, return a generic 500 error
            return new ObjectResult(new ErrorResponseDto("An unexpected error occurred", "500"))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}