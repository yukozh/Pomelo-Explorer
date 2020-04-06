using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Pomelo.Explorer
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VueMiddleware
    {
        private readonly RequestDelegate _next;

        public VueMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var id = AssemblyHelper.GetAssemblyIdFromUrl(httpContext.Request.Path);
            var endpoint = httpContext.Request.Path.Value.Replace("/", "\\");
            if (endpoint.Length >= 1)
            {
                endpoint = endpoint.Substring(1);
            }
            var diskPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", endpoint + ".html");
            var resourcePath = AssemblyHelper.ConvertUrlToResourcePath(httpContext.Request.Path + ".html");

            if (File.Exists(diskPath)
                || id != null && AssemblyHelper.GetAssemblyById(id).GetManifestResourceNames().Contains(resourcePath))
            {
                await httpContext.Response.WriteAsync(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html")));
                await httpContext.Response.CompleteAsync();
                return;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class VueMiddlewareExtensions
    {
        public static IApplicationBuilder UseVueMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VueMiddleware>();
        }
    }
}
