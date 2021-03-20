using Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static void UseCustomErrors(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.Use(DevelopmentErrorHandler);
            }
            else
            {
                app.Use(ProductionErrorHandler);
            }
        }

        private static Task DevelopmentErrorHandler(HttpContext httpContext, Func<Task> next)
        {
            return HandleError(httpContext, true);
        }
        private static Task ProductionErrorHandler(HttpContext httpContext, Func<Task> next)
        {
            return HandleError(httpContext, false);
        }

        private static async Task HandleError(HttpContext httpContext, bool includeDetails)
        {
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var exc = exceptionDetails?.Error;

            httpContext.Response.ContentType = "application/problem+json";
            if (exc is not null)
            {
                if (exc is ValidationException || exc is TaskCanceledException || exc is ExpectedException)
                {
                    await ProcessExpectedException(httpContext, exc, includeDetails);
                }
                else
                {
                    await ProcessUnexpectedException(httpContext, exc, includeDetails);
                }
            }
        }
        private static Task ProcessExpectedException(HttpContext httpContext, Exception exc, bool includeDetails)
        {
            return ProcessException(httpContext, exc, includeDetails, 400, exc.Message);
        }
        private static Task ProcessUnexpectedException(HttpContext httpContext, Exception exc, bool includeDetails)
        {
            return ProcessException(httpContext, exc, includeDetails, 500, "An error has occured");
        }
        private static Task ProcessException(HttpContext httpContext, Exception exc, bool includeDetails, int statusCode,
            string title)
        {
            string details = null;
            if (includeDetails)
            {
                title += $" {exc.Message}";
                details = exc.ToString();
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = details
            };
            var stream = httpContext.Response.Body;
            return JsonSerializer.SerializeAsync(stream, problem);
        }
    }

}
