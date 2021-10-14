using Microsoft.AspNetCore.Http;

namespace ValidataAPI.Utils.Services
{
    public interface IHttpContextService
    {
        string GetContextItemValue(string key);
    }

    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _context;

        public HttpContextService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetContextItemValue(string key)
        {
            return _context.HttpContext?.Items[key]?.ToString();
        }
    }
}