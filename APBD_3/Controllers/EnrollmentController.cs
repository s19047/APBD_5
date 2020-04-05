using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_3.DTOs.Requests;
using APBD_3.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD_3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        String CONNECTION_STRING = @"Data Source=LAPTOP-11FAC326\SQLEXPRESS;Initial Catalog=s19047;Integrated Security=True";

       [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            //check if all required data has been delivered
                //we already do that using anotations in EnrollStudentRequest Class

           

            // Check if enrollment exists else Insert
            //Check if index exists else Insert/400
            //return Enrollment model
            //DTO - Data transfer Objects 

            
            using (var client = new SqlConnection(CONNECTION_STRING))
            using (var command = new SqlCommand())
            { 
               command.CommandText = "SELECT IdStudy FROM Studies where Name = '@Name';";
               command.Parameters.AddWithValue("Name", request.Studies);
                command.Connection = client;

                client.Open();
                var tran = client.BeginTransaction();
                try
                    {
                        var reader = command.ExecuteReader();

                        //check if studies exist else rollback + 404
                        if (!reader.Read())
                        {
                            tran.Rollback();
                            return BadRequest("Study Doesn't Exist");
                            //studies does not exist
                        }
                        int IdStudy = (int)reader["IdStudy"];

                        // Note that the assignment asks to search for values where semester = 1; However, our tables have 
                        // years as semesters so for example 2020 , for that reason I just assumed 2020 as the default semester
                        //instead of 1
                        int enrollId = 0;
                        command.CommandText = "SELECT * FROM Enrollment  where Semester = 2020 AND IdStudy='@IdStudy'";
                        command.Parameters.AddWithValue("IdStudy", IdStudy);

                        reader = command.ExecuteReader();

                        //check if enrollment already exists , else insert one 

                        if (!reader.Read())
                        {
                            command.CommandText = "Select Top 1 IdEnrollment from Enrollment Order By  IdEnrollment DESC";
                            reader = command.ExecuteReader();
                            enrollId = int.Parse(reader["IdEnrollment"].ToString());

                            command.CommandText = "insert into Enrollment(IdEnrollment,IdStudy, Semester, StartDate) values (@enrollId, @IdStudy, @Semester, '@Date')";
                            command.Parameters.AddWithValue("enrollId", ++enrollId);
                            command.Parameters.AddWithValue("IdStudy", IdStudy);
                            command.Parameters.AddWithValue("Semester", "2020");
                            command.Parameters.AddWithValue("Date", DateTime.Now);

                        }
                        else
                        {
                            enrollId = int.Parse(reader["IdEnrollment"].ToString());
                        }


                        //check if index number was assigned to any other student , if not insert student 
                        command.CommandText = "Select * from Student where IndexNumber = '@index'";
                        command.Parameters.AddWithValue("index", request.IndexNumber);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            tran.Rollback();
                            return BadRequest("Student Already Exists!");
                        }

                        command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@FirstName,@LastName,@Birthdate,@enrollId";
                        command.Parameters.AddWithValue("FirstName", request.FirstName);
                        command.Parameters.AddWithValue("LastName", request.LastName);
                        command.Parameters.AddWithValue("BirthDate", request.Birthdate);
                        command.Parameters.AddWithValue("enrollId", enrollId);
                        command.ExecuteNonQuery();


                        tran.Commit();
                        client.Close();

                    } catch (SqlException e)
                    {
                        tran.Rollback();

                    }
                
            }

            var response = new EnrollStudentResponse()
            {
                request = request,
                semester = 2020
            };

            return CreatedAtAction(nameof(EnrollStudent),response);
        }

        

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(String Studies,int Semester )
        {
            //check if Enrollment exists
            using (var client = new SqlConnection(CONNECTION_STRING))
            using (var command = new SqlCommand())
            {
                
                command.CommandText = "SELECT IdStudy FROM Studies where Name = '@Name';";
                command.Parameters.AddWithValue("Name", Studies);
                command.Connection = client;

                client.Open();
                var tran = client.BeginTransaction();

                var reader = command.ExecuteReader();

                    //check if studies exist else rollback + 404
                    if (!reader.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Study Doesn't Exist");
                        //studies does not exist
                    }
                    int IdStudy = (int)reader["IdStudy"];

                    command.CommandText = "SELECT * FROM Enrollment  where Semester = @Semester AND IdStudy='@IdStudy'";
                    command.Parameters.AddWithValue("IdStudy", IdStudy);
                    command.Parameters.AddWithValue("Semester", Semester);

                    reader = command.ExecuteReader();

                    //check if studies exist else rollback + 404
                    if (!reader.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Enrollment Doesn't Exist");
                        //studies does not exist
                    }

            }
                //Find all the students froms tudies = IT and semester=1
                //Promote all student to the 2 semester
                // find an enrollment record with studies= IT and semester = 2 -> IdEnrollment=10


                //create stored procedure 
                return Ok();
        }
    }
}