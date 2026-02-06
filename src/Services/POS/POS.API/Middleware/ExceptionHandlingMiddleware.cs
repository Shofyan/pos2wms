using System.Diagnostics;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using POS.Domain.Exceptions;

namespace POS.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => HandleValidationException(validationEx, traceId),
            DomainException domainEx => HandleDomainException(domainEx, traceId),
            OperationCanceledException => HandleOperationCancelled(traceId),
            _ => HandleUnhandledException(exception, traceId)
        };

        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                traceId);
        }
        else
        {
            _logger.LogWarning(
                "Handled exception: {ExceptionType} - {Message}. TraceId: {TraceId}",
                exception.GetType().Name,
                exception.Message,
                traceId);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private (int, ProblemDetails) HandleValidationException(ValidationException ex, string traceId)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return ((int)HttpStatusCode.BadRequest, new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Failed",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = traceId
        });
    }

    private (int, ProblemDetails) HandleDomainException(DomainException ex, string traceId)
    {
        return ((int)HttpStatusCode.BadRequest, new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Business Rule Violation",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = ex.Message,
            Instance = traceId,
            Extensions = { ["code"] = ex.Code }
        });
    }

    private (int, ProblemDetails) HandleOperationCancelled(string traceId)
    {
        return (499, new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Request Cancelled",
            Status = 499,
            Detail = "The request was cancelled by the client.",
            Instance = traceId
        });
    }

    private (int, ProblemDetails) HandleUnhandledException(Exception ex, string traceId)
    {
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = _environment.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
            Instance = traceId
        };

        if (_environment.IsDevelopment())
        {
            details.Extensions["exception"] = ex.ToString();
        }

        return ((int)HttpStatusCode.InternalServerError, details);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
