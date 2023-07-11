using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace FileServerApp
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteAsJsonAsync<T> (this HttpResponse response, T obj, JsonSerializerOptions options = null) 
        {
            response.ContentType = "application/json; charset=utf-8";
            await JsonSerializer.SerializeAsync(response.Body, obj, options);
        }

        public static async Task SendFileAttachmentAsync (this HttpResponse response, string filepath) 
        {
            response.Headers.Append ("Content-Type", 
                "application/octet-stream");
            response.Headers.Append ("Content-Disposition",
                $"attachment; filename=\"{Path.GetFileName(filepath)}\"");
            
            await response.SendFileAsync(filepath);
        }

        public static async Task SendFileAssetAsync (this HttpResponse response, string filepath) 
        {
            string contentType = "unknown/unknown";
            Mime.TryGetContentType(filepath, out contentType);
            response.Headers.Append ("Content-Type", contentType);
            response.Headers.Append ("Cache-Control", "public, max-age=100000000");
            await response.SendFileAsync(filepath);
        }

        public static async Task SendPreprocessedHtmlAsync (
                this HttpResponse response, 
                string filepath, 
                IDictionary<string, object> replacements) 
        {
            string preprocessingResult = HtmlPreprocessHelper.GetPreprocessedHtml(filepath, replacements, useCache: true);
            await response.WriteAsync(preprocessingResult);
        }

        public static async Task NotFound (this HttpResponse response)
        {
            response.StatusCode = 404;
            await response.WriteAsync("Error 404: File not found. :(");
        }

        static FileExtensionContentTypeProvider Mime = new FileExtensionContentTypeProvider();
    }
}
