using System;
using System.Net;
using System.Threading.Tasks;
using CoreApi.Contract;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace CoreApi.WebApi.Middleware
{
    [UsedImplicitly]
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            var result = new ResponseContract
            {
                Error = true,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = exception.Message
            };

            Task.Run(() =>
            {
                Log.Error(exception, exception.Message);
            });

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
