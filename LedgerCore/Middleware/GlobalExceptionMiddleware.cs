using LedgerCore.Application.Exceptions;
using LedgerCore.Domain.Expections;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LedgerCore.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(
            HttpContext context,
            Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred after the response had already started.");

                return;
            }

            // The caller always sees the same generic 401, but the logs
            // distinguish a bad token (client problem) from an endpoint
            // that asked for an identity without being authenticated
            // (our bug, probably a missing [Authorize]).
            if (ex is MissingUserIdentityException identityEx)
            {
                if (identityEx.Reason == IdentityFailureReason.InvalidClaim)
                {
                    _logger.LogWarning(
                        "Authenticated token had a missing or invalid identity claim.");
                }
                else
                {
                    _logger.LogError(
                        ex,
                        "Identity was requested without an authenticated user ({Reason}). " +
                        "The endpoint is probably missing [Authorize].",
                        identityEx.Reason);
                }
            }

            var (statusCode, title, detail, shouldLog) = ex switch
            {
                MissingUserIdentityException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "Authentication is required.",
                        false
                    ),

                WalletNotFoundException =>
                    (
                        StatusCodes.Status404NotFound,
                        "Wallet not found",
                        "The requested wallet could not be found.",
                        false
                    ),

                UserNotFoundException =>
                    (
                        StatusCodes.Status404NotFound,
                        "User not found",
                        "The requested user could not be found.",
                        false
                    ),

                DuplicateAccountNumberException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Duplicate account number",
                        "The account number already exists.",
                        true
                    ),

                DuplicateEmailException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Duplicate email",
                        "The email address is already registered.",
                        true
                    ),

                InvalidRefreshTokenException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Invalid refresh token",
                        "The refresh token is invalid or cannot be used.",
                        false
                    ),

                InvalidCredentialsException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Invalid credentials",
                        "The email or password is incorrect.",
                        false
                    ),

                UnauthorizedAccessException =>
                    (
                        StatusCodes.Status403Forbidden,
                        "Forbidden",
                        "You are not authorized to perform this operation.",
                        true
                    ),

                InvalidOperationException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Invalid operation",
                        "The requested operation cannot be completed in the current state.",
                        false
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Internal server error",
                        "An unexpected error occurred.",
                        false   // logged once below as an error
                    )
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred.");
            }
            else if (shouldLog)
            {
                _logger.LogWarning(
                    ex,
                    "Handled application exception: {ExceptionType}",
                    ex.GetType().Name);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                problemDetails);
        }
    }
}