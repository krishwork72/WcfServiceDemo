using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIDemo.Core.Models;

namespace WebAPIDemo.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetEmployees()
        {
            var Employees = new List<Employee>()
            {
                new Employee(){ Id=1,Name="Krishna"},
                new Employee(){ Id=2, Name="Murari" }
            };
            return Ok(Employees);
        }
        [HttpGet]
        public async Task<ActionResult> GetEmployeeById(int EmployeeId)
        {
            var Employee = new Employee() { Id = EmployeeId, Name = "Krishna" };
            return Ok(await Task.FromResult(Employee));
        }
        [HttpPost]
        public ActionResult SaveEmployee(int id,[FromBody] Employee Employee)
        {
            var std = Employee;
            std.Name = "MR. " + std.Name;
            return Ok(std);
        }
    }
}
