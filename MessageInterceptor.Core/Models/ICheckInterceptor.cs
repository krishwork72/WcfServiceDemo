using System.Collections.Generic;

namespace MessageInterceptor.Core.Models
{
    public interface ICheckInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, IDictionary<string, object> payloads);
    }
}