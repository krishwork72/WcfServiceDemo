using WcfServiceDemo.ServiceContracts;

namespace WcfServiceDemo
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SampleService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SampleService.svc or SampleService.svc.cs at the Solution Explorer and start debugging.
    public class SampleService : ISampleService
    {
        public void DoWork()
        {

        }
        public string GetString()
        {
            return "Krishna";
        }
    }
}
