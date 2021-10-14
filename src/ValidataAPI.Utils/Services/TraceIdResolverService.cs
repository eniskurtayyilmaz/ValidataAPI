using ValidataAPI.Utils.Common;

namespace ValidataAPI.Utils.Services
{
    public interface ITraceIdResolverService
    {
        string GetTraceId();
    }

    public class TraceIdResolverService : ITraceIdResolverService
    {
        private readonly IHttpContextService _contextService;

        public TraceIdResolverService(IHttpContextService contextService)
        {
            _contextService = contextService;
        }

        public string GetTraceId()
        {
            return _contextService.GetContextItemValue(Constants.TraceIdHeaderName);
        }
    }
}