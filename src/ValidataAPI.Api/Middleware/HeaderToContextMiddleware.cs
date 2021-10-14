using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ValidataAPI.Utils.Common;
using ValidataAPI.Utils.Generators;

namespace ValidataAPI.Api.Middleware
{
    public class HeaderToContextMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderToContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceIdHeader = context.Request.Headers[Constants.TraceIdHeaderName];
            var traceId = traceIdHeader.Count > 0 ? traceIdHeader[0] : GuidUtils.New().ToString();
            var userIdHeader = context.Request.Headers[Constants.UserIdHeaderName];
            var userId = userIdHeader.Count > 0 ? userIdHeader[0] : Constants.AnonymousUserName;
            context.Items[Constants.TraceIdHeaderName] = traceId;
            context.Items[Constants.UserIdHeaderName] = userId;
            context.Response.Headers[Constants.TraceIdHeaderName] = traceId;
            await _next(context);
        }
    }
}