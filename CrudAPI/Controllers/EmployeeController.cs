using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrudAPI.Controllers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CrudAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpGet]
        public JsonResult Get()
        {
            var query = @"select EmployeeId, EmployeeName,
                        Department, convert(varchar(10),DateOfJoining,120) as DateOfJoining,
                        PhotoFileName from dbo.Employee";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myCon.Close();
                }
            }
            return new JsonResult(table);


        }

        [HttpPost]
        public JsonResult Post(Employee employee)
        {

            var query = @"insert into dbo.Employee
                        (EmployeeName, Department, DateOfJoining, PhotoFileName)    
                        values
                        (
                        '" + employee.EmployeeName + @"'
                        ,'" + employee.Department + @"'
                        ,'" + employee.DateOfJoining + @"'
                        ,'" + employee.PhotoFileName + @"'
                        
                        )";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully!");
        }
        [HttpPut]
        public JsonResult Put(Employee employee)
        {

            var query = @"
                        update dbo.Employee set
                        EmployeeName='" + employee.EmployeeName + @"'
                        ,Department='" + employee.Department + @"'
                        ,DateOfJoining='" + employee.DateOfJoining + @"'
                        ,PhotoFileName='" + employee.PhotoFileName + @"'
                        where EmployeeId= " + employee.EmployeeId + @"
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myCon.Close();
                }
            }
            return new JsonResult("Updated Successfully!");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {

            var query = @"
                        delete from dbo.Employee
                        where EmployeeId= " + id + @"
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myCon.Close();
                }
            }
            return new JsonResult("Deleted  Successfully!");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

                using(var stream= new FileStream(physicalPath,FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(fileName);
            }
            catch(Exception)
            {
                return new JsonResult("anonymous.jpg");
            }
        }

        [Route("GetAllDepartmentNames")]
        [HttpGet]
        public JsonResult GetAllDepartmentNames()
        {
            var query = @"select DepartmentName from dbo.Department";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myCon.Close();
                }
            }
            return new JsonResult(table);

        }
    }
}
