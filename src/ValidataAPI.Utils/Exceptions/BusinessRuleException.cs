using System;

namespace ValidataAPI.Utils.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public string UserMessage { get; set; }
        public BusinessRuleException(string userMessage) : base($"{userMessage}")
        {
            this.UserMessage = userMessage;
        }
    }
    public class DomainNotFoundException : Exception
    {
        public DomainNotFoundException(string domain, string id) : base($"{domain}-{id}")
        {
        }
    }
}