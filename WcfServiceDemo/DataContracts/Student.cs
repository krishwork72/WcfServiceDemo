using System.Runtime.Serialization;

namespace WcfServiceDemo.DataContracts
{
    [DataContract]
    public class Student
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}