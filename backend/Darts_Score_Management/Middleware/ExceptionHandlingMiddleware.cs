using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Darts_Score_Management.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = "Internal server error. Please try again later.",
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case ValidationException validationEx: 
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Validation Error";
                    problemDetails.Detail = validationEx.Message;
                    break;
                case ArgumentNullException argNullEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid Argument";
                    problemDetails.Detail = argNullEx.Message;
                    break;
                case ArgumentException argEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid Argument";
                    problemDetails.Detail = argEx.Message;
                    break;
                case DbUpdateException dbEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Database Error";
                    problemDetails.Detail = "An error occurred while saving to the database. " + (dbEx.InnerException?.Message ?? dbEx.Message);
                    break;
                case InvalidOperationException invalidOpEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Operation Error";
                    problemDetails.Detail = invalidOpEx.Message;
                    break;
                case KeyNotFoundException notFoundEx:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Not Found";
                    problemDetails.Detail = notFoundEx.Message;
                    break;
                default:
                    problemDetails.Extensions["errorId"] = Guid.NewGuid().ToString();
                    if (_env.IsDevelopment())
                    {
                        problemDetails.Extensions["stackTrace"] = exception.StackTrace;
                    }
                    break;
            }

            problemDetails.Extensions["correlationId"] = correlationId;

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
