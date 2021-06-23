using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotnet_Core.Middlewares
{
    public class DataRequest
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string Body { get; set; }
    }
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env)
        {
            var scheme = context.Request.Scheme;
            var host = context.Request.Host.ToString();
            var path = context.Request.Path;
            var query = context.Request.QueryString.ToString();
            var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
            
            List<DataRequest> data = new List<DataRequest>();

            data.Add(new DataRequest() {
                Scheme = scheme,
                Host = host,
                Path = path,
                Query = query,
                Body = bodyAsText,
            });

            string filePath = Path.Combine(env.WebRootPath, $"data/data.json");
            var file = System.IO.File.Create(filePath);
            await JsonSerializer.SerializeAsync(file, data);
            await file.DisposeAsync();
        }
    }

    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }
}