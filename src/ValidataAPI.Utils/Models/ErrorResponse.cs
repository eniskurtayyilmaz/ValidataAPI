using System.Collections.Generic;

namespace ValidataAPI.Utils.Models
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            Errors = new Dictionary<string, string[]>();
        }
        public int StatusCode { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string,string[]> Errors { get; set; }
    }
}