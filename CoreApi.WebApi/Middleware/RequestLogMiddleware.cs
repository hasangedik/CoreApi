using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace CoreApi.WebApi.Middleware
{
    [UsedImplicitly]
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await WriteLogAsync(httpContext);
            }
            catch
            {
                //ignored
            }
        }

        private async Task WriteLogAsync(HttpContext httpContext)
        {
            var request = await ReadRequestBody(httpContext);

            //Copy a pointer to the original response body stream
            var originalBodyStream = httpContext.Response.Body;

            //Create a new memory stream...
            await using var responseBody = new MemoryStream();
            //...and use that for the temporary response body
            httpContext.Response.Body = responseBody;

            //Continue down the Middleware pipeline, eventually returning to this class
            await _next(httpContext);
            //Format the response from the server
            var response = await ReadResponseBody(httpContext.Response);

            _ = Task.Run(() =>
              {
                  using (LogContext.PushProperty("RequestBody", request))
                  {
                      using (LogContext.PushProperty("ResponseBody", response))
                      {
                          try
                          {
                              Log.Information("Request ({@ResponseStatusCode}): {@RequestPath} \nRequestBody: {@RequestBody} \nResponse Body: {@ResponseBody}",
                                  httpContext.Response.StatusCode, httpContext.Request.Path.Value);
                          }
                          catch
                          {
                              //
                          }
                      }
                  }
              });

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadRequestBody(HttpContext context)
        {
            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                return string.Empty;

            context.Request.EnableBuffering();

            // Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, Convert.ToInt32(context.Request.ContentLength), true);
            var body = await reader.ReadToEndAsync();
            // Do some processing with body…

            // Reset the request body stream position so the next middleware can read it
            context.Request.Body.Position = 0;

            return body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }

    public static class RequestLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
