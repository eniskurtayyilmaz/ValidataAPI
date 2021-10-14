using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ValidataAPI.Utils.Services;
using ValidataAPI.Utils.Common;

namespace ValidataAPI.Api.Middleware
{
    public class ContextToLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextService _contextService;
        private readonly ILogger<ContextToLoggerMiddleware> _logger;

        public ContextToLoggerMiddleware(RequestDelegate next, IHttpContextService contextService,
            ILogger<ContextToLoggerMiddleware> logger)
        {
            _next = next;
            _contextService = contextService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var scopes = new List<KeyValuePair<string,object>>()
            {
                new(Constants.TraceIdHeaderName,
                    _contextService.GetContextItemValue(Constants.TraceIdHeaderName)),
                new(Constants.UserIdHeaderName,
                    _contextService.GetContextItemValue(Constants.UserIdHeaderName))
            };
            using (_logger.BeginScope(scopes))
            {
                await _next(context);
            }
        }
    }
}