using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfServiceDemo.Constants;
using WcfServiceDemo.DataContracts;

namespace WcfServiceDemo.ServiceContracts
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        [WebInvoke(Method = HttpVerbs.HttpGet, UriTemplate = "GetStudentDetails")]
        List<Student> GetStudentDetails();
        [OperationContract]
        [WebInvoke(Method = HttpVerbs.HttpGet, UriTemplate = "GetData/{value}",ResponseFormat = WebMessageFormat.Json)]
        string GetData(string value);

        [OperationContract]
        [WebInvoke(Method = HttpVerbs.HttpPost, UriTemplate = "GetDataUsingDataContract")]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }
}
