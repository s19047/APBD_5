using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_3.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD_3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        List<Student> students = new List<Student>();

        String CONNECTION_STRING = @"Data Source=LAPTOP-11FAC326\SQLEXPRESS;Initial Catalog=s19047;Integrated Security=True";

        [HttpGet]
       public IActionResult GetStudents(String orderby = "FirstName")
        {
            

            using (var client = new SqlConnection(CONNECTION_STRING))
            using(var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name as Studies, e.Semester "+
                                       "from Student s "+
                                       "join Enrollment e on e.IdEnrollment = s.IdEnrollment " +
                                       "join Studies st on st.IdStudy = e.IdStudy; ";
                client.Open();
                var response = command.ExecuteReader();
                while (response.Read()) {
                    var st = new Student();
                    st.FirstName = response["FirstName"].ToString();
                    st.DoB = DateTime.Parse(response["BirthDate"].ToString());
                    st.LastName = response["LastName"].ToString();
                    st.Studies = response["Studies"].ToString();
                    st.Semester = int.Parse(response["Semester"].ToString());

                    students.Add(st);
                }

            }
            return Ok(students);
        }

        //getting the semester number by Index number

        [HttpGet("{indexNumber}")]
        public IActionResult GetSemester(string indexNumber)
        {
            int semester = 0;
            using (var client = new SqlConnection(CONNECTION_STRING))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "SELECT e.Semester " +
                                       "FROM Student s , Enrollment e " +
                                       "WHERE s.IdEnrollment = e.IdEnrollment and s.IndexNumber = '@index';";
                command.Parameters.AddWithValue("index",indexNumber);
                client.Open();
                var response = command.ExecuteReader();
                if (response.Read())
                {
                    semester = int.Parse(response["Semester"].ToString());
                }

            }

               
                return Ok(semester);
        }
/*
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1,20000)}";
            return Ok(student);
        }
        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {
            
            _DbService.deleteStudent(id);
            return Ok("Delete completed");
        }

        //I realize the task said that id should be inputted 
        //however i feel like having a student inputed makes more sense
        //since i want to be able to update things like first or last name

        [HttpPut]
        public IActionResult UpdateStudent(Student student)
        {
            _DbService.UpdateStudent(student);
            return Ok("Update completed");
        }

    */
    }
}