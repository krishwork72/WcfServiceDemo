using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPIDemo.Models;

namespace WebAPIDemo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmployeeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetEmployees()
        {
            var students = new List<Student>()
            {
                new Student(){ Id=1,Name="Krishna"},
                new Student(){ Id=2, Name="Murari" }
            };
            return Ok(students);
        }
        [HttpGet]
        public IHttpActionResult GetEmployeeById(int studentId)
        {
            var student = new Student() { Id = studentId, Name = "Krishna" };
            return Ok(student);
        }
        [HttpPost]
        public IHttpActionResult SaveEmployee([FromBody] Student student)
        {
            var std = student;
            std.Name ="MR. " +std.Name;
            return Ok(std);
        }
    }
}
