using MessageInterceptor.Models;
using System.Collections.Generic;

namespace MessageInterceptor.Filters
{
    public interface ICheckApiInterceptor
    {
        bool DoIntercept(List<HeaderModel> headers, string payload);
    }
}
