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

            var (statusCode, title, detail, shouldLog) = ex switch
            {
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
                    InvalidRefreshTokenException =>
                    (
                    StatusCodes.Status401Unauthorized,
                    "Invalid refresh token",
                    "The refresh token is invaild or can't be used",
                    false
                    ),

                DuplicateEmailException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Duplicate email",
                        "The email address is already registered.",
                        true
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

                InvalidCredentialsException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Invalid credentials",
                        "The email or password is incorrect.",
                        false
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Internal server error",
                        "An unexpected error occurred.",
                        true
                    )
            };

            if (shouldLog)
            {
                _logger.LogWarning(
                    ex,
                    "Handled application exception: {ExceptionType}",
                    ex.GetType().Name);
            }

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred.");
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