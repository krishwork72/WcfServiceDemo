using System.ServiceModel;

namespace WcfServiceDemo.ServiceContracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISampleService" in both code and config file together.
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        void DoWork();
        [OperationContract]
        string GetString();
    }
}
