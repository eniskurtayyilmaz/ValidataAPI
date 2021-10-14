using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ValidataAPI.Utils.Common;
using ValidataAPI.Utils.Services;
using ValidataAPI.Utils.Exceptions;
using ValidataAPI.Utils.Models;

namespace ValidataAPI.Api.Middleware
{
public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsonSerializerService _serializerService;
        private readonly IHttpContextService _contextService;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, IJsonSerializerService serializerService,
            IHttpContextService contextService, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _serializerService = serializerService;
            _contextService = contextService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessRuleException businessRuleException)
            {
                int statusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogWarning("Business Rule Exception Occurred", businessRuleException);
                await CreateErrorResponse(context, businessRuleException, statusCode,
                    _contextService.GetContextItemValue(Constants.TraceIdHeaderName));
            }
            catch (DomainNotFoundException notFoundException)
            {
                const int statusCode = (int)HttpStatusCode.NotFound;
                _logger.LogWarning("Domain not found", notFoundException);
                await CreateErrorResponse(context, notFoundException, statusCode,
                    _contextService.GetContextItemValue(Constants.TraceIdHeaderName));
            }
            catch (Exception ex)
            {
                const int statusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError("Unexpected Exception Occurred", ex);
                await CreateErrorResponse(context, ex, statusCode,
                    _contextService.GetContextItemValue(Constants.TraceIdHeaderName));
            }
        }

        private async Task CreateErrorResponse(HttpContext context, Exception exception, int statusCode, string traceId)
        {
            context.Response.ContentType = Constants.ContentTypeApplicationJson;
            context.Response.StatusCode = statusCode;
            var error = new ErrorResponse
            {
                StatusCode = statusCode,
                TraceId = traceId
            };
            error.Errors.Add(exception.GetType().ToString(), new[] { exception.Message });
            var serializedResponse = _serializerService.Serialize(error);
            await context.Response.WriteAsync(serializedResponse);
        }
    }
}