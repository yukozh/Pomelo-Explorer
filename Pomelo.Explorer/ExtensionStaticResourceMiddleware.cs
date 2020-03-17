using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Pomelo.Explorer
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExtensionStaticResourceMiddleware
{
    private readonly RequestDelegate _next;

    public ExtensionStaticResourceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
            if (!httpContext.Request.Path.Value.StartsWith("/static"))
            {
                await _next(httpContext);
                return;
            }

            var resourcePath = AssemblyHelper.ConvertUrlToResourcePath(httpContext.Request.Path.Value);
            if (resourcePath == null)
            {
                await _next(httpContext);
                return;
            }

            var id = AssemblyHelper.GetAssemblyIdFromUrl(httpContext.Request.Path.Value);
            if (id == null)
            {
                await _next(httpContext);
                return;
            }
            var asm = AssemblyHelper.GetAssemblyById(id);
            using (var stream = asm.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    await _next(httpContext);
                    return;
                }

                await stream.CopyToAsync(httpContext.Response.Body);
            }
            await httpContext.Response.CompleteAsync();
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExtensionStaticResourceMiddlewareExtensions
{
    public static IApplicationBuilder UseExtensionStaticResourceMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExtensionStaticResourceMiddleware>();
    }
}
}
