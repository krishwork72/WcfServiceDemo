using System;
using System.Collections.Generic;
using WcfServiceDemo.DataContracts;
using WcfServiceDemo.ServiceContracts;

namespace WcfServiceDemo
{
    public class TestService : ITestService
    {
        public List<Student> GetStudentDetails()
        {
            List<Student> stuList = new List<Student>();
            stuList.Add(new Student { ID = "123", Name = "Krishna" });
            return stuList;
        }
        public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += " Suffix";
            }
            return composite;
        }
    }
}
